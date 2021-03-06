﻿namespace Saturn72.Core.Plugins
{
    public interface IInstallablePlugin : IPlugin
    {
        /// <summary>
        /// Install plugin
        /// </summary>
        void Install();

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        void Uninstall();
    }
}
