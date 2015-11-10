using System.Collections.Generic;
using System.Linq;
using Automation.Core.Data;
using Automation.Core.Domain.Job;
using Automation.Core.Events;
using Automation.Core.Services.Events;
using Automation.Extensions;

namespace Automation.Core.Services.Jobs
{
    public class AutomationJobPlanService : IAutomationJobPlanService
    {
        #region fields
        private readonly IRepository<AutomationJobPlan> _automationJobPlanRepository;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region ctor
        public AutomationJobPlanService(IRepository<AutomationJobPlan> automationJobPlanRepository, IEventPublisher eventPublisher)
        {
            _automationJobPlanRepository = automationJobPlanRepository;
            _eventPublisher = eventPublisher;
        }

        #endregion

        public IEnumerable<AutomationJobPlan> GetAutomationJobPlansByAutomationJobId(int automationJobId, bool showDisabled = false)
        {
            var query = _automationJobPlanRepository.Table
                .Where(plan=>plan.AutomationJobId == automationJobId && !plan.Deleted);

            if(!showDisabled)
                query.Where(plan => plan.Enabled);

            return query.ToList();
        }

        public void InsertAutomationJobPlan(AutomationJobPlan automationJobPlan)
        {
            Guard.NotNull(automationJobPlan, "AutomationJobPlan");

            _automationJobPlanRepository.Insert(automationJobPlan);
            _eventPublisher.EntityInserted(automationJobPlan);
        }

        public AutomationJobPlan GetAutomationJobPlanById(int id)
        {
            return _automationJobPlanRepository.GetById(id);
            
        }

        public void UpdateAutomationJobPlan(AutomationJobPlan automationJobPlan)
        {
            Guard.NotNull(automationJobPlan, "AutomationJobPlan");

            _automationJobPlanRepository.Update(automationJobPlan);
            _eventPublisher.EntityUpdated(automationJobPlan);
        }

        public void DeleteAutomationJobPlan(int id)
        {
            var automationJobPlan = _automationJobPlanRepository.GetById(id);
            if (automationJobPlan.IsNull())
                return;

            automationJobPlan.Deleted = true;
            _automationJobPlanRepository.Update(automationJobPlan);
            _eventPublisher.EntityDeleted(automationJobPlan);
        }
    }
}