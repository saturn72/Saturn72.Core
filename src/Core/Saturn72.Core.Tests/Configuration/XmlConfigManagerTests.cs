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
            Assert.Equal("Modules", xmlConfig.AppDomainLoadData.ModulesParentDirecotry);
            Assert.Equal("Plugins", xmlConfig.AppDomainLoadData.PluginsParentDirecotry);

            var expectedModules = new[]
            {
                new Module("Saturn72.Modules.Data.EntityFrameworkRepository, AssemblyName", true, "Module description", "name1"),
                new Module("Saturn72.Modules.WebApi.RestfulServer, AssemblyName", false, "Module description", "name2")
            };
            Assert.Equal(expectedModules.Length, xmlConfig.Modules.Count());

            for (var i = 0; i < expectedModules.Length; i++)
            {
                Assert.Equal(expectedModules[i].Active, xmlConfig.Modules.ElementAt(i).Active);
                Assert.Equal(expectedModules[i].Description, xmlConfig.Modules.ElementAt(i).Description);
                Assert.Equal(expectedModules[i].Type, xmlConfig.Modules.ElementAt(i).Type);
                Assert.Equal(expectedModules[i].Name, xmlConfig.Modules.ElementAt(i).Name);
            }
        }

        [Fact]
        public void Load_AppDomainLoadDataLoadsDefaultOnMissingValues()
        {
            var configPath = "Config/UseDefaultAppDomainLoadDataConfig.xml";
            var xmlConfig = new XmlConfigManager();
            xmlConfig.Load(configPath);

            Assert.Equal(true, xmlConfig.AppDomainLoadData.DeleteShadowDirectoryOnStartup);
            Assert.Equal("Modules", xmlConfig.AppDomainLoadData.ModulesParentDirecotry);
            Assert.Equal("Plugins", xmlConfig.AppDomainLoadData.PluginsParentDirecotry);
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