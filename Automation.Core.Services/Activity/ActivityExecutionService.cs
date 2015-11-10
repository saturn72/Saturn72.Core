using System;
using System.Diagnostics;
using Automation.Core.Activity;
using Automation.Core.Events;
using Automation.Core.Services.Events.Execution;
using Automation.Extensions;

namespace Automation.Core.Services.Activity
{
    public class ActivityExecutionService : IActivityExecutionService
    {
        private readonly IEventPublisher _eventPublisher;

        public ActivityExecutionService(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public virtual void RunRequestPostDoActions(RequestBase request)
        {
            Guard.NotNull(request, "request");
            request.PostDoActions.RunAll();
        }

        public virtual void RunRequestPreDoActions(RequestBase request)
        {
            Guard.NotNull(request, "request");
            _eventPublisher.Publish(new BeforeRequestPreDoActionsEvent {Request = request});
            request.PreDoActions.RunAll();
            _eventPublisher.Publish(new AfterRequestPreDoActionsEvent {Request = request});
        }

        public void RunRequestDo(RequestBase request, ActivityResponse response)
        {
            Guard.NotNull(request, "request");
            Guard.NotNull(response, "response");

            response.Request = request;
            response.StartTime = DateTime.Now;
            var stopwatch = new Stopwatch();

            _eventPublisher.Publish(new BeforeRequestDoEvent {Request = request});

            stopwatch.Start();
            response.ActualValue = request.Do();
            stopwatch.Stop();

            response.EndTime = DateTime.Now;
            response.ElapsedTime = stopwatch.Elapsed;
            _eventPublisher.Publish(new AfterRequestDoEvent {Request = request, Response = response});
        }

        public void ValidateRequest(RequestBase request, ActivityResponse response)
        {
            Guard.NotNull(request, () => { throw new ArgumentNullException("request"); });
            Guard.NotNull(response, () => { throw new ArgumentNullException("response"); });

            _eventPublisher.Publish(new BeforeRequestValidation {Request = request, Response = response});
            response.ValidationResult = request.RunRequestValidationPoints();
            _eventPublisher.Publish(new AfterRequestValidation {Request = request, Response = response});
        }

        public virtual void RunRequestSubRequest(RequestBase subRequest, ActivityResponse response)
        {
            if (subRequest.IsNull())
                return;
            var subResponse = new ActivityResponse
            {
                Request = subRequest,
                ParentResponse = response
            };
            response.SubResponses.Add(subResponse);

            RunRequestDo(subRequest, subResponse);
            ValidateRequest(subRequest, subResponse);
        }
    }
}