using System.Linq;
using Automation.Core.Infrastructure;
using Automation.Extensions.UnitTesting;
using Xunit;


namespace Automation.Core.Tests.Infrastructure
{
    public class TypeFinderTests
    {
        [Fact]
      [Trait("Category", "unit_test")]
        public void TypeFinder_Benchmark_Findings()
        {
            var finder = new AppDomainTypeFinder();

            var type = finder.FindClassesOfType<ISomeInterface>();
            type.Count().ShouldEqual(1);
            typeof (ISomeInterface).IsAssignableFrom(type.FirstOrDefault()).ShouldBeTrue();
        }

        public interface ISomeInterface
        {
        }

        public class SomeClass : ISomeInterface
        {
        }
    }
}