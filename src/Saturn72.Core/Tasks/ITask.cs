namespace Saturn72.Core.Tasks
{
    public interface ITask
    {
        int Order { get; }

        void Execute();
    }
}