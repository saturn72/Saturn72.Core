using System;
using System.Collections.Generic;
using System.Linq;
using Automation.Core.Domain.Tasks;
using Automation.Core.Infrastructure;
using Automation.Extensions;

namespace Automation.Core.Services.Tasks
{
    public class TaskManager
    {
        #region Ctor

        private TaskManager()
        {
        }

        #endregion

        #region Properties

        public static TaskManager Instance
        {
            get { return _taskManager; }
        }

        #endregion

        public void Initialize()
        {
            _taskThreads.Clear();

            InsertAllAutoAssignScheduleTask();

            var taskService = EngineContext.Current.Resolve<IScheduleTaskService>();
            var scheduleTasks = taskService.GetAllTasks(false)
                .OrderBy(x => x.Seconds);


            //group by threads with the same seconds
            foreach (var scheduleTaskGrouped in scheduleTasks.GroupBy(x => x.Seconds))
            {
                //create a thread
                var taskThread = new TaskThread
                {
                    Seconds = scheduleTaskGrouped.Key
                };

                scheduleTaskGrouped.ForEachItem(sTask => taskThread.AddTask(new Task(sTask)));
                _taskThreads.Add(taskThread);
            }

            //sometimes a task period could be set to several hours (or even days).
            //in this case a probability that it'll be run is quite small (an application could be restarted)
            //we should manually run the tasks which weren't run for a long time
            var notRunTasks = scheduleTasks
                .Where(x => x.Seconds >= _notRunTasksInterval)
                .Where(
                    x =>
                        !x.LastStartUtc.HasValue ||
                        x.LastStartUtc.Value.AddSeconds(_notRunTasksInterval) < DateTime.UtcNow);

            //create a thread for the tasks which weren't run for a long time
            if (notRunTasks.Any())
            {
                var taskThread = new TaskThread
                {
                    RunOnlyOnce = true,
                    Seconds = 60*5 //let's run such tasks in 5 minutes after application start
                };

                notRunTasks.ForEachItem(sTask => taskThread.AddTask(new Task(sTask)));
                _taskThreads.Add(taskThread);
            }
        }

        protected virtual void InsertAllAutoAssignScheduleTask()
        {
            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            var autoAssignedTasks = typeFinder.FindClassesOfType<IAutoAssignedScheduleTask>();

            var taskService = EngineContext.Current.Resolve<IScheduleTaskService>();
            var scheduleTasks = taskService.GetAllTasks(false);


            autoAssignedTasks.ForEachItem(at =>
            {
                var autoAssignedTask = Activator.CreateInstance(at) as IAutoAssignedScheduleTask;
                Guard.NotEmpty(autoAssignedTask.Name, "autoAssignedTask.Name");

                if (scheduleTasks.Any(t => t.Name.EqualsToIgnoreCases(autoAssignedTask.Name)))
                    return;
                var taskType = autoAssignedTask.Task.GetType();
                taskService.InsertTask(new ScheduleTask
                {
                    Enabled = true,
                    LastEndUtc = null,
                    LastStartUtc = null,
                    LastSuccessUtc = null,
                    Name = autoAssignedTask.Name,
                    Type = taskType.Namespace + "." + taskType.Name + ", " + taskType.Assembly.GetName().Name,
                    Seconds = autoAssignedTask.Seconds,
                    StopOnError = autoAssignedTask.StopOnError
                });
            });
        }

        public void Start()
        {
            _taskThreads.ForEachItem(t => t.InitTimer());
        }

        #region Fields

        private static readonly TaskManager _taskManager = new TaskManager();
        private readonly IList<TaskThread> _taskThreads = new List<TaskThread>();
        private readonly int _notRunTasksInterval = 30*60;

        #endregion
    }
}