using System;
using Automation.Core.Activity;
using Automation.Core.Aspects;
using Automation.Core.Events;
using Automation.Core.Infrastructure;
using Automation.Core.Services.Events;
using Automation.Core.Services.Events.Invocation;
using Automation.Extensions;

namespace Automation.Core.Services.Activity
{
    /// <summary>
    ///     Main exec
    /// </summary>
    public class ActivityExecutionAspect : IInvocationAspect
    {
        private readonly IActivityExecutionService _activityExecutionService =
            EngineContext.Current.Resolve<IActivityExecutionService>();

        private readonly IEventPublisher _eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

        public int Order
        {
            get { return 10; }
        }

        public void Action(AspectMessage aspectMessage)
        {
            Guard.NotNull(new object[]{aspectMessage, aspectMessage.Request, aspectMessage.Request.Do});

            var request = aspectMessage.Request;
            SetRequestExpectedValueWhenShouldBeVoid(request);

            var response = aspectMessage.Response;
            _activityExecutionService.RunActivityFlow(request, response);

            _eventPublisher.Publish(new ActivityResponseCreatedEvent
            {
                FiredOn = DateTime.Now,
                ActivityReponse = response
            });
        }

        private void SetRequestExpectedValueWhenShouldBeVoid(RequestBase request)
        {
            if (request.ReturnType == typeof (VoidResult))
                request.ExpectedValue = new VoidResult();
        }
    }
}