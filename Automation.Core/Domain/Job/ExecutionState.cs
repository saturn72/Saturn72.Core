namespace Automation.Core.Domain.Job
{
    public enum ExecutionState
    {
        Pending = 0,
        Pulled = 10,
        Executing = 20,
        Paused = 30,
        Canceled = 40,
        Completed = 50
    }
}