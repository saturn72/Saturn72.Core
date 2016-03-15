using Saturn72.Core.Tasks;

namespace Saturn72.Core.Services.Tasks
{
    public interface IAutoAssignedScheduleTask
    {
        int Seconds { get; }
        bool StopOnError { get; }
        string Name { get; }
        ITask Task { get; }
    }
}