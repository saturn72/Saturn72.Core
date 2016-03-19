using System;
using System.Linq;
using Saturn72.Core.Configuration;
using Saturn72.Core.Modules;
using Xunit;

namespace Saturn72.Core.Tests.Configuration
{
    public class XmlConfigManagerTests
    {
        [Fact]
        public void Load_GetXmlData()
        {
            var configPath = "Config/RootConfig.xml";
            var xmlConfig = new XmlConfigManager();
            xmlConfig.Load(configPath);

            Assert.Equal(true, xmlConfig.AppDomainLoadData.DeleteShadowDirectoryOnStartup);
            Assert.Equal("ModuleInstances", xmlConfig.AppDomainLoadData.ModulesDynamicLoadingData.RootDirectory);
            Assert.Equal(@"ModuleInstances\bin", xmlConfig.AppDomainLoadData.ModulesDynamicLoadingData.ShadowCopyDirectory);
            Assert.Equal("Plugins", xmlConfig.AppDomainLoadData.PluginsDynamicLoadingData.RootDirectory);
            Assert.Equal(@"Plugins\bin", xmlConfig.AppDomainLoadData.PluginsDynamicLoadingData.ShadowCopyDirectory);

            var expectedModules = new[]
            {
                new ModuleInstance("Saturn72.ModuleInstances.Data.EntityFrameworkRepository, AssemblyName", true),
                new ModuleInstance("Saturn72.ModuleInstances.WebApi.RestfulServer, AssemblyName", false)
            };
            Assert.Equal(expectedModules.Length, xmlConfig.ModuleInstances.Count());

            for (var i = 0; i < expectedModules.Length; i++)
            {
                Assert.Equal(expectedModules[i].Active, xmlConfig.ModuleInstances.ElementAt(i).Active);
                Assert.Equal(expectedModules[i].Type, xmlConfig.ModuleInstances.ElementAt(i).Type);
            }
        }

        [Fact]
        public void Load_AppDomainLoadDataLoadsDefaultOnMissingValues()
        {
            var configPath = "Config/UseDefaultAppDomainLoadDataConfig.xml";
            var xmlConfig = new XmlConfigManager();
            xmlConfig.Load(configPath);

            Assert.Equal(true, xmlConfig.AppDomainLoadData.DeleteShadowDirectoryOnStartup);
            Assert.Equal("ModuleInstances", xmlConfig.AppDomainLoadData.ModulesDynamicLoadingData.RootDirectory);
            Assert.Equal(@"ModuleInstances\bin", xmlConfig.AppDomainLoadData.ModulesDynamicLoadingData.ShadowCopyDirectory);
            Assert.Equal("Plugins", xmlConfig.AppDomainLoadData.PluginsDynamicLoadingData.RootDirectory);
            Assert.Equal(@"Plugins\bin", xmlConfig.AppDomainLoadData.PluginsDynamicLoadingData.ShadowCopyDirectory);

        }

        [Fact]
        public void Load_ThrowsExceptionOnMissingModuleType()
        {
            var configPath = "Config/MissingModuleType.xml";
            var xmlConfig = new XmlConfigManager();
            Assert.Throws<NullReferenceException>(()=> xmlConfig.Load(configPath));
        }
    }
}