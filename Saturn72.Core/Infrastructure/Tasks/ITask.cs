namespace Saturn72.Core.Infrastructure.Tasks
{
    public interface ITask
    {
        int Order { get; }

        void Execute();
    }
}