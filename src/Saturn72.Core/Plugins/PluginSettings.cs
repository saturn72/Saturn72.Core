using System;
using System.Configuration;
using Saturn72.Core.Configuration;

namespace Saturn72.Core.Plugins
{
    public class PluginSettings : ISettings
    {
        private const string PluginRelativePath = "~/Plugins";
        private const string ShadowCopyRelativePath = "~/Plugins/bin";

        public string PluginFolder { get; set; }
        public bool ClearShadowDirectoryOnStartup { get; set; }
        public string ShadowCopyFolder { get; set; }

        public static PluginSettings LoadSettings()
        {
            return new PluginSettings
            {
                PluginFolder = CommonHelper.BuildRelativePath(PluginRelativePath),
                ClearShadowDirectoryOnStartup = GetClearShadowDirectoryOnStartupValue(),
                ShadowCopyFolder = CommonHelper.BuildRelativePath(ShadowCopyRelativePath)
            };
        }

        private static bool GetClearShadowDirectoryOnStartupValue()
        {
            return !string.IsNullOrEmpty(ConfigurationManager.AppSettings["ClearPluginsShadowDirectoryOnStartup"]) &&
                   Convert.ToBoolean(ConfigurationManager.AppSettings["ClearPluginsShadowDirectoryOnStartup"]);
        }
    }
}