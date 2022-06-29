using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using telegram_queue_bot.Menus;
using telegram_queue_bot.DataStructures;
using StackExchange.Redis;

namespace telegram_queue_bot
{
    public enum LogMethod { StdOut, None };

    public class Bot
    {
        public static Bot Instance { get { return lazy.Value; } }
        private static readonly Lazy<Bot> lazy = new(new Bot());

        public Dictionary<long, Queue> CurrentQueues { get; private set; }
        public List<Queue> Queues { get; private set; }
        public List<string> Admins { get; private set; }

        public TelegramBotClient BotClient { get; private set; }

        public LogMethod LogMethod { get; private set; }
        public bool isDevMode { get; private set; }

        private Bot()
        {
            var env = ReadEnvironment();
            BotClient = new(env[IEnvNames.BotToken]);
            Admins = LoadAdmins(env[IEnvNames.Admins]);

            if (env[IEnvNames.LogMethod] != "")
            {
                switch (env[IEnvNames.LogMethod].ToLower())
                {
                    case "stdout":
                        LogMethod = LogMethod.StdOut;
                        break;
                    default:
                        LogMethod = LogMethod.None;
                        break;
                }
            }
            else
            {
                LogMethod = LogMethod.None;
            }

            if (env[IEnvNames.Dev] != "")
            {
                switch (env[IEnvNames.Dev].ToLower())
                {
                    case "true":
                    case "enabled":
                    case "1":
                        isDevMode = true;
                        break;
                    default:
                        isDevMode = false;
                        break;
                }
            }
            else
            {
                isDevMode = false;
            }

            Queues = new();
            CurrentQueues = new();
        }

        public async Task RunAsync()
        {
            RestoreQueuesState();
            RestoreCurrentQueuesState();

            using var cts = new CancellationTokenSource();

            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            };

            BotClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            if (!isDevMode)
            {
                Thread.Sleep(-1);
            }
            else
            {
                Console.WriteLine($"isDevMode: {isDevMode}");
                Console.ReadLine();
            }
            cts.Cancel();

            SaveQueuesState();
            SaveCurrentQueuesState();
        }

        public bool IsAdmin(Message message)
        {
            if (Admins.Contains(message.Chat.Id.ToString()))
            {
                return true;
            }
            return false;
        }

        public bool IsAdmin(CallbackQuery callbackQuery)
        {
            if (Admins.Contains(callbackQuery.Message.Chat.Id.ToString()))
            {
                return true;
            }
            return false;
        }

        private List<string> LoadAdmins(string envVar)
        {
            List<string> admins = new();

            var envVarSplit = envVar.Split(",");

            foreach (var item in envVarSplit)
            {
                admins.Add(item);
            }

            return admins;
        }

        public void RemoveCurrentQueue(User user)
        {
            if (CurrentQueues.Remove(user.Id))
            {
                SaveCurrentQueuesState();
            }
        }

        public void SetCurrentQueue(User user, Queue queue)
        {
            CurrentQueues.Remove(user.Id);
            CurrentQueues.Add(user.Id, queue);
            SaveCurrentQueuesState();
        }

        public Queue? GetCurrentQueue(User user)
        {
            foreach (var item in CurrentQueues)
            {
                if (item.Key == user.Id)
                {
                    return item.Value;
                }
            }
            return null;
        }

        public bool QueuesContains(string name)
        {
            if (name == null || name == "") return false;

            foreach (var item in Queues)
            {
                if (item.Name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public bool CurrentQueuesContains(User user)
        {
            var currentQueue = GetCurrentQueue(user);
            if (currentQueue == null) return false;

            foreach (var item in Program.Bot.Queues)
            {
                if (currentQueue.Name == item.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddQueue(string name)
        {
            if (name == "") return;
            foreach (var item in Queues)
            {
                if (item.Name == name)
                {
                    return;
                }
            }
            Queues.Add(new Queue(name));
            SaveQueuesState();
        }

        public void RemoveQueue(string name)
        {
            if (name == "") return;
            foreach (var item in Queues.ToList<Queue>())
            {
                if (item.Name == name)
                {
                    item.Clear();
                    Queues.Remove(item);
                }
            }
            SaveQueuesState();

            foreach (var item in CurrentQueues)
            {
                if (item.Value.Name == name)
                {
                    CurrentQueues.Remove(item.Key);
                }
            }

            SaveCurrentQueuesState();
        }

        private void RestoreQueuesState()
        {
            string queuesSerialized = "" + Program.Db.StringGet(IRedisValueNames.QueuesState);
            if (queuesSerialized == "") return;
            foreach (var item in queuesSerialized.Split(" "))
            {
                AddQueue(item);
            }
        }

        private void SaveQueuesState()
        {
            string queuesSerialized = "";
            foreach (var item in Queues)
            {
                queuesSerialized += $"{item.Name} ";
            }
            Program.Db.StringSet(IRedisValueNames.QueuesState, queuesSerialized);
        }

        private void RestoreCurrentQueuesState()
        {
            string currentQueuesSerialized = "" + Program.Db.StringGet(IRedisValueNames.CurrentQueuesState);
            if (currentQueuesSerialized == "") return;
            foreach (var item in currentQueuesSerialized.Split(" "))
            {
                var itemData = item.Split(":");
                if (itemData.Length == 2)
                {
                    CurrentQueues.Add(long.Parse(itemData[0]), new(itemData[1]));
                }
            }
        }

        private void SaveCurrentQueuesState()
        {
            string currentQueuesSerialized = "";
            foreach (var item in CurrentQueues)
            {
                currentQueuesSerialized += $"{item.Key}:{item.Value.Name} ";
            }
            Program.Db.StringSet(IRedisValueNames.CurrentQueuesState, currentQueuesSerialized);
        }

        private Dictionary<string, string> ReadEnvironment()
        {
            Dictionary<string, string> env = new();

            var EnvRedisUrl = "" + Environment.GetEnvironmentVariable(IEnvNames.RedisUrl);
            var EnvBotToken = "" + Environment.GetEnvironmentVariable(IEnvNames.BotToken);
            var EnvAdmins = "" + Environment.GetEnvironmentVariable(IEnvNames.Admins);
            var EnvLogMethod = "" + Environment.GetEnvironmentVariable(IEnvNames.LogMethod);
            var EnvDev = "" + Environment.GetEnvironmentVariable(IEnvNames.Dev);
            if (EnvRedisUrl == "") EnvRedisUrl = IEnvDefaults.EnvRedisUrl;

            env.Add(IEnvNames.RedisUrl, EnvRedisUrl);
            env.Add(IEnvNames.BotToken, EnvBotToken);
            env.Add(IEnvNames.Admins, EnvAdmins);
            env.Add(IEnvNames.LogMethod, EnvLogMethod);
            env.Add(IEnvNames.Dev, EnvDev);

            return env;
        }

        async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await MainMenu.HandleMessageAsync(botClient, message, cancellationToken);
            await BotCmds.HandleMessageAsync(botClient, message, cancellationToken);
        }

        async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            await MainMenu.HandleCallbackQueryAsync(botClient, callbackQuery, cancellationToken);
            await QueuesListMenu.HandleCallbackQueryAsync(botClient, callbackQuery, cancellationToken);
            await QueueMenu.HandleCallbackQueryAsync(botClient, callbackQuery, cancellationToken);
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.CallbackQuery != null)
            {
                if (LogMethod == LogMethod.StdOut)
                {
                    Console.WriteLine($"[C] {update.CallbackQuery.Data}");
                }
                await HandleCallbackQuery(botClient, update.CallbackQuery, cancellationToken);
            }
            else if (update.Message != null)
            {
                if (LogMethod == LogMethod.StdOut)
                {
                    Console.WriteLine($"[M] {update.Message.Text}");
                }
                await HandleMessageAsync(botClient, update.Message, cancellationToken);
            }
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
