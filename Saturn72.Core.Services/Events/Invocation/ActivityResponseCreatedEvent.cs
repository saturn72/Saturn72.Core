using Automation.Core.Activity;
using Automation.Core.Events;

namespace Automation.Core.Services.Events.Invocation
{
    public class ActivityResponseCreatedEvent : EventBase
    {
        public ActivityResponse ActivityReponse { get; set; }
    }
}