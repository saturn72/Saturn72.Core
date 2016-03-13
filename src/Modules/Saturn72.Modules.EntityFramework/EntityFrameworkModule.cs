using Saturn72.Core.Infrastructure;
using Saturn72.Core.Modules;

namespace Saturn72.Modules.EntityFramework
{
    public class EntityFrameworkModule : Resolver, IModule
    {
        public void Load()
        {
            Resolve<IDatabaseProvider>().SetDatabaseInitializer();
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}