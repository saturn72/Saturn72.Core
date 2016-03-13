using Autofac;
using Saturn72.Core;
using Saturn72.Core.Data;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Extensions;
using Saturn72.Modules.EntityFramework.Settings;

namespace Saturn72.Modules.EntityFramework.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 10;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            var dataProviderManager = new EfDataProviderManager();
            var dataSettings = dataProviderManager.DataSettings;
            Guard.MustFollow(() => dataSettings.NotNull() && dataSettings.IsValid(),
                () => { throw new Saturn72Exception("Failed to read DataSettings."); });

            var dataProvider = dataProviderManager.DataProvider;
            Guard.NotNull(dataProvider, "no database provider was found");
            builder.RegisterInstance(dataProvider).As<IDatabaseProvider>().SingleInstance();

            var nameOrConnectionString = dataSettings.DataConnectionString.HasValue()
                ? dataSettings.DataConnectionString
                : dataSettings.DatabaseName;

            builder.Register<IDbContext>(c => new Saturn72ObjectContext(nameOrConnectionString))
                .InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof (EfRepository<>)).As(typeof (IRepository<>)).InstancePerLifetimeScope();
        }
    }
}