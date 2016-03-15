using System.Web.Http;
using Owin;
using Saturn72.Core.Infrastructure;

namespace Saturn72.Modules.Owin
{
    public class Startup
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            //Resolver.TypeFinder.FindClassesOfTypeAndRunMethod<IOwinMiddleware>(m => m.Configure(appBuilder),
            //    m => m.Order);

            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}
                );

            appBuilder.UseWebApi(config);
        }
    }
}