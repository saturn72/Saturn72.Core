namespace Automation.Core.Infrastructure.Tasks
{
    public interface ITask
    {
        int Order { get; }

        void Execute();
    }
}