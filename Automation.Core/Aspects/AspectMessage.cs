using System;
using Automation.Core.Activity;
using Castle.DynamicProxy;

namespace Automation.Core.Aspects
{
    public sealed class AspectMessage
    {
        public object InvocationId { get; set; }
        public DateTime CreatedOn { get; set; }
        public IInvocation Invocation { get; set; }
        public DateTime InvocationStartOn { get; set; }
        public DateTime InvocationEndOn { get; set; }
        public ActivityResponse Response { get; set; }
        public RequestBase Request { get; set; }
    }
}