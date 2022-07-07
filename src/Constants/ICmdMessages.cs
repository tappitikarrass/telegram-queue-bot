using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_queue_bot.Constants
{
    public interface ICmdMessages
    {
        public static string NotEnoughArgs = "Not enough arguments. 1 expected, but $LENGTH$ is given.";
        public static string TooManyArgs = "Too many arguments. 1 expected, but $LENGTH$ is given.";
        public static string NotAllowed = "You are not allowed to run this command.";
    }
}
