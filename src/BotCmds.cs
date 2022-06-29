using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using telegram_queue_bot.Constants;

namespace telegram_queue_bot
{
    public static class BotCmds
    {

        public delegate void Cmd(string arg);

        public static async Task RunCmdWithArgAdmin(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken, Cmd cmd)
        {
            if (Program.Bot.Admins.Contains(message.Chat.Id.ToString()))
            {
                await RunCmdWithArg(botClient, message, cancellationToken, cmd);
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    ICmdMessages.NotAllowed,
                    cancellationToken: cancellationToken);
            }
        }

        public static async Task RunCmdWithArg(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken, Cmd cmd)
        {
            var cmdWithArgs = message.Text.Split(" ");
            if (cmdWithArgs.Length == 2)
            {
                cmd(cmdWithArgs[1]);
            }
            else if (cmdWithArgs.Length < 2)
            {
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    // $"Not enough arguments. 1 expected, but {cmdWithArgs.Length - 1} is given.",
                    ICmdMessages.NotEnoughArgs.Replace(
                        "$LENGTH$", (cmdWithArgs.Length - 1).ToString()),
                    cancellationToken: cancellationToken);
            }
            else if (cmdWithArgs.Length > 2)
            {
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    // $"Too many arguments. 1 expected, but {cmdWithArgs.Length - 1} is given.",
                    ICmdMessages.TooManyArgs.Replace(
                        "$LENGTH$", (cmdWithArgs.Length - 1).ToString()),
                    cancellationToken: cancellationToken);
            }
        }

        public static async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken)
        {
            if (message.Text.StartsWith("/get_user_chat_id"))
            {
                await botClient.SendTextMessageAsync(
                    message.Chat.Id,
                    $"*user\\_chat\\_id:* `{message.Chat.Id}`",
                    parseMode: ParseMode.MarkdownV2,
                    cancellationToken: cancellationToken);
            }
            if (message.Text.StartsWith("/add_queue"))
            {
                await RunCmdWithArgAdmin(
                    botClient,
                    message,
                    cancellationToken,
                    delegate (string arg)
                    {
                        Program.Bot.AddQueue(arg);
                    });
            }
            if (message.Text.StartsWith("/remove_queue"))
            {
                await RunCmdWithArgAdmin(
                    botClient,
                    message,
                    cancellationToken,
                    delegate (string arg)
                    {
                        Program.Bot.RemoveQueue(arg);
                    });
            }
        }
    }
}
