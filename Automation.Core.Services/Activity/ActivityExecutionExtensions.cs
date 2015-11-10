using Automation.Core.Activity;
using Automation.Extensions;

namespace Automation.Core.Services.Activity
{
    public static class ActivityExecutionExtensions
    {
        public static void RunActivityFlow(this IActivityExecutionService service, RequestBase request, ActivityResponse response)
        {
            Guard.NotNull(request, "request");
            Guard.NotNull(response, "response");

            service.RunRequestPreDoActions(request);
            service.RunRequestDo(request, response);
            service.ValidateRequest(request, response);
            service.RunRequestPostDoActions(request);
        }
    }
}