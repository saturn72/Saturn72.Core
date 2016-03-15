using Saturn72.Core.Infrastructure;
using Saturn72.Core.Modules;

namespace Saturn72.Modules.EntityFramework
{
    public class EntityFrameworkModule : Resolver, IModule
    {
        public void Load()
        {
        }

        public void Start()
        {
            Resolve<IDatabaseProvider>().SetDatabaseInitializer();
        }

        public void Stop()
        {
        }

        public int StartupOrder => -1000;
        public int StopOrder => 1000;
    }
}