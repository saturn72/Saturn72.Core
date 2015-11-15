using System;
using Saturn72.Core.Plugins;
using Xunit;

namespace Saturn72.Core.Tests.Plugins
{
    public class PluginManagerTests
    {
        [Fact]
        public void MarkPluginAsInstalled_ThrowsExceptionOnNullSystemNameSystemName()
        {
typeof(ArgumentNullException).ShouldBeThrownBy(() => PluginManager.MarkPluginAsInstalled(null), "systemName");
        }

        [Fact]
        public void MarkPluginAsInstalled_ThrowsExceptionOnEmptySystemNameSystemName()
        {
            typeof(ArgumentNullException).ShouldBeThrownBy(() => PluginManager.MarkPluginAsInstalled(string.Empty), "systemName");
        }

        [Fact]
        public void MarkPluginAsInstalled_CreatesPluginFileIfNotExists()
        {


    ////var server = MockRepository.GeneratePartialMock<HttpServerUtilityBase>();
    //        var context = new MockRepository().();
            
    //        PartialMock<HttpContextBase>();
    //context.Expect(x => x.Server).Return(server);
    //var expected = @"c:\work\App_Data\foo.txt";
    //server.Expect(x => x.MapPath("~/App_Data/foo.txt")).Return(expected);
    //var requestContext = new RequestContext(context, new RouteData());
    //sut.ControllerContext = new ControllerContext(requestContext, sut);

    //// act
    //var actual = sut.Index();

    //// assert
    //var viewResult = actual as ViewResult;
    //Assert.AreEqual(viewResult.Model, expected);

    //        PluginManager.in
    //        PluginManager.MarkPluginAsInstalled("Test Plugin");
        }
    }
}
