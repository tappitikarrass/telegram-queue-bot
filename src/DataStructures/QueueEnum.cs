using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace telegram_queue_bot.DataStructures
{
    public class QueueEnum : IEnumerator<RedisValue>
    {
        public RedisValue Current => _value;

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        private int _position;
        public Queue _queue;
        private RedisValue _value;

        public QueueEnum()
        {
        }

        public QueueEnum(Queue queue)
        {
            _queue = queue;
            _position = -1;
            _value = default;
        }

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            if (++_position >= _queue.Length)
            {
                return false;
            }
            else
            {
                _value = _queue[_position];
            }
            return true;
        }

        public void Reset()
        {
            _position = -1;
        }
    }
}
