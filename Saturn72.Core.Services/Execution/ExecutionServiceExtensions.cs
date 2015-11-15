using System.Collections.Generic;
using Automation.Extensions;

namespace Automation.Core.Services.Execution
{
    public static class ExecutionServiceExtensions
    {
        public static void Run(this ITestCaseExecutionDataService executionService, ICollection<int> selectedIds)
        {
            selectedIds.ForEachItem(executionService.Execute);
        }
    }
}