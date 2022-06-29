using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_queue_bot
{
    public interface IRedisValueNames
    {
        public static string CurrentQueuesState = "CurrentQueuesState";
        public static string QueuesState = "QueuesState";
    }
}
