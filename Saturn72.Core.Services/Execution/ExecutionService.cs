using System;
using System.Collections.Generic;
using System.Linq;
using Automation.Core.Data;
using Automation.Core.Domain.Job;
using Automation.Core.Domain.Logging;
using Automation.Core.Events;
using Automation.Core.Services.Jobs;
using Automation.Core.Services.Localization;
using Automation.Extensions;

namespace Automation.Core.Services.Execution
{
    public class ExecutionService : IExecutionService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IExecutionQueueManager _executionQueueManager;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IRepository<AutomationJobExecutionData> _testCaseExecutionDataRepository;
        private readonly IAutomationJobService _testCaseService;

        public ExecutionService(IAutomationJobService testCaseService, ILocalizationService localizationService,
            IExecutionQueueManager executionQueueManager, IEventPublisher eventPublisher, ILogger logger,
            IRepository<AutomationJobExecutionData> testCaseExecutionDataRepository)
        {
            _testCaseService = testCaseService;
            _localizationService = localizationService;
            _executionQueueManager = executionQueueManager;
            _eventPublisher = eventPublisher;
            _logger = logger;
            _testCaseExecutionDataRepository = testCaseExecutionDataRepository;
        }

        public void Execute(AutomationJobExecutionData automationJobExecutionData)
        {
            Guard.NotNull(automationJobExecutionData, "AutomationJobExecutionData");
            if(automationJobExecutionData.Deleted)
                _logger.Warning("Request to run AutomationJob with ID {0} was declined - The AutomationJob does not exists".AsFormat(automationJobExecutionData.TestCaseId));
            _executionQueueManager.Enqueue(automationJobExecutionData);

            _logger.Information(
                "AutomationJob was inserted to execution queue. AutomationJob detais: {0}".AsFormat(
                    automationJobExecutionData.ToString()));
            //TODO:log user activity
        }

        public void Execute(int testCaseId)
        {
            var testCase = _testCaseService.GetAutomationJobById(testCaseId);
            CheckThatTestCaseCanBeExecuted(testCase);

            Execute(ToTestCaseExecutionData(testCase));
        }

        public void ReloadAllOpenExecutions()
        {
            _executionQueueManager.Reload();
        }

        public IEnumerable<AutomationJobExecutionData> GetAllTestCaseExecutionData(bool ignoreCompleted = true)
        {
            return _testCaseExecutionDataRepository.Table
                .Where(ts => !ts.Deleted &&
                             (!AutomationJobExecutionDataExtensions.CompleteExecutionStates.Contains(ts.ExecutionState) ||
                              !ignoreCompleted));
        }

        public AutomationJobExecutionData GetTestCaseExecutionDataById(int id)
        {
            return _testCaseExecutionDataRepository.Table.FirstOrDefault(ts => !ts.Deleted);
        }

        public IEnumerable<AutomationJobExecutionData> GetTestCaseExecutionDataByClientId(int clientId,
            bool ignoreCompleted = true)
        {
            return GetAllTestCaseExecutionData(ignoreCompleted)
                .Where(ts => ts.ClientId == clientId && !ts.Deleted && (!ts.IsCompleted() || !ignoreCompleted))
                .OrderBy(tc => tc.CreatedOnUtc);
        }

        protected void CheckThatTestCaseCanBeExecuted(AutomationJob automationJob)
        {
            if (automationJob.IsNull() || automationJob.Deleted)
                throw new Saturn72Exception(_localizationService.GetResource("Automation.TestCaseNotExists"));

            if (!automationJob.Published || !automationJob.Enabled)
                throw new Saturn72Exception(_localizationService.GetResource("Automation.TestCaseDisabled"));
        }

        protected virtual AutomationJobExecutionData ToTestCaseExecutionData(AutomationJob automationJob)
        {
            Guard.NotNull(automationJob, "AutomationJob");

            return new AutomationJobExecutionData
            {
                ExecutionState = ExecutionState.Pending,
                TestCaseId = automationJob.Id,
                CreatedOnUtc = DateTime.UtcNow,
                UpdatedOnUtc = DateTime.UtcNow
            };
        }
    }
}