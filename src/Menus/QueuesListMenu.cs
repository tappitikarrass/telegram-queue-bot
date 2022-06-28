﻿using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegram_queue_bot.Menus
{
    public static class QueuesListMenu
    {
        public static InlineKeyboardMarkup Keyboard { get => BuildKeyboard(); }

        public static async Task Build(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            var message = callbackQuery.Message;
            if (message == null) return;

            await botClient.EditMessageTextAsync(
                message.Chat.Id,
                message.MessageId,
                "*Available queues*",
                parseMode: ParseMode.MarkdownV2,
                replyMarkup: Keyboard,
                cancellationToken: cancellationToken);
        }

        private static InlineKeyboardMarkup BuildKeyboard()
        {
            List<InlineKeyboardButton> buttons = new();

            //foreach (var item in Program.bot.Queues)
            //{
            //    buttons.Add(item.Name);
            //}

            buttons.Add(
                InlineKeyboardButton.WithCallbackData(
                    "Back",
                    "QueuesList-back"));

            return new(buttons);
        }

        public static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
            if (callbackQuery.Data == null) return;
            switch (callbackQuery.Data)
            {
                case "QueuesList-back":
                    {
                        await MainMenu.Build(botClient, callbackQuery, cancellationToken);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}