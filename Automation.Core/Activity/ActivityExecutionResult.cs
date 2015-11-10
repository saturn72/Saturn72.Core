using Automation.Extensions;

namespace Automation.Core.Activity
{
    public class ActivityExecutionResult
    {
        public static ActivityExecutionResult Default = new ActivityExecutionResult("Default", 0);
        public static ActivityExecutionResult Pass = new ActivityExecutionResult("Pass", 10);
        public static ActivityExecutionResult Warning = new ActivityExecutionResult("Warning", 20);
        public static ActivityExecutionResult Fail = new ActivityExecutionResult("Fail", 50);

        #region ctor

        private ActivityExecutionResult(string name, int value)
        {
            _name = name;
            _value = value;
        }

        #endregion

        #region Fields

        private readonly string _name;
        private readonly int _value;

        #endregion

        #region Properties

        public string Name
        {
            get { return _name; }
        }

        public int Value
        {
            get { return _value; }
        }

        #endregion
    }

    public static class ActivityExecutionResultExtensions
    {
        public static bool ToBooleanResult(this ActivityExecutionResult executionResult)
        {
            return executionResult.NotNull() && executionResult == ActivityExecutionResult.Pass;
        }
    }
}