using System.Linq;
using Autofac;
using Autofac.Integration.WebApi;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Web.Controllers;

namespace Saturn72.Modules.Owin
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 100;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            var webapiControllerAssemnblies = typeFinder.FindAssembliesWithTypeDerivatives<Saturn72ApiControllerBase>();
            builder.RegisterApiControllers(webapiControllerAssemnblies.ToArray());
            //var appDomainAssemblies = typeFinder.GetAssemblies().ToArray();
            //builder.RegisterApiControllers(appDomainAssemblies);
        }
    }
}