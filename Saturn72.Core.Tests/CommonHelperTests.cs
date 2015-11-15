using Saturn72.TestSdk.UnitTesting;
using Xunit;

namespace Saturn72.Core.Tests
{
    public class CommonHelperTests
    {
        [Fact]
        public void Can_get_typed_value()
        {
            CommonHelper.To<int>("1000").ShouldBe<int>();
            CommonHelper.To<int>("1000").ShouldEqual(1000);
        }

        [Fact]
        public void IsWebApp_returns_false()
        {
            Assert.False(CommonHelper.IsWebApp());
        }
    }
}