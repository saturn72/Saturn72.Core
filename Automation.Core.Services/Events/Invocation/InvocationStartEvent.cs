namespace Automation.Core.Services.Events.Invocation
{
    public class InvocationStartEvent : InvocationEventBase
    {
        public override string TextMessage
        {
            get { return "Start invocation"; }
        }
    }
}
