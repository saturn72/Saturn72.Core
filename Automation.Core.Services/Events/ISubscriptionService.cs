using System.Collections.Generic;
using Automation.Core.Events;

namespace Automation.Core.Services.Events
{
    public interface ISubscriptionService
    {
        IEnumerable<IConsumer<TEventMessage>> GetSubscriptions<TEventMessage>() where TEventMessage : EventBase;
    }
}