namespace Automation.Core.Plugins
{
    public abstract class BasePlugin : IPlugin
    {
        public PluginDescriptor PluginDescriptor { get; set; }
    }
}