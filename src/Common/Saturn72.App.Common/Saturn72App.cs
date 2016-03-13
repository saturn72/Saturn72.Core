using System;
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
            Console.Out.WriteLine("Start {0} application".AsFormat(_appId));
            LoadAllModules();
            StartAllModules();

            //Load and start AllModules
            //Loadand start AllPlugins
            //
        }


        /// <summary>
        ///     Loads all application modules
        /// </summary>
        protected virtual void StartAllModules()
        {
            _configManager.Modules.ForEachItem(m => { ModuleManager.Start(m); });
        }

        #region Fields

        private readonly string _appId;
        private readonly string _rootConfigFilePath;
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
            _rootConfigFilePath = rootConfigFilePath;
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
            AppDomainLoader.Load(_configManager.AppDomainLoadData);
        }

        #endregion
    }
}