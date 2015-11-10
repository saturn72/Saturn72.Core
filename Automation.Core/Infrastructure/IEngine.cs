using Automation.Core.Infrastructure.DependencyManagement;

namespace Automation.Core.Infrastructure
{
    public interface IEngine
    {
        /// <summary>
        /// Initialize components and plugins in the nop environment.
        /// </summary>
        void Initialize();

        TService Resolve<TService>(object key = null) where TService : class;

        TService[] ResolveAll<TService>() where TService : class;

        ContainerManager ContainerManager { get; }
    }
}