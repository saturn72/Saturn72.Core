using System;
using System.Linq;
using Saturn72.Core.Infrastructure;
using Xunit;

namespace Saturn72.Core.Tests.Infrastructure
{
    public class AppDomainLoaderTests
    {
        [Fact]
        public void Load_AddsAssemblyToAppDomain()
        {
            var data = new AppDomainLoadData();

            AppDomainLoader.LoadToAppDomain(data);
            var loadedAssembliesNames = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetName().Name);

            Assert.True(loadedAssembliesNames.Contains("AssemblyToLoad"), "AppdomainLoader failed to load assembly");
        }
    }
}
