using System;

namespace Automation.Core.Validation
{
    /// <summary>
    /// Defines validation key for Activityrequest.
    /// This is used to locate validators during runtime.
    /// The default key is the method name (e.g. "CreateContact", "DeleteContact"
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class ValidationKeyAttribute : Attribute
    {
        public string Key { get; set; }
    }
}
