using System.Collections.Generic;
using System.Linq;
using Automation.Core.Data;
using Automation.Core.Domain.Job;
using Automation.Core.Events;
using Automation.Core.Services.Events;
using Automation.Core.Services.Execution.Event;
using Automation.Core.Services.Queue;
using Automation.Extensions;

namespace Automation.Core.Services.Execution
{
    public class ExecutionQueueManager : IExecutionQueueManager
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IQueueManager<AutomationJobExecutionData> _testCaseExecutionDataQueue;
        private readonly IRepository<AutomationJobExecutionData> _testCaseExecutionDataRepository;

        public ExecutionQueueManager(IRepository<AutomationJobExecutionData> testCaseExecutionDataRepository,
            IQueueManager<AutomationJobExecutionData> testCaseExecutionDataQueue, IEventPublisher eventPublisher)
        {
            _testCaseExecutionDataRepository = testCaseExecutionDataRepository;
            _testCaseExecutionDataQueue = testCaseExecutionDataQueue;
            _eventPublisher = eventPublisher;
        }

        public void Reload()
        {
            var seq = _testCaseExecutionDataRepository.Table
                .Where(
                    t =>
                        !t.Deleted &&
                        !AutomationJobExecutionDataExtensions.CompleteExecutionStates.Contains(t.ExecutionState))
                .OrderBy(t => t.CreatedOnUtc);

            _testCaseExecutionDataQueue.Clear();
            seq.ForEachItem(_testCaseExecutionDataQueue.Enqueue);
        }

        public IEnumerable<AutomationJobExecutionData> Items
        {
            get { return _testCaseExecutionDataQueue.Items; }
        }

        public void Enqueue(AutomationJobExecutionData automationJobExecutionData)
        {
            Guard.NotNull(automationJobExecutionData, "AutomationJobExecutionData");
            
            _testCaseExecutionDataRepository.Insert(automationJobExecutionData);
            _eventPublisher.EntityInserted(automationJobExecutionData);

            _testCaseExecutionDataQueue.Enqueue(automationJobExecutionData);
            _eventPublisher.InsertedToQueue(automationJobExecutionData);

        }
    }
}