using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_queue_bot
{
    public interface IMenuMessages
    {
        // Shared
        public static string Back = $"{GEmojiSharp.Emoji.Get(":arrow_left:").Raw} Back";
        // MainMenu
        public static string MainMenu = "Main menu";
        public static string QueuesList = $"{GEmojiSharp.Emoji.Get(":pencil2:").Raw} Queues list";
        // QueuesListMenu
        public static string AvailableQueues = $"{GEmojiSharp.Emoji.Get(":memo:").Raw} Available queues";
        // QueueMenu
        public static string Enlist = $"{GEmojiSharp.Emoji.Get(":white_check_mark:").Raw} Enlist";
        public static string Delist = $"{GEmojiSharp.Emoji.Get(":x:").Raw} Delist";
        public static string Queue = $"{GEmojiSharp.Emoji.Get(":page_facing_up:").Raw} Queue";
    }
}
