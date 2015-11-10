using System;
using Automation.Core.Events;
using Automation.Core.Services.Events;

namespace Automation.Core.Services.Execution.Event
{
    public static class EventPublisherExtensions
    {
        public static void InsertedToQueue<T>(this IEventPublisher eventPublisher,T item)
        {
            eventPublisher.Publish(new InsertedToQueue<T>
            {
                Item = item,
                FiredOn = DateTime.Now,
            });
        }
    }
}
