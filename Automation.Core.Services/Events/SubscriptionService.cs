using System.Collections.Generic;
using Automation.Core.Events;
using Automation.Core.Infrastructure;

namespace Automation.Core.Services.Events
{
    public class SubscriptionService : ISubscriptionService
    {
        public IEnumerable<IConsumer<TEventMessage>> GetSubscriptions<TEventMessage>() where TEventMessage : EventBase
        {
            return EngineContext.Current.ResolveAll<IConsumer<TEventMessage>>();
        }

    }
}