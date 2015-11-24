using System.Collections.Generic;

namespace Saturn72.Core.Services.Queue
{
    public interface IQueueManager<TEntity> where TEntity : BaseEntity
    {
        void Enqueue(TEntity tEntity);

        IEnumerable<TEntity> Items { get; }
        void Clear();
    }
}