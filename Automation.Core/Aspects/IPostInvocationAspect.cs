namespace Automation.Core.Aspects
{
    public interface IPostInvocationAspect : IAspect
    {
        int Order { get; }

        void Action(AspectMessage aspectMessage);
    }
}
