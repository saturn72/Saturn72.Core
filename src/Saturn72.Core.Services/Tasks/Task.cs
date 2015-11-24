using System;
using Autofac;
using Saturn72.Core.Domain.Tasks;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.Tasks;
using Saturn72.Core.Logging;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Tasks
{
    public class Task
    {
        public Task(ScheduleTask scheduleTask)
        {
            Type = scheduleTask.Type;
            Enabled = scheduleTask.Enabled;
            StopOnError = scheduleTask.StopOnError;
            Name = scheduleTask.Name;
        }

        #region Properties

        public string Name { get; set; }

        public bool StopOnError { get;private  set; }

        public bool Enabled { get; set; }

        public string Type { get; set; }

        #endregion

        public void Execute(bool throwException = false, bool dispose = true)
        {
            this.IsRunning = true;

            //background tasks has an issue with Autofac
            //because scope is generated each time it's requested
            //that's why we get one single scope here
            //this way we can also dispose resources once a task is completed
            var scope = EngineContext.Current.ContainerManager.Scope();
            var scheduleTaskService = EngineContext.Current.ContainerManager.Resolve<IScheduleTaskService>();
            var scheduleTask = scheduleTaskService.GetTaskByType(this.Type);

            try
            {
                var task = this.CreateTask(scope);
                if (task != null)
                {
                    this.LastStartUtc = DateTime.UtcNow;
                    if (scheduleTask != null)
                    {
                        //update appropriate datetime properties
                        scheduleTask.LastStartUtc = this.LastStartUtc;
                        scheduleTaskService.UpdateTask(scheduleTask);
                    }

                    //execute task
                    task.Execute();
                    this.LastEndUtc = this.LastSuccessUtc = DateTime.UtcNow;
                }
            }
            catch (Exception exc)
            {
                this.Enabled = !this.StopOnError;
                this.LastEndUtc = DateTime.UtcNow;

                //log error
                var logger = EngineContext.Current.ContainerManager.Resolve<ILogger>();
                logger.Error(string.Format("Error while running the '{0}' schedule task. {1}", this.Name, exc.Message), exc);
                if (throwException)
                    throw;
            }

            if (scheduleTask != null)
            {
                //update appropriate datetime properties
                scheduleTask.LastEndUtc = this.LastEndUtc;
                scheduleTask.LastSuccessUtc = this.LastSuccessUtc;
                scheduleTaskService.UpdateTask(scheduleTask);
            }

            //dispose all resources
            if (dispose)
            {
                scope.Dispose();
            }

            this.IsRunning = false;
        }

        private ITask CreateTask(ILifetimeScope scope)
        {
            ITask task = null;
            if (Enabled)
            {
                var type2 = System.Type.GetType(Type);
                if (type2.NotNull())
                {
                    object instance;
                    if (!EngineContext.Current.ContainerManager.TryResolve(type2, scope, out instance))
                        instance = EngineContext.Current.ContainerManager.ResolveUnregistered(type2, scope);

                    task = instance as ITask;
                }
            }

            return task;
        }

        public DateTime LastStartUtc { get; private set; }

        public bool IsRunning { get; private set; }

        public DateTime LastSuccessUtc { get; private set; }

        public DateTime LastEndUtc { get; private set; }
    }
}