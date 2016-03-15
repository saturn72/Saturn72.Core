using System.Reflection;
using Saturn72.Core.Events;

namespace Saturn72.Core.Services.Events.Reporting
{
    public class ReportEventMessage : EventBase
    {
        public MethodInfo MethodInfo { get; set; }
    }
}
