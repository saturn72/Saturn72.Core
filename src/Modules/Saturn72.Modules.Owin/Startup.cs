using System.Web.Http;
using Autofac.Integration.WebApi;
using Owin;
using Saturn72.Core.Infrastructure;

namespace Saturn72.Modules.Owin
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            // Configure Web API for self-host. 
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute("DefaultRestApi", "api/{controller}/{id}", new {id = RouteParameter.Optional}
                );

            config.Routes.MapHttpRoute("DefaultApi", "api/{controller}/{action}/{id}",
                new {id = RouteParameter.Optional}
                );

            appBuilder.UseWebApi(config);
            SetupAutofacWebApi(appBuilder, config);

            config.MapHttpAttributeRoutes();
            config.EnsureInitialized();
        }

        private void SetupAutofacWebApi(IAppBuilder appBuilder, HttpConfiguration config)
        {
            //Do not chage autofac registration order
            var container = EngineContext.Current.ContainerManager.Container;
            appBuilder.UseAutofacMiddleware(container);
            appBuilder.UseAutofacWebApi(config);
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}