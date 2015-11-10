using System;
using System.Collections.Generic;

namespace Automation.Core.Domain.Job
{
    public class AutomationJob : BaseEntity
    {
        private ICollection<AutomationJobPlan> _automationJobPlans;
        private ICollection<Platform> _supportedPlatforms;
        private ICollection<Platform> _unsupportedPlatforms;

        public string Name { get; set; }
        public bool Deleted { get; set; }
        public string DisplayName { get; set; }
        public string Comment { get; set; }
        public string Description { get; set; }
        public bool Published { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdatedOnUtc { get; set; }
        public Guid Guid { get; set; }

        public virtual ICollection<AutomationJobPlan> AutomationJobPlans
        {
            get { return _automationJobPlans ??(_automationJobPlans = new List<AutomationJobPlan>()); }
            set  { _automationJobPlans = value; }
        }

        public virtual ICollection<Platform> SupportedPlatforms
        {
            get { return _supportedPlatforms ??(_supportedPlatforms = new List<Platform>()); }
            set { _supportedPlatforms = value; }
        }

        public virtual ICollection<Platform> UnsupportedPlatforms
        {
            get { return _unsupportedPlatforms ?? (_unsupportedPlatforms = new List<Platform>()); }
            set { _unsupportedPlatforms = value; }
        }
    }
}