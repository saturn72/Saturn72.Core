using System.Collections.Generic;
using Saturn72.Core.Domain.Tasks;

namespace Saturn72.Core.Services.Tasks
{
    public interface IScheduleTaskService
    {
        IEnumerable<ScheduleTask> GetAllTasks(bool showHidden = false);
        ScheduleTask GetTaskByType(string type);
        void UpdateTask(ScheduleTask task);
        void InsertTask(ScheduleTask scheduleTask);
    }
}