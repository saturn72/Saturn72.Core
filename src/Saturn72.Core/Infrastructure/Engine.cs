using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Infrastructure.Tasks;
using Saturn72.Extensions;

namespace Saturn72.Core.Infrastructure
{
    public class Engine : IEngine
    {
        #region Properties

        public ContainerManager ContainerManager { get; private set; }

        #endregion Properties

        public void Initialize()
        {
            if (!CommonHelper.IsWebApp())
                LoadPlugins();
            RegisterDependencies();
            RunStartupTasks();
        }

        public TService Resolve<TService>(object key = null) where TService : class
        {
            return ContainerManager.Resolve<TService>(key);
        }

        public TService[] ResolveAll<TService>() where TService : class
        {
            return ContainerManager.ResolveAll<TService>();
        }

        protected virtual void LoadPlugins()
        {
            var appDomainAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var preAppStartObjects = new List<object>();

            appDomainAssemblies.ForEachItem(asm =>
            {
                var attributes = asm.GetCustomAttributes(typeof (PreApplicationStartMethodAttribute), false);
                attributes.ForEachItem(att => preAppStartObjects.Add(att));
            });

            foreach (var paso in preAppStartObjects)
            {
                var apsm = paso as PreApplicationStartMethodAttribute;
                var type = apsm.Type;
                if (type.IsAbstract)
                    continue;
                var methodInfo = type.GetMethod(apsm.MethodName);
                var instance = Activator.CreateInstance(type);
                Trace.WriteLine("Start pre-application start method on type: " + type.FullName + " Method: " + apsm.MethodName);

                methodInfo.Invoke(instance, null);
            }
        }

        #region Fields

        #endregion Fields

        #region Utilities

        protected virtual void RegisterDependencies()
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            //Core dependencies
            var typeFinder = new WebAppTypeFinder();
            builder = new ContainerBuilder();
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            builder.Update(container);

            //register dependencies provided by other assemblies
            builder = new ContainerBuilder();
            var drTypes = typeFinder.FindClassesOfType<IDependencyRegistrar>();
            var drInstances = new List<IDependencyRegistrar>();
            foreach (var drType in drTypes)
                drInstances.Add((IDependencyRegistrar)Activator.CreateInstance(drType));

            //sort
            drInstances = drInstances.AsQueryable().OrderBy(t => t.Order).ToList();
            foreach (var dependencyRegistrar in drInstances)
                dependencyRegistrar.Register(builder, typeFinder);

            builder.Update(container);
            ContainerManager = new ContainerManager(container);

            //set dependency resolver
            if (CommonHelper.IsWebApp())
            {
                DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
                GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            }
        }

        /// <summary>
        ///     Runs all startup task across domain
        /// </summary>
        protected virtual void RunStartupTasks()
        {
            var typeFinder = ContainerManager.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
                startUpTask.Execute();
        }

        #endregion Utilities
    }
}