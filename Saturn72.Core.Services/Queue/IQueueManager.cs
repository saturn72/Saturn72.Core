using System.Collections.Generic;

namespace Automation.Core.Services.Queue
{
    public interface IQueueManager<TEntity> where TEntity : BaseEntity
    {
        void Enqueue(TEntity tEntity);

        IEnumerable<TEntity> Items { get; }
        void Clear();
    }
}