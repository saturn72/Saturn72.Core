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

        public AppDomainLoadData( bool deleteShadowDirectoryOnStartup, string pluginsParentDirectory,
            string modulesParentDirectory)
        {
            DeleteShadowDirectoryOnStartup = deleteShadowDirectoryOnStartup;
            ModulesParentDirecotry = modulesParentDirectory;
            PluginsParentDirecotry = pluginsParentDirectory;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the root folder of all plugins
        /// </summary>
        public string PluginsParentDirecotry { get; }

        /// <summary>
        ///     Gets the root folder of all modules
        /// </summary>
        public string ModulesParentDirecotry { get; }

        /// <summary>
        ///     Get value indicating if the shadow copy folder should be deleted on startup
        /// </summary>
        public bool DeleteShadowDirectoryOnStartup { get; }

        #endregion
    }
}