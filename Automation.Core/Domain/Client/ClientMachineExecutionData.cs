using System.Collections.Generic;
using Automation.Core.Domain.Job;

namespace Automation.Core.Domain.Client
{
    public class ClientMachineExecutionData : BaseEntity
    {
        private IEnumerable<AutomationJobExecutionData> _testCaseExecutionDatas;
        public int ClientMachineId { get; set; }
        public virtual ClientMachine ClientMachine { get; set; }

        public IEnumerable<AutomationJobExecutionData> TestCaseExecutionDatas
        {
            get { return _testCaseExecutionDatas ?? (_testCaseExecutionDatas = new List<AutomationJobExecutionData>()); }
            set { _testCaseExecutionDatas = value; }
        }
    }
}