using Automation.Core.Activity;
using Automation.Extensions;

namespace Automation.Core.Validation
{
    public class ValidationPoint
    {
        private ActivityExecutionResult _activityExecutionResult;

        #region ctor
        
        public ValidationPoint()
        {
            ActivityExecutionResult = ActivityExecutionResult.Default;
        }

        #endregion

        public ActivityExecutionResult ActivityExecutionResult
        {
            get { return _activityExecutionResult ?? (_activityExecutionResult = ActivityExecutionResult.Default); }
            set { _activityExecutionResult = value; }
        }

        public string Message { get; set; }
        public string ReturnCodes { get; set; }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ActivityExecutionResult != null ? ActivityExecutionResult.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ReturnCodes != null ? ReturnCodes.GetHashCode() : 0);
                return hashCode;
            }
        }

        protected bool Equals(ValidationPoint other)
        {
            return Equals(ActivityExecutionResult, other.ActivityExecutionResult) &&
                   string.Equals(Message, other.Message) && string.Equals(ReturnCodes, other.ReturnCodes);
        }

        public static ValidationPoint operator +(ValidationPoint vPoint1, ValidationPoint vPoint2)
        {
            Guard.NotNull(vPoint1, "vPoint1");
            Guard.NotNull(vPoint2, "vPoint2");

            var actExeResult = vPoint1.ActivityExecutionResult.Value > vPoint2.ActivityExecutionResult.Value
                ? vPoint1.ActivityExecutionResult
                : vPoint2.ActivityExecutionResult;

            var returnCodes = vPoint1.ReturnCodes + ";" + vPoint2.ReturnCodes;
            var message = vPoint1.Message + "\n" + vPoint2.Message;
            return new ValidationPoint(actExeResult, message, returnCodes);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ValidationPoint) obj);
        }

        #region ctor

        public ValidationPoint(ActivityExecutionResult activityExecutionResult, string message)
        {
            ActivityExecutionResult = activityExecutionResult;
            Message = message;
        }

        public ValidationPoint(ActivityExecutionResult activityExecutionResult, string message, string returnCodes) :
            this(activityExecutionResult, message)
        {
            ReturnCodes = returnCodes;
        }

        #endregion ctor

        public static ValidationPoint BuildFailValidationPoint(string message, string returnCode)
        {
            return new ValidationPoint(ActivityExecutionResult.Fail, message,returnCode);
        }

        public static ValidationPoint BuildPassValidationPoint(string message, string returnCode =null)
        {
            return new ValidationPoint(ActivityExecutionResult.Pass, message, returnCode);
        }
    }
}