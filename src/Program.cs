using StackExchange.Redis;
using telegram_queue_bot.DataStructures;

namespace telegram_queue_bot
{
    public class Program
    {
        public static IDatabase? Db;
        public static ConnectionMultiplexer? Redis;
        public static Bot? Bot;

        public static async Task Main()
        {
            {
                ConfigurationOptions redisConfig = new()
                {
                    EndPoints = { "localhost:6379" }
                };
                Redis = ConnectionMultiplexer.Connect(redisConfig);
                Db = Redis.GetDatabase();
            }

            Bot = Bot.Instance;
            await Bot.RunAsync();
        }
    }
}