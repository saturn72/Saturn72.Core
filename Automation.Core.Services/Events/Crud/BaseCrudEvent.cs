using Automation.Core.Events;

namespace Automation.Core.Services.Events
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