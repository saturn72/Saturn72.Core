using System.Linq;
using Saturn72.Core.Infrastructure;
using Saturn72.TestSdk.UnitTesting;
using Xunit;

namespace Saturn72.Core.Tests.Infrastructure
{
    public class TypeFinderTests
    {
        [Fact]        
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