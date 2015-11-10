using System.Collections.Generic;
using Automation.Core.Domain.Job;

namespace Automation.Core.Services.Execution
{
    public interface IExecutionQueueManager
    {
        void Reload();

        IEnumerable<AutomationJobExecutionData> Items { get; }

        void Enqueue(AutomationJobExecutionData automationJobExecutionData);
    }
}