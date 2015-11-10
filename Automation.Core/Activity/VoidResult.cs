using System;

namespace Automation.Core.Activity
{
    /// <summary>
    /// Represents void ExecutionResult
    /// </summary>
    public sealed class VoidResult
    {
        public static VoidResult ReturnVoidResult(Action action)
        {
            action();
            return new VoidResult();
        }

        public override bool Equals(object obj)
        {
            return obj is VoidResult ;
        }
    }
}