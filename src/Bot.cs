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

namespace telegram_queue_bot
{
    public class Bot
    {
        public static Bot Instance { get { return lazy.Value; } }
        private static readonly Lazy<Bot> lazy = new(new Bot());

        public KeyValuePair<long, Queue> CurrentQueues { get; private set; }
        public List<Queue> Queues { get; private set; }

        public TelegramBotClient BotClient { get; private set; }

        private Bot()
        {
            var env = ReadEnvironment();
            BotClient = new(env[IEnvNames.BotToken]);

            Queues = new();
            CurrentQueues = new();
        }

        public async Task RunAsync()
        {
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

            // Thread.Sleep(-1);
            Console.ReadLine();
            cts.Cancel();
        }

        private void RestoreQueuesState()
        {
        }

        private void SaveQueuesState()
        {
        }

        private void RestoreCurrentQueuesState()
        {
        }

        private void SaveCurrentQueuesState()
        {
        }

        private Dictionary<string, string> ReadEnvironment()
        {
            Dictionary<string, string> env = new();

            var EnvRedisUrl = "" + Environment.GetEnvironmentVariable(IEnvNames.RedisUrl);
            var EnvBotToken = "" + Environment.GetEnvironmentVariable(IEnvNames.BotToken);

            if (EnvRedisUrl == "") EnvRedisUrl = IEnvDefaults.EnvRedisUrl;

            env.Add(IEnvNames.RedisUrl, EnvRedisUrl);
            env.Add(IEnvNames.BotToken, EnvBotToken);

            return env;
        }

        async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            await MainMenu.HandleMessageAsync(botClient, message, cancellationToken);
        }

        async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            await MainMenu.HandleCallbackQueryAsync(botClient, callbackQuery, cancellationToken);
            await QueuesListMenu.HandleCallbackQueryAsync(botClient, callbackQuery, cancellationToken);
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.CallbackQuery != null)
            {
                Console.WriteLine($"[C] {update.CallbackQuery.Data}");
                await HandleCallbackQuery(botClient, update.CallbackQuery, cancellationToken);
            }
            else if (update.Message != null)
            {
                Console.WriteLine($"[M] {update.Message.Text}");
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
