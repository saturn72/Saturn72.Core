using System.Collections.Generic;

namespace Automation.Core.Data
{
    public class QueueDataCollection<T> : IDataCollection<T>
    {
        private Queue<T> _queue;
        protected virtual Queue<T> Queue { get { return _queue ?? (_queue = new Queue<T>()); } }

        public void Insert(T item)
        {
            Queue.Enqueue(item);
        }

        public T Dequeue()
        {
            return Queue.Dequeue();
        }

        public void Clear()
        {
            Queue.Clear();
        }

        public IEnumerable<T> Items
        {
            get { return Queue.ToArray(); }
        }
    }
}