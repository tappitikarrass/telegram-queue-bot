using StackExchange.Redis;
using System.Collections;
using Telegram.Bot.Types;

namespace telegram_queue_bot.DataStructures
{
    public class Queue : IEnumerable<RedisValue>, IEquatable<Queue>
    {
        public RedisKey Name { get; private set; }
        public long Length
        {
            get
            {
                long length = 0;
                try
                {
                    length = Program.Bot.Db.ListLength(Name);
                }
                catch (RedisTimeoutException)
                {
                    ExceptionHandlers.RedisTimeoutHandler();
                }
                return length;
            }
        }

        public RedisValue this[int index]
        {
            get
            {
                if (index < 0 || index > Length)
                {
                    throw new IndexOutOfRangeException();
                }

                RedisValue value = new();

                try
                {
                    value = Program.Bot.Db.ListGetByIndex(Name, index);
                }
                catch (RedisTimeoutException)
                {
                    ExceptionHandlers.RedisTimeoutHandler();
                }
                return value;
            }
        }

        public Queue()
        {
        }

        public Queue(String name)
        {
            Name = name;
        }

        public void Push(User user)
        {
            if (!this.Contains(user))
            {
                try
                {
                    Program.Bot.Db.ListRightPush(Name, $"{user.Id} {user.FirstName} {user.LastName}");
                }
                catch (RedisTimeoutException)
                {
                    ExceptionHandlers.RedisTimeoutHandler();
                }
            }
        }
        public void Remove(User user)
        {
            if (!this.Contains(user)) return;

            var currentQueue = Program.Bot.GetCurrentQueue(user);
            if (currentQueue == null) return;

            foreach (var item in currentQueue)
            {
                if (item.StartsWith($"{user.Id}"))
                {
                    try
                    {
                        Program.Bot.Db.ListRemove(Name, item);
                    }
                    catch (RedisTimeoutException)
                    {
                        ExceptionHandlers.RedisTimeoutHandler();
                    }
                }
            }
        }

        public void Clear()
        {
            try
            {
                Program.Bot.Db.ListTrim(Name, 1, 0);
            }
            catch (RedisTimeoutException)
            {
                ExceptionHandlers.RedisTimeoutHandler();
            }
        }

        public bool Contains(User user)
        {
            foreach (var entry in this)
            {
                if (entry.ToString().StartsWith(user.Id.ToString()))
                {
                    return true;
                }
            }
            return false;
        }
        public IEnumerator<RedisValue> GetEnumerator()
        {
            return new QueueEnum(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            string ans = "```\n";
            int counter = 0;
            foreach (var item in this)
            {
                var itemSplit = item.ToString().Split(" ");
                ans += $"{++counter,2}. {itemSplit[1]} {itemSplit[2]}\n";
            }
            return $"{ans}\n```";
        }

        public bool Equals(Queue? other)
        {
            if (other == null) return false;
            return other.Name == this.Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Queue);
        }
    }
}
