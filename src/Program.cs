using StackExchange.Redis;
using telegram_queue_bot.DataStructures;

namespace telegram_queue_bot
{
    public class Program
    {
        public static IDatabase? Db;
        public static ConnectionMultiplexer? Redis;

        public static void Main()
        {
            {
                ConfigurationOptions redisConfig = new()
                {
                    EndPoints = { "localhost:6379" }
                };
                Redis = ConnectionMultiplexer.Connect(redisConfig);
                Db = Redis.GetDatabase();
            }

            Queue q = new("aboba");
            q.Clear();
            q.Push("abc");
            q.Push("olegio");
            q.Push("olegio");
            q.Push("vasek1");
            q.Remove("vasek2");
            q.Remove("vasek1");
            q.Push("vasek");
            Console.WriteLine(q);
        }
    }
}