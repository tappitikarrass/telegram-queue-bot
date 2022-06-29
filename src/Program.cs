using StackExchange.Redis;
using telegram_queue_bot.DataStructures;

namespace telegram_queue_bot
{
    public class Program
    {
        public static readonly Bot Bot = Bot.Instance;

        public static async Task Main()
        {
            await Bot.RunAsync();
        }
    }
}