using Autofac;
using Saturn72.Core.Caching;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.DependencyManagement;

namespace Saturn72.Core.Runtime
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