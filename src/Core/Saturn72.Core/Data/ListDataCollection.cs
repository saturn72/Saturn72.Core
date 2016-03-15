using System.Collections.Generic;
using System.Linq;
using Saturn72.Extensions;

namespace Saturn72.Core.Data
{
    public class ListDataCollection<T> : IDataCollection<T>
    {
        private IList<T> _list;

        protected virtual IList<T> List
        {
            get { return _list ?? (_list = new List<T>()); }
        }

        public virtual void Insert(T item)
        {
            List.Add(item);
        }

        public virtual T Dequeue()
        {
            var result = List.FirstOrDefault();

            if (!List.IsEmpty())
                List.RemoveAt(List.Count - 1);
            
            return result;
        }

        public void Clear()
        {
            List.Clear();
        }

        public IEnumerable<T> Items {
            get { return _list; }
        }
    }
}