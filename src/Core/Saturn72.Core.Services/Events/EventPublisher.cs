using System;
using System.Threading.Tasks;
using Saturn72.Core.Events;
using Saturn72.Core.Logging;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Events
{
    public class EventPublisher : IEventPublisher
    {
        #region ctor

        public EventPublisher(ISubscriptionService subscriptionService, ILogger logger)
        {
            _subscriptionService = subscriptionService;
            _logger = logger;
        }

        #endregion

        public bool ShouldHandleConsumersExceptions
        {
            get { return false; }
        }

        public void Publish<TEventMessage>(TEventMessage eventMessage) where TEventMessage : EventBase
        {
            _logger.Information("Publish event of type {0}".AsFormat(eventMessage.GetType()));

            var subscriptions = _subscriptionService.GetSubscriptions<TEventMessage>();

            var action = PrepareConsumerAction(eventMessage);
            Parallel.ForEach(subscriptions, s => action(s));
        }

        private Action<IConsumer<TEventMessage>> PrepareConsumerAction<TEventMessage>(TEventMessage eventMessage)
            where TEventMessage : EventBase
        {
            Action<IConsumer<TEventMessage>> coreAction = c => c.HandleEvent(eventMessage);

            if (!ShouldHandleConsumersExceptions)
                return coreAction;

            return c =>
            {
                try
                {
                    coreAction(c);
                }
                catch (Exception ex)
                {
                    //we put in to nested try-catch to prevent possible cyclic (if some error occurs)
                    try
                    {
                        _logger.Error(ex.Message, ex);
                    }
                    catch (Exception)
                    {
                        //do nothing
                    }
                }
            };
        }

        #region Fields

        private readonly ISubscriptionService _subscriptionService;
        private readonly ILogger _logger;

        #endregion
    }
}