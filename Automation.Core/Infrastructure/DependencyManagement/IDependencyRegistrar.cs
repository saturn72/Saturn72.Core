using Autofac;

namespace Automation.Core.Infrastructure.DependencyManagement
{
    public interface IDependencyRegistrar
    {
        int Order { get; }

        void Register(ContainerBuilder builder, ITypeFinder typeFinder);
    }
}