using System.Collections.Generic;

namespace Automation.Core.Data
{
    public interface IDataCollection<T>
    {
        void Insert(T item);
        T Dequeue();
        void Clear();
        IEnumerable<T> Items { get; }
    }
}