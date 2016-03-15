using System.Collections.Generic;
using Saturn72.Core.Data;

namespace Saturn72.Core.Services.Queue
{
    public class QueueManager<TEntity> : IQueueManager<TEntity> where TEntity : BaseEntity
    {
        private readonly IDataCollection<TEntity> _dataCollection;

        public QueueManager(IDataCollection<TEntity> dataCollection)
        {
            _dataCollection = dataCollection;
        }

        public virtual void Enqueue(TEntity tEntity)
        {
            _dataCollection.Insert(tEntity);
        }

        public virtual IEnumerable<TEntity> Items
        {
            get { return _dataCollection.Items; }
        }

        public void Clear()
        {
            _dataCollection.Clear();
        }
    }
}