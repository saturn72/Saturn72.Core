namespace Saturn72.Core.Infrastructure
{
    public class AppDomainLoadData
    {
        #region ctor

        public AppDomainLoadData()
            : this(true)
        {
        }

        public AppDomainLoadData(bool deleteShadowDirectoryOnStartup) :
            this(deleteShadowDirectoryOnStartup, "Plugins", "Modules")
        {
        }

        public AppDomainLoadData(bool deleteShadowDirectoryOnStartup, string pluginsParentDirectory,
            string modulesParentDirectory)
            : this(deleteShadowDirectoryOnStartup,
                new DynamicLoadingData(pluginsParentDirectory, pluginsParentDirectory + @"\bin"),
                new DynamicLoadingData(modulesParentDirectory, modulesParentDirectory + @"\bin"))
        {
        }

        public AppDomainLoadData(bool deleteShadowDirectoryOnStartup, DynamicLoadingData pluginsDynamicLoadData,
            DynamicLoadingData modulesDynamicLoadData)
        {
            DeleteShadowDirectoryOnStartup = deleteShadowDirectoryOnStartup;
            ModulesDynamicLoadingData = modulesDynamicLoadData;
            PluginsDynamicLoadingData = pluginsDynamicLoadData;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the dynamic download data for plugins <see cref="DynamicLoadingData" />
        /// </summary>
        public DynamicLoadingData PluginsDynamicLoadingData { get; }

        /// <summary>
        ///     Gets the dynamic download data for modules <see cref="DynamicLoadingData" />
        /// </summary>
        public DynamicLoadingData ModulesDynamicLoadingData { get; }

        /// <summary>
        ///     Get value indicating if the shadow copy folder should be deleted on startup
        /// </summary>
        public bool DeleteShadowDirectoryOnStartup { get; }

        #endregion
    }
}