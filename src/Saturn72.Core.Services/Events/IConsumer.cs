using Saturn72.Core.Events;

namespace Saturn72.Core.Services.Events
{
    public interface IConsumer<TEventMessage> where TEventMessage : EventBase
    {
        void HandleEvent(TEventMessage eventMessage);
    }
}