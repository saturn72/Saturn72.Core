using Autofac;

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public interface IDependencyRegistrar
    {
        int Order { get; }

        void Register(ContainerBuilder builder, ITypeFinder typeFinder);
    }
}