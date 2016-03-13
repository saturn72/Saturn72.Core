using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Autofac;
using Saturn72.Core.Infrastructure.DependencyManagement;
using Saturn72.Core.Tasks;
using Saturn72.Extensions;

namespace Saturn72.Core.Infrastructure
{
    public class Engine: IEngine
    {
        #region Properties

        public ContainerManager ContainerManager { get; private set; }

        #endregion Properties

        public void Initialize()
        {
            if (!CommonHelper.IsWebApp())
                LoadPlugins();
            var typeFinder = new WebAppTypeFinder();
            RegisterDependencies(typeFinder);
            RunStartupTasks(typeFinder);
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
                Trace.WriteLine("Start pre-application start method on type: " + type.FullName + " Method: " +
                                apsm.MethodName);

                methodInfo.Invoke(instance, null);
            }
        }

        #region Fields

        #endregion Fields

        #region Utilities

        /// <summary>
        /// Registers all dependencies across domain
        /// </summary>
        /// <param name="typeFinder">TypeFinder <see cref="ITypeFinder"/></param>
        protected virtual void RegisterDependencies(ITypeFinder typeFinder)
        {
            var builder = new ContainerBuilder();
            var container = builder.Build();

            //Core dependencies
            builder = new ContainerBuilder();
            builder.RegisterInstance(this).As<IEngine>().SingleInstance();
            builder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            builder.Update(container);

            //register dependencies provided by other assemblies
            builder = new ContainerBuilder();
            typeFinder.FindTypeAndRunMethod<IDependencyRegistrar>(dr => dr.Register(builder, typeFinder), dr => dr.Order);
            builder.Update(container);
            ContainerManager = new ContainerManager(container);


            //TODO: only for web apps
            ////set dependency resolver
            //if (CommonHelper.IsWebApp())
            //{
            //    DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            //    GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            //}
        }

        /// <summary>
        /// Runs all startup task across domain
        /// </summary>
        /// <param name="typeFinder">TypeFinder <see cref="ITypeFinder"/></param>
        protected virtual void RunStartupTasks(ITypeFinder typeFinder)
        {
            typeFinder.FindTypeAndRunMethod<IStartupTask>(s => s.Execute(), s => s.Order);
        }

        #endregion Utilities
    }
}