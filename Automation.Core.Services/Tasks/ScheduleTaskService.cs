using System;
using System.Collections.Generic;
using System.Linq;
using Automation.Core.Data;
using Automation.Core.Domain.Tasks;
using Automation.Core.Events;
using Automation.Core.Services.Events;
using Automation.Extensions;

namespace Automation.Core.Services.Tasks
{
    public class ScheduleTaskService : IScheduleTaskService
    {
        private readonly IRepository<ScheduleTask> _taskRepository;
        private readonly IEventPublisher _eventPublisher;

        public ScheduleTaskService(IRepository<ScheduleTask> taskRepository, IEventPublisher eventPublisher)
        {
            _taskRepository = taskRepository;
            _eventPublisher = eventPublisher;
        }

        public virtual IEnumerable<ScheduleTask> GetAllTasks(bool showHidden = false)
        {
            var query = _taskRepository.Table;
            if (!showHidden)
            {
                query = query.Where(t => t.Enabled);
            }
            query = query.OrderByDescending(t => t.Seconds);

            var tasks = query.ToList();
            return tasks;
        }

        public ScheduleTask GetTaskByType(string type)
        {
            if (type.IsEmpty())
                return null;

            var query = _taskRepository.Table;
            query = query.Where(st => st.Type == type);
            query = query.OrderByDescending(t => t.Id);

            var task = query.FirstOrDefault();
            return task;
        }

        public void UpdateTask(ScheduleTask task)
        {
            Guard.NotNull(task, "task");

            SetMinimumSqlDateTimeValue(task);
            _taskRepository.Update(task);
        }

        public void InsertTask(ScheduleTask scheduleTask)
        {
            Guard.NotNull(scheduleTask, "scheduleTask");
            _taskRepository.Insert(scheduleTask);
            _eventPublisher.EntityInserted(scheduleTask);
        }

        private static void SetMinimumSqlDateTimeValue(ScheduleTask task)
        {
            var minDateTime = DateTime.Parse("1753-01-01");

            if (task.LastEndUtc == DateTime.MinValue)
                task.LastEndUtc = minDateTime;

            if (task.LastStartUtc == DateTime.MinValue)
                task.LastEndUtc = minDateTime;

            if (task.LastSuccessUtc == DateTime.MinValue)
                task.LastSuccessUtc = minDateTime;
        }
    }
}