using Saturn72.Core.Events;

namespace Saturn72.Core.Services.Events
{
    public class BaseCrudEvent<T>:EventBase where T : BaseEntity
    {
        protected BaseCrudEvent(T entity)
        {
            Entity = entity;
        }
        public T Entity { get; private set; }
    }
}