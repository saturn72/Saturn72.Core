using System;
using System.Collections.Generic;
using System.Linq;
using Automation.Core.Data;
using Automation.Core.Domain.Job;
using Automation.Core.Events;
using Automation.Core.Services.Events;
using Automation.Extensions;

namespace Automation.Core.Services.Jobs
{
    public class AutomationJobService : IAutomationJobService
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<AutomationJob> _automationJobRepository;

        public AutomationJobService(IEventPublisher eventPublisher, IRepository<AutomationJob> automationJobRepository)
        {
            _eventPublisher = eventPublisher;
            _automationJobRepository = automationJobRepository;
        }

        public IEnumerable<AutomationJob> GetAll(bool showUnpublished = false)
        {
            return _automationJobRepository.Table.Where(x => !x.Deleted && (showUnpublished || x.Published));
        }

        public AutomationJob GetAutomationJobById(object autoJobId)
        {
            return _automationJobRepository.GetById(autoJobId);
        }

        public IEnumerable<AutomationJob> GetByGuid(Guid autoJobGuid)
        {
            return _automationJobRepository.GetBy(tc => tc.Guid == autoJobGuid && !tc.Deleted).OrderBy(t => t.Id);
        }

        public IEnumerable<AutomationJob> GetByDisplayName(string displayName, bool ignoreCases = true)
        {
            return (ignoreCases
                ? _automationJobRepository.GetBy(tc => tc.DisplayName.EqualsToIgnoreCases(displayName) && !tc.Deleted)
                : _automationJobRepository.GetBy(tc => tc.DisplayName == displayName && !tc.Deleted))
                .OrderBy(t => t.Id);
        }

        public void InsertAutomationJob(AutomationJob automationJob)
        {
            Guard.NotNull(automationJob, "AutomationJob");

            _automationJobRepository.Insert(automationJob);
            _eventPublisher.EntityInserted(automationJob);
        }

        public IEnumerable<AutomationJob> GetByName(string name, bool ignoreCases = true)
        {
            return (ignoreCases
                ? _automationJobRepository.GetBy(tc => tc.DisplayName.EqualsToIgnoreCases(name) && !tc.Deleted)
                : _automationJobRepository.GetBy(tc => tc.DisplayName.Equals(name) && !tc.Deleted))
                .OrderBy(t => t.Id);
        }

        public void UpdateAutomationJob(AutomationJob automationJob)
        {
            Guard.NotNull(automationJob, "AutomationJob");

            _automationJobRepository.Update(automationJob);
            _eventPublisher.EntityUpdated(automationJob);
        }
    }
}