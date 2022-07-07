using StackExchange.Redis;

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