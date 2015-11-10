using System;
using System.Collections.Generic;
using Automation.Core.Domain.Job;

namespace Automation.Core.Services.Jobs
{
    public interface IAutomationJobService
    {
        IEnumerable<AutomationJob> GetAll(bool showUnpublished = false);
        AutomationJob GetAutomationJobById(object autoJobId);
        IEnumerable<AutomationJob> GetByGuid(Guid autoJobGuid);
        IEnumerable<AutomationJob> GetByDisplayName(string displayName, bool ignoreCases = true);
        void InsertAutomationJob(AutomationJob automationJob);
        IEnumerable<AutomationJob> GetByName(string name, bool ignoreCases = true);
        void UpdateAutomationJob(AutomationJob automationJob);
    }
}