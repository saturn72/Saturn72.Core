using System.Collections.Generic;
using Automation.Core.Domain.Job;

namespace Automation.Core.Services.Execution
{
    public interface IExecutionService
    {
        void ReloadAllOpenExecutions();
        IEnumerable<AutomationJobExecutionData> GetTestCaseExecutionDataByClientId(int clientId, bool ignoreCompleted = true);
        IEnumerable<AutomationJobExecutionData> GetAllTestCaseExecutionData(bool ignoreCompleted = true);
        AutomationJobExecutionData GetTestCaseExecutionDataById(int id);
        void Execute(AutomationJobExecutionData automationJobExecutionData);
        void Execute(int testCaseId);
    }
}