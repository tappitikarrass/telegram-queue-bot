using telegram_queue_bot.Constants;

namespace telegram_queue_bot
{
    public static class ExceptionHandlers
    {
        public static void RedisTimeoutHandler()
        {
            Console.WriteLine(IExceptionMessages.RedisTimeout);
            Environment.Exit(1);
        }

        public static void RedisConnectionHandler()
        {
            Console.WriteLine(IExceptionMessages.RedisConnection);
            Environment.Exit(1);
        }
    }
}
