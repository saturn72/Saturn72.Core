using Automation.Core.Aspects;
using Automation.Core.Events;

namespace Automation.Core.Services.Events.Invocation
{
    public abstract class InvocationEventBase : EventBase
    {
        public InvocationData InvocationData { get; set; }
        public abstract string TextMessage { get; }
    }
}