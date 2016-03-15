namespace Saturn72.Core.Events
{
    /// <summary>
    /// Represents event publisher in Pub/Sub pattern
    /// </summary>
    public interface IEventPublisher
    {
        /// <summary>
        /// Publishes and event message
        /// </summary>
        /// <typeparam name="TEventMessage">Event TextMessage type</typeparam>
        /// <param name="eventMessage">Event TextMessage type</param>
        void Publish<TEventMessage>(TEventMessage eventMessage) where TEventMessage : EventBase;

        bool ShouldHandleConsumersExceptions { get; }
    }
}