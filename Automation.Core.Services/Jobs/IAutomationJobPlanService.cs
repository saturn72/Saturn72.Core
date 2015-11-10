using System.Collections.Generic;
using Automation.Core.Domain.Job;

namespace Automation.Core.Services.Jobs
{
    public interface IAutomationJobPlanService
    {
        IEnumerable<AutomationJobPlan> GetAutomationJobPlansByAutomationJobId(int automationJobId, bool showDisabled = false);
        void InsertAutomationJobPlan(AutomationJobPlan automationJobPlan);
        AutomationJobPlan GetAutomationJobPlanById(int id);
        void UpdateAutomationJobPlan(AutomationJobPlan automationJobPlan);
        void DeleteAutomationJobPlan(int id);
    }
}
