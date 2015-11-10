using Automation.Core.Activity;
using Automation.Core.Validation;
using Automation.Extensions.UnitTesting;
using Xunit;


namespace Automation.Core.Tests.Validation
{
    public class ValidationPointTests
    {
        [Fact]
        public void AddOperator()
        {
            var vp1 = new ValidationPoint(ActivityExecutionResult.Default, "Message1", "1");
            var vp2 = new ValidationPoint(ActivityExecutionResult.Fail, "Message2", "2");

            var expected = new ValidationPoint(ActivityExecutionResult.Fail, "Message1\nMessage2", "1;2");

            var actual = (vp1 + vp2);
            actual.ShouldEqual(expected);
        }

        [Fact]
        public void Equal_Operator_ReturnsFalse()
        {
            var vp1 = new ValidationPoint(ActivityExecutionResult.Default, "Message1", "1");
            var vp2 = new ValidationPoint(ActivityExecutionResult.Fail, "Message2", "2");
            vp1.ShouldNotEqual(vp2);
        }

        [Fact]
        public void EqualsOverride_ReturnsTrue()
        {
            var vp1 = new ValidationPoint(ActivityExecutionResult.Default, "Message1", "1");
            var vp2 = new ValidationPoint(ActivityExecutionResult.Default, "Message1", "1");

            vp1.ShouldEqual(vp2);
        }

        [Fact]
        public void EqualsOverride_FalseOnNull()
        {
            var vp1 = new ValidationPoint(ActivityExecutionResult.Default, "Message1", "1");
            vp1.ShouldNotEqual(null);
        }

        [Fact]
        public void EqualsOverride_trueOnsameObject()
        {
            var vp1 = new ValidationPoint(ActivityExecutionResult.Default, "Message1", "1");
            vp1.ShouldEqual(vp1);
        }
    }
}