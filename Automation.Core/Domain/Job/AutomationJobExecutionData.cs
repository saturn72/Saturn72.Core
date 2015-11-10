using System;

namespace Automation.Core.Domain.Job
{
    public class AutomationJobExecutionData : BaseEntity
    {
        public ExecutionState ExecutionState { get; set; }
        public int TestCaseId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public DateTime? ExecutionStartedOn { get; set; }
        public DateTime? ExecutionEndedOn { get; set; }
        public bool Deleted { get; set; }
        public int ClientId { get; set; }
    }
}