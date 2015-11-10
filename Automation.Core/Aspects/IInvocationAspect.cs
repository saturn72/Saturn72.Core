namespace Automation.Core.Aspects
{
    public interface IInvocationAspect:IAspect
    {
        int Order { get; }

        void Action(AspectMessage aspectMessage);
    }
}
