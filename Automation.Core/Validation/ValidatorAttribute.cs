using System;

namespace Automation.Core.Validation
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class ValidatorAttribute : Attribute
    {
        public string ValidationKey { get; set; }
    }
}