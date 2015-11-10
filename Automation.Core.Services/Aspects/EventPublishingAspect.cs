using System.Linq;
using Automation.Core.Aspects;
using Automation.Core.Events;
using Automation.Core.Infrastructure;
using Automation.Core.Services.Events.Invocation;
using Automation.Extensions;
using Castle.DynamicProxy;

namespace Automation.Core.Services.Aspects
{
    public class EventPublishingAspect : IPostInvocationAspect, IPreInvocationAspect, IInvocationAspect
    {
        private IEventPublisher _eventPublisher;

        #region Properties

        protected virtual IEventPublisher EventPublisher
        {
            get { return _eventPublisher ?? (_eventPublisher = EngineContext.Current.Resolve<IEventPublisher>()); }
        }

        #endregion

        public int Order
        {
            get { return -100; }
        }

        public void Action(AspectMessage aspectMessage)
        {
            CheckAndPublishEvent<InvokingEvent>(aspectMessage);
        }

        void IPostInvocationAspect.Action(AspectMessage aspectMessage)
        {
            CheckAndPublishEvent<InvocationEndEvent>(aspectMessage);
        }

        int IPostInvocationAspect.Order
        {
            get { return -100; }
        }

        void IPreInvocationAspect.Action(AspectMessage aspectMessage)
        {
            CheckAndPublishEvent<InvocationStartEvent>(aspectMessage);
        }

        int IPreInvocationAspect.Order
        {
            get { return -100; }
        }

        protected TEvent AspectMessageToInvocationEventBase<TEvent>(AspectMessage aspectMessage)
            where TEvent : InvocationEventBase, new()
        {
            return new TEvent
            {
                InvocationId = aspectMessage.InvocationId,
                FiredOn = aspectMessage.CreatedOn,
                InvocationData = ToInvocationData(aspectMessage.Invocation)
            };
        }

        private void CheckAndPublishEvent<TEvent>(AspectMessage aspectMessage) where TEvent : InvocationEventBase, new()
        {
            Guard.NotNull(aspectMessage, "aspectMessage");
            EventPublisher.Publish(AspectMessageToInvocationEventBase<TEvent>(aspectMessage));
        }

        private InvocationData ToInvocationData(IInvocation invocation)
        {
            var parametersData = invocation.Method.GetParameters()
                .Select((t, i) => new ParameterData(t.Name, invocation.Arguments[i]));

            return new InvocationData
            {
                TargetType = invocation.TargetType,
                MethodName = invocation.Method.Name,
                ParametersData = parametersData,
                ReturnValue = invocation.ReturnValue
            };
        }
    }
}