using System.Collections.Generic;
using Saturn72.Core.Events;
using Saturn72.Core.Infrastructure;

namespace Saturn72.Core.Services.Events
{
    public class SubscriptionService : ISubscriptionService
    {
        public IEnumerable<IConsumer<TEventMessage>> GetSubscriptions<TEventMessage>() where TEventMessage : EventBase
        {
            return EngineContext.Current.ResolveAll<IConsumer<TEventMessage>>();
        }

    }
}