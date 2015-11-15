using Xunit;

namespace Saturn72.Core.Tests
{
    public class CommonHelperTests
    {
        [Fact]
        [Trait("Category", "unit_test")]
        public void Can_get_typed_value()
        {
            CommonHelper.To<int>("1000").ShouldBe<int>();
            CommonHelper.To<int>("1000").ShouldEqual(1000);
        }

        [Fact]
        [Trait("Category", "unit_test")]
        public void IsWebApp_returns_false()
        {
            Assert.False(CommonHelper.IsWebApp());
        }
    }
}