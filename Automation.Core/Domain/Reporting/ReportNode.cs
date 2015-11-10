using System;
using Automation.Core.Aspects;

namespace Automation.Core.Domain.Reporting
{
    public class ReportNode : BaseEntity
    {
        public object InvocationContextId { get; set; }
        public string ActionName { get; set; }
        public DateTime ExecutionDateTime { get; set; }
        public string Message { get; set; }
        public InvocationData InvocationData { get; set; }
    }
}