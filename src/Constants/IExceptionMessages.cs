using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_queue_bot.Constants
{
    public interface IExceptionMessages
    {
        public static string RedisTimeout = "" +
            "[E] Unable to send request to Redis.\n" +
            "[E] Redis server seems to be down.";
        public static string RedisConnection = "" +
            "[E] Unable to connect to Redis.\n" +
            $"[E] Make sure that \"{IEnvNames.RedisUrl}\" env variable is set and Redis server is up and running.";
    }
}
