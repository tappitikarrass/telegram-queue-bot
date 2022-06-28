using StackExchange.Redis;
using System.Collections;

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

        public void Push(RedisValue item)
        {
            if (!this.Contains(item))
            {
                Program.Db.ListRightPush(Name, item);
            }
        }
        public void Remove(RedisValue item)
        {
            if (this.Contains(item))
            {
                Program.Db.ListRemove(Name, item);
            }
        }

        public void Clear()
        {
            Program.Db.ListTrim(Name, 1, 0);
        }

        public bool Contains(RedisValue item)
        {
            return Program.Db.ListRange(Name).Contains(item);
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
