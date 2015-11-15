using Automation.Core.Activity;

namespace Automation.Core.Services.Activity
{
    public interface IActivityExecutionService
    {
        void RunRequestPostDoActions(RequestBase request);
        void RunRequestPreDoActions(RequestBase request);
        void RunRequestDo(RequestBase request, ActivityResponse response);
        void ValidateRequest(RequestBase request, ActivityResponse response);
        void RunRequestSubRequest(RequestBase subRequest, ActivityResponse response);
    }
}