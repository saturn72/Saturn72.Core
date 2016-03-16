using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Modules;
using Saturn72.Extensions;

namespace Saturn72.Core.Configuration
{
    public class XmlConfigManager : IConfigManager
    {
        private IList<Module> _modules;

        /// <summary>
        ///     Loads all config nodes
        /// </summary>
        /// <param name="data">object contains data for config loading</param>
        public void Load(object data)
        {
            var configPath = data as string;
            if (configPath.IsNull())
            {
                Console.Out.WriteLine("Config file does not exists");
                return;
            }
            Guard.FileExists(configPath);

            var configRoot = XDocument.Load(configPath).FirstNode;
            LoadAppDomain(configRoot);
            LoadModules(configRoot);
        }

        #region Utilities

        private void LoadAppDomain(XNode configRoot)
        {
            var mde = configRoot.XPathSelectElement("appdomain");
            var deleteShadowOnStartup = mde.Attribute("DeleteShadowDirectoryOnStartup")?.Value.ToBoolean() ?? true;
            var moduleDirectory = configRoot.XPathSelectElement("appdomain/modules")?.Attribute("Directory")?.Value ??
                                  "Modules";
            var moduleShadowCopyDirectory =
                configRoot.XPathSelectElement("appdomain/modules")?.Attribute("ShadowCopyDirectory")?.Value ??
                @"Modules\bin";
            var pluginsDirectory = configRoot.XPathSelectElement("appdomain/plugins")?.Attribute("Directory")?.Value ??
                                   "Plugins";
            var pluginsShadowCopyDirectory =
                configRoot.XPathSelectElement("appdomain/plugins")?.Attribute("ShadowCopyDirectory")?.Value ??
                @"Plugins\bin";

            AppDomainLoadData = new AppDomainLoadData(deleteShadowOnStartup,
                new DynamicLoadingData(pluginsDirectory, pluginsShadowCopyDirectory),
                new DynamicLoadingData(moduleDirectory, moduleShadowCopyDirectory));
        }

        private void LoadModules(XNode configRoot)
        {
            var elements = configRoot.XPathSelectElements("modules/module");
            _modules = new List<Module>(elements.Count());
            elements.ForEachItem(m =>
            {
                var type = m.Attribute("Type").Value;
                var active = m.Attribute("Active")?.Value.ToBoolean() ?? true;
                var description = m.Attribute("Description")?.Value ?? string.Empty;
                var name = m.Attribute("Name")?.Value ?? type;
                _modules.Add(new Module(type, active, description, name));
            });
        }

        #endregion

        #region Properties

        public IEnumerable<Module> Modules => _modules;

        public AppDomainLoadData AppDomainLoadData { get; private set; }

        #endregion
    }
}