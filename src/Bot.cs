using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_queue_bot
{
    public class Bot
    {
        public static Bot Instance { get { return lazy.Value; } }
        private static readonly Lazy<Bot> lazy = new(new Bot());

        private Bot()
        {
            var env = ReadEnvironment();
        }

        private Dictionary<string, string> ReadEnvironment()
        {
            Dictionary<string, string> env = new();

            var EnvRedisUrl = "" + Environment.GetEnvironmentVariable(IEnvNames.RedisUrl);
            var EnvBotToken = "" + Environment.GetEnvironmentVariable(IEnvNames.BotToken);

            if (EnvRedisUrl == "") EnvRedisUrl = IEnvDefaults.EnvRedisUrl;

            env.Add(IEnvNames.RedisUrl, EnvRedisUrl);
            env.Add(IEnvNames.BotToken, EnvBotToken);

            return env;
        }

    }
}
