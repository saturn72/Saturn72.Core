using Automation.Core.Events;

namespace Automation.Core.Services.Events
{
    public interface IConsumer<TEventMessage> where TEventMessage : EventBase
    {
        void HandleEvent(TEventMessage eventMessage);
    }
}