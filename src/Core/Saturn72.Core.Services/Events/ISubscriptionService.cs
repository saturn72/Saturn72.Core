using System.Collections.Generic;
using Saturn72.Core.Events;

namespace Saturn72.Core.Services.Events
{
    public interface ISubscriptionService
    {
        IEnumerable<IConsumer<TEventMessage>> GetSubscriptions<TEventMessage>() where TEventMessage : EventBase;
    }
}