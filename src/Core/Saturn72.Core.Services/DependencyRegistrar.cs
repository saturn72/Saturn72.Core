using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Saturn72.Core.Configuration;
using Saturn72.Core.Data;
using Saturn72.Core.Events;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Logging;
using Saturn72.Core.Services.Configuration;
using Saturn72.Core.Services.Events;
using Saturn72.Core.Services.Logging;
using Saturn72.Core.Services.Queue;
using Saturn72.Core.Services.Sites;
using Saturn72.Core.Services.Tasks;
using Saturn72.Core.Services.Users;

namespace Saturn72.Core.Services
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order => 10;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            RegisterPubSubComponents(builder, typeFinder);

            //settings
            builder.RegisterSource(new SettingsSource());

            //Services
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<PortalService>().As<IProtalService>().InstancePerLifetimeScope();
            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerLifetimeScope();

            //Managers
            builder.RegisterType<TaskManager>().As<ITaskManager>().InstancePerLifetimeScope();


            builder.RegisterGeneric(typeof (ListDataCollection<>)).As(typeof (IDataCollection<>))
                //.Named("list", typeof (IDataCollection<>))
                .SingleInstance();
        }

        protected virtual void RegisterPubSubComponents(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //Register event consumers
            var consumers = typeFinder.FindClassesOfType(typeof (IConsumer<>)).ToList();
            foreach (var consumer in consumers)
            {
                builder.RegisterType(consumer)
                    .As(consumer.FindInterfaces((type, criteria) =>
                    {
                        var isMatch = type.IsGenericType &&
                                      ((Type) criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                        return isMatch;
                    }, typeof (IConsumer<>)))
                    .InstancePerLifetimeScope();
            }

            builder.RegisterType<EventPublisher>().As<IEventPublisher>().SingleInstance();
            builder.RegisterType<SubscriptionService>().As<ISubscriptionService>().SingleInstance();
            builder.RegisterGeneric(typeof (QueueManager<>)).As(typeof (IQueueManager<>)).SingleInstance();

            //Core services Logging
            builder.RegisterType<DbLogger>().As<ILogger>().SingleInstance();
            builder.RegisterType<DbLogRecordService>().As<ILogRecordService>().InstancePerLifetimeScope();
        }
    }

    public class SettingsSource : IRegistrationSource
    {
        private static readonly MethodInfo BuildMethod = typeof (SettingsSource).GetMethod(
            "BuildRegistration",
            BindingFlags.Static | BindingFlags.NonPublic);

        public IEnumerable<IComponentRegistration> RegistrationsFor(
            Service service,
            Func<Service, IEnumerable<IComponentRegistration>> registrations)
        {
            var ts = service as TypedService;
            if (ts != null && typeof (ISettings).IsAssignableFrom(ts.ServiceType))
            {
                var buildMethod = BuildMethod.MakeGenericMethod(ts.ServiceType);
                yield return (IComponentRegistration) buildMethod.Invoke(null, null);
            }
        }

        public bool IsAdapterForIndividualComponents
        {
            get { return false; }
        }

        private static IComponentRegistration BuildRegistration<TSettings>() where TSettings : ISettings, new()
        {
            return RegistrationBuilder
                .ForDelegate((c, p) => c.Resolve<ISettingService>().LoadSetting<TSettings>())
                .InstancePerLifetimeScope()
                .CreateRegistration();
        }
    }
}