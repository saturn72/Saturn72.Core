using Automation.Core.Infrastructure.Tasks;

namespace Automation.Core.Services.Tasks
{
    public interface IAutoAssignedScheduleTask
    {
        int Seconds { get; }
        bool StopOnError { get; }
        string Name { get; }
        ITask Task { get; }
    }
}