using Saturn72.TestSdk.UnitTesting;
using Xunit;

namespace Saturn72.Core.Tests
{
    public class Saturn72ExceptionTests
    {
        [Fact]
        public void Saturn72Exception_Message()
        {
            var message = "__message__";
            typeof (Saturn72Exception).ShouldBeThrownBy(() => { throw new Saturn72Exception(message); }, message);
        }

        [Fact]
        public void Saturn72Exception_MessageFormat()
        {
            var message = "A__b";
            typeof (Saturn72Exception).ShouldBeThrownBy(() => { throw new Saturn72Exception(message); }, "A__b");
        }
    }
}