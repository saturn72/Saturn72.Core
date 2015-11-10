using System;
using System.Collections.Generic;
using System.Reflection;
using Automation.Core.Aspects;
using Castle.DynamicProxy;

namespace Automation.Core
{
    public interface IRuntimeInterceptor : IInterceptor
    {
        IEnumerable<IAspect> Aspects { get; }

        IEnumerable<MethodInfo> MethodsToIntercept { get; set; }

        IEnumerable<Type> TypesToIntercept { get; set; }
    }
}
