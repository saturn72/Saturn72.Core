using System;
using System.Threading;
using Saturn72.Core;
using Saturn72.Core.Configuration;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Modules;
using Saturn72.Extensions;

namespace Saturn72.App.Common
{
    public class Saturn72App
    {
        /// <summary>
        ///     Starts the application
        /// </summary>
        public void Start()
        {
            var terminate = false;

            Console.Out.WriteLine("Start {0} application".AsFormat(_appId));

            Console.Out.WriteLine("LoadToAppDomain application modules...");
            LoadAllModules();

            Console.Out.WriteLine("Start application engine...");
            EngineContext.Initialize(true);

            Console.Out.WriteLine("Start all modules...");
            StartAllModules();

            Console.CancelKeyPress += (o, args) => terminate = true;
            Console.Out.Write("Press CTRL+C to break application.");
            while (!terminate)
            {
                Thread.Sleep(5000);
            }

            Console.Out.WriteLine("Stop all modules...");
            StopAllModules();

        }


        #region Fields

        private readonly string _appId;
        private readonly IConfigManager _configManager;

        #endregion

        #region ctors

        /// <summary>
        ///     Created new instance of Saturn72App
        /// </summary>
        /// <param name="appId">Application Id</param>
        public Saturn72App(string appId) : this(appId, "Config/RootConfig.xml")
        {
        }

        /// <summary>
        ///     Created new instance of Saturn72App
        /// </summary>
        /// <param name="appId">Application Id</param>
        /// <param name="rootConfigFilePath">Root config file path</param>
        public Saturn72App(string appId, string rootConfigFilePath)
            : this(appId, rootConfigFilePath, GetDefaultConfigManager(rootConfigFilePath))
        {
        }

        /// <summary>
        ///     Created new instance of Saturn72App
        /// </summary>
        /// <param name="appId">Application Id</param>
        /// <param name="rootConfigFilePath">Root config file path</param>
        /// <param name="configManager">Config holder</param>
        public Saturn72App(string appId, string rootConfigFilePath, IConfigManager configManager)
        {
            _appId = appId;
            _configManager = configManager;
        }

        #endregion

        #region Utilities

        protected static IConfigManager GetDefaultConfigManager(object configData)
        {
            var ch = new XmlConfigManager();
            ch.Load(configData as string);
            return ch;
        }

        private void LoadAllModules()
        {
            AppDomainLoader.LoadToAppDomain(_configManager.AppDomainLoadData);
            var typeFinder = new AppDomainTypeFinder();
            typeFinder.FindClassesOfTypeAndRunMethod<IModule>(m=>m.Load(), m=>m.StartupOrder);
        }


        /// <summary>
        ///     Stops all application modules
        /// </summary>
        protected virtual void StopAllModules()
        {
            var typeFinder = Resolver.TypeFinder;
            typeFinder.FindClassesOfTypeAndRunMethod<IModule>(m => m.Stop(), m => m.StopOrder);
        }


        /// <summary>
        ///     Starts all application modules
        /// </summary>
        protected virtual void StartAllModules()
        {
            var typeFinder = Resolver.TypeFinder;
            typeFinder.FindClassesOfTypeAndRunMethod<IModule>(m => m.Start(), m => m.StartupOrder);
        }

        #endregion
    }
}