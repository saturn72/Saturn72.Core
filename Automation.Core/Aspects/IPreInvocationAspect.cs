namespace Automation.Core.Aspects
{
    public interface IPreInvocationAspect : IAspect
    {
        int Order { get;}
        void Action(AspectMessage aspectMessage);
    }
}