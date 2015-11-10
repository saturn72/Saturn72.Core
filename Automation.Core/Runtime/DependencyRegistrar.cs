using Autofac;
using Automation.Core.Caching;
using Automation.Core.Infrastructure;
using Automation.Core.Infrastructure.DependencyManagement;

namespace Automation.Core.Runtime
{
    internal class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get { return 999999999; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //Register main interceptor
      
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>()
                .Named<ICacheManager>("auto_cache_static").SingleInstance();
        }
    }
}