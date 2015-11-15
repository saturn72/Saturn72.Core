using Xunit;

namespace Saturn72.Core.Tests
{
    public class AutomationExceptionTests
    {
        [Fact]
        public void AutomationException_Message()
        {
            var message = "__message__";
            typeof(Saturn72Exception).ShouldBeThrownBy(() => { throw new Saturn72Exception(message); }, message);
        }

        [Fact]
        public void AutomationException_MessageFormat()
        {
            var message = "A__b";
            typeof(Saturn72Exception).ShouldBeThrownBy(() => { throw new Saturn72Exception(message); }, "A__b");
        }
    }
}