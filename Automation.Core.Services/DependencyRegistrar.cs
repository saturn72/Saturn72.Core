using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Automation.Core.Configuration;
using Automation.Core.Data;
using Automation.Core.Events;
using Automation.Core.Infrastructure;
using Automation.Core.Infrastructure.DependencyManagement;
using Automation.Core.Services.Activity;
using Automation.Core.Services.Aspects;
using Automation.Core.Services.Configuration;
using Automation.Core.Services.Events;
using Automation.Core.Services.Execution;
using Automation.Core.Services.Jobs;
using Automation.Core.Services.Queue;
using Automation.Core.Services.Reporting;
using Automation.Core.Services.Sites;
using Automation.Core.Services.Tasks;
using Automation.Core.Services.Users;
using Castle.DynamicProxy;

namespace Automation.Core.Services
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public int Order
        {
            get { return 10; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            RegisterPubSubComponents(builder, typeFinder);

            //settings
            builder.RegisterSource(new SettingsSource());

            //report
            builder.RegisterType<Reporter>().As<IReporter>().SingleInstance();
            //Aspects
            builder.RegisterType<RuntimeInterceptor>().As<IRuntimeInterceptor>().SingleInstance()
                .Named<IInterceptor>(RuntimeInterceptor.InterceptionKey);

            //Services
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();
            builder.RegisterType<ActivityExecutionService>().As<IActivityExecutionService>().InstancePerLifetimeScope();
            builder.RegisterType<UserService>().As<IUserService>().InstancePerLifetimeScope();
            builder.RegisterType<PortalService>().As<IProtalService>().InstancePerLifetimeScope();

            builder.RegisterType<AutomationJobService>().As<IAutomationJobService>().InstancePerLifetimeScope();
            builder.RegisterType<AutomationJobPlanService>().As<IAutomationJobPlanService>().InstancePerLifetimeScope();
            builder.RegisterType<TestCaseExecutionDataService>().As<ITestCaseExecutionDataService>().InstancePerLifetimeScope();
            builder.RegisterType<ExecutionService>().As<IExecutionService>().InstancePerLifetimeScope();

            builder.RegisterType<ExecutionQueueManager>().As<IExecutionQueueManager>()
                .SingleInstance();
            builder.RegisterType<TestExecutor>().As<ITestExecutor>().InstancePerLifetimeScope();

            builder.RegisterType<ScheduleTaskService>().As<IScheduleTaskService>().InstancePerLifetimeScope();


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