using System.Linq;

namespace Automation.Core.Domain.Job
{
    public static class AutomationJobExecutionDataExtensions
    {
        public static readonly ExecutionState[] CompleteExecutionStates = { ExecutionState.Canceled, ExecutionState.Completed };
        
        public static bool IsCompleted(this AutomationJobExecutionData automationJobExecutionData)
        {
            return CompleteExecutionStates.Contains(automationJobExecutionData.ExecutionState);
        }
    }
}