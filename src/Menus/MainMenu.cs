using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegram_queue_bot.Menus
{
    public static class MainMenu
    {
        public static InlineKeyboardMarkup Keyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Queues list", "mainMenu-QueuesList")
            }
        });

        public static async Task Build(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
        }

        public static async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (message.Text == null) return;

            if (message.Text.StartsWith("/start"))
            {
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    "*Main menu*",
                    parseMode: ParseMode.MarkdownV2,
                    replyMarkup: Keyboard,
                    cancellationToken: cancellationToken);
            }
        }

        public static async Task HandleCallbackQueryAsync(ITelegramBotClient botClient, CallbackQuery callbackQuery, CancellationToken cancellationToken)
        {
        }

    }
}
