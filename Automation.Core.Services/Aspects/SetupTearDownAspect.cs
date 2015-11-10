using Automation.Core.Activity;
using Automation.Core.Aspects;
using Automation.Core.Infrastructure;
using Automation.Core.Services.Activity;
using Automation.Extensions;

namespace Automation.Core.Services.Aspects
{
    public class SetupTearDownAspect : IPreInvocationAspect, IPostInvocationAspect
    {
        int IPostInvocationAspect.Order
        {
            get { return 100; }
        }

        void IPostInvocationAspect.Action(AspectMessage aspectMessage)
        {
            RunSubRequest(aspectMessage, "TearDown");
        }

        int IPreInvocationAspect.Order
        {
            get { return 100; }
        }

        void IPreInvocationAspect.Action(AspectMessage aspectMessage)
        {
            RunSubRequest(aspectMessage, "Setup");
        }

        private static void RunSubRequest(AspectMessage aspectMessage, string subRequestName)
        {
            Guard.NotNull(new[]
            {
                aspectMessage,
                aspectMessage.Invocation,
                aspectMessage.Invocation.ReturnValue
            });

            var request = aspectMessage.Invocation.ReturnValue as RequestBase;
            if (request.IsNull()) return;

            var requestType = request.GetType();
            var subRequestInfo = requestType.GetProperty(subRequestName);
            var subRequestInstance = subRequestInfo.GetValue(request, null) as RequestBase;
            if (subRequestInstance.IsNull())
                return;

            var activityExecutionService = EngineContext.Current.Resolve<IActivityExecutionService>();
            activityExecutionService.RunRequestSubRequest(subRequestInstance, aspectMessage.Response);
        }
    }
}