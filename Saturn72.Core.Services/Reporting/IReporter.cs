using Automation.Core.Services.Events;
using Automation.Core.Services.Events.Invocation;

namespace Automation.Core.Services.Reporting
{
    public interface IReporter : IConsumer<InvocationStartEvent>, IConsumer<InvokingEvent>,
        IConsumer<InvocationEndEvent>
    {
    }
}