using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using telegram_queue_bot.Constants;

namespace telegram_queue_bot.Menus
{
    public static class MainMenu
    {
        public static InlineKeyboardMarkup Keyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData(IMenuMessages.QueuesList, "MainMenu-queuesList")
            }
        });

        public static async Task Build(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var message = callbackQuery.Message;
            if (message == null) return;
            await botClient.EditMessageTextAsync(
                message.Chat.Id,
                message.MessageId,
                $"*{IMenuMessages.MainMenu}*",
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: Keyboard,
                cancellationToken: cancellationToken);
        }

        public static async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (message.Text == null) return;

            if (message.Text.StartsWith("/start"))
            {
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    $"*{IMenuMessages.MainMenu}*",
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: Keyboard,
                    cancellationToken: cancellationToken);
            }
        }

        public static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery.Data == null) return;
            switch (callbackQuery.Data)
            {
                case "MainMenu-queuesList":
                    {
                        await QueuesListMenu.Build(botClient, callbackQuery, cancellationToken);
                    }
                    break;
                default:
                    break;
            }
        }

    }
}
