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
                return Program.Db.ListLength(Name);
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
                return Program.Db.ListGetByIndex(Name, index);
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
                Program.Db.ListRightPush(Name, $"{user.Id} {user.FirstName} {user.LastName}");
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
                    Program.Db.ListRemove(Name, item);
                }
            }
        }

        public void Clear()
        {
            Program.Db.ListTrim(Name, 1, 0);
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
            string ans = "";

            foreach (var item in this)
            {
                ans += $"{item}\n";
            }
            return ans;
        }

        public bool Equals(Queue? other)
        {
            return other.Name == this.Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Queue);
        }
    }
}
