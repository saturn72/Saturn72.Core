using System;
using System.Collections.Generic;
using System.Threading;
using Saturn72.Extensions;

namespace Saturn72.Core.Services.Tasks
{
    public class TaskThread : IDisposable
    {
        #region Ctor

        internal TaskThread()
        {
            _tasks = new Dictionary<string, Task>();
            Seconds = 10*60;
        }

        #endregion

        public int Interval
        {
            get { return Seconds*1000; }
        }

        public void Dispose()
        {
            if ((_timer == null) || _disposed) return;

            lock (this)
            {
                _timer.Dispose();
                _timer = null;
                _disposed = true;
            }
        }

        public void AddTask(Task task)
        {
            if (!_tasks.ContainsKey(task.Name))
                _tasks.Add(task.Name, task);
        }

        public void InitTimer()
        {
            if (_timer == null)
                _timer = new Timer(TimerHandler, null, Interval, Interval);
        }

        private void TimerHandler(object state)
        {
            _timer.Change(-1, -1);
            Run();
            if (RunOnlyOnce)
            {
                Dispose();
            }
            else
            {
                _timer.Change(Interval, Interval);
            }
        }

        private void Run()
        {
            if (Seconds <= 0)
                return;

            StartedUtc = DateTime.UtcNow;
            IsRunning = true;
            _tasks.Values.ForEachItem(t=>t.Execute());
            IsRunning = false;
        }

        public bool IsRunning { get; private set; }

        public DateTime StartedUtc { get; private set; }

        #region Prtoperties

        public int Seconds { get; set; }
        public bool RunOnlyOnce { get; set; }

        #endregion

        #region Fields

        private Timer _timer;
        private bool _disposed;
        private readonly IDictionary<string, Task> _tasks;

        #endregion
    }
}