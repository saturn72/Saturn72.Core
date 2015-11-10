namespace Automation.Core.Services.Events.Invocation
{
    public class InvokingEvent : InvocationEventBase
    {
        public override string TextMessage
        {
            get { return "Invoking"; }
        }
    }
}