using System;

namespace Automation.Core.Domain.Job
{
    public class AutomationJobPlan : BaseEntity
    {
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public bool Enabled { get; set; }
        public bool Deleted { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Comment { get; set; }
        public bool IsDefault { get; set; }

        public int AutomationJobId { get; set; }
        public virtual AutomationJob AutomationJob { get; set; }
    }
}