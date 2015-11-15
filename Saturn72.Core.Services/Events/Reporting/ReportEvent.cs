using System.Reflection;
using Automation.Core.Events;

namespace Automation.Core.Services.Events.Reporting
{
    public class ReportEventMessage : EventBase
    {
        public MethodInfo MethodInfo { get; set; }
    }
}
