namespace Automation.Core.Services.Events.Invocation
{
    public class InvocationEndEvent : InvocationEventBase
    {
        public override string TextMessage
        {
            get { return "End Invocation"; }
        }
    }
}
