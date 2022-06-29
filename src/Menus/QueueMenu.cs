using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using telegram_queue_bot.DataStructures;

namespace telegram_queue_bot.Menus
{
    public static class QueueMenu
    {
        private static InlineKeyboardMarkup BuildKeyboard(CallbackQuery callbackQuery)
        {
            List<List<InlineKeyboardButton>> rows = new();

            if (callbackQuery.From != null)
            {
                Queue currentQueue;
                var user = callbackQuery.From;
                var currentQueueExitsts = Program.Bot.CurrentQueues.TryGetValue(user.Id, out currentQueue);
                var entryExists = false;

                // Check if entry is in the list
                foreach (var item in currentQueue)
                {
                    if (item.ToString().StartsWith($"{user.Id}"))
                    {
                        entryExists = true;
                        break;
                    }
                }

                if (entryExists)
                {
                    rows.Add(new()
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{IMenuMessages.Delist}",
                            "Queue-delist")
                    });
                }
                else
                {
                    rows.Add(new()
                    {
                        InlineKeyboardButton.WithCallbackData(
                            $"{IMenuMessages.Enlist}",
                            "Queue-enlist")
                    });
                }
            }

            rows.Add(new()
            {
                InlineKeyboardButton.WithCallbackData(
                        $"{IMenuMessages.Back}",
                        "Queue-back")
            });

            return new(rows);
        }

        public static async Task Build(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var message = callbackQuery.Message;
            if (message == null) return;

            var currentQueue = Program.Bot.GetCurrentQueue(callbackQuery.From);
            if (currentQueue == null)
            {
                await QueuesListMenu.Build(botClient, callbackQuery, cancellationToken);
                return;
            }

            await botClient.EditMessageTextAsync(
                message.Chat.Id,
                message.MessageId,
                $"*{IMenuMessages.Queue} {currentQueue.Name}:*\n{currentQueue}",
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: BuildKeyboard(callbackQuery),
                cancellationToken: cancellationToken);
        }

        public static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var user = callbackQuery.From;

            if (callbackQuery.Data == null) return;
            if (callbackQuery.Data.StartsWith("Queue:"))
            {
                var queueName = callbackQuery.Data.Remove(
                    callbackQuery.Data.IndexOf(ICallbackQueryConstanants.Queue),
                    ICallbackQueryConstanants.Queue.Length
                    );

                if (!Program.Bot.QueuesContains(queueName))
                {
                    await QueuesListMenu.Build(botClient, callbackQuery, cancellationToken);
                    return;
                }

                Program.Bot.SetCurrentQueue(user, new(queueName));
                await Build(botClient, callbackQuery, cancellationToken);
            }
            switch (callbackQuery.Data)
            {
                case "Queue-enlist":
                    {
                        var currentQueue = Program.Bot.GetCurrentQueue(user);
                        if (currentQueue == null)
                        {
                            await QueuesListMenu.Build(botClient, callbackQuery, cancellationToken);
                            return;
                        }

                        if (Program.Bot.QueuesContains(currentQueue.Name.ToString()))
                        {
                            currentQueue.Push($"{user.Id} {user.FirstName} {user.LastName}");
                            await Build(botClient, callbackQuery, cancellationToken);
                        }
                    }
                    break;
                case "Queue-delist":
                    {
                        var currentQueue = Program.Bot.GetCurrentQueue(user);
                        if (currentQueue == null)
                        {
                            await QueuesListMenu.Build(botClient, callbackQuery, cancellationToken);
                            return;
                        }

                        if (Program.Bot.QueuesContains(currentQueue.Name.ToString()))
                        {
                            currentQueue.Remove($"{user.Id} {user.FirstName} {user.LastName}");
                            await Build(botClient, callbackQuery, cancellationToken);
                        }
                    }
                    break;
                case "Queue-back":
                    {
                        Program.Bot.RemoveCurrentQueue(callbackQuery.From);
                        await QueuesListMenu.Build(botClient, callbackQuery, cancellationToken);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
