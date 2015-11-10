using System.Collections.Generic;
using Automation.Core.Domain.Tasks;

namespace Automation.Core.Services.Tasks
{
    public interface IScheduleTaskService
    {
        IEnumerable<ScheduleTask> GetAllTasks(bool showHidden = false);
        ScheduleTask GetTaskByType(string type);
        void UpdateTask(ScheduleTask task);
        void InsertTask(ScheduleTask scheduleTask);
    }
}