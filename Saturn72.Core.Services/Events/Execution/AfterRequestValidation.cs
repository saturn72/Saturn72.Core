using Automation.Core.Activity;
using Automation.Core.Events;

namespace Automation.Core.Services.Events.Execution
{
    public class AfterRequestValidation:EventBase
    {
        public RequestBase Request { get; set; }
        public ActivityResponse Response { get; set; }
    }
}
