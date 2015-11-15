using System.Collections.Generic;
using System.Linq;
using Autofac;
using Xunit;


namespace Automation.Core.Tests.Infrastructure.DependencyManagement
{
    public class AutofacTests
    {
        public interface IFoo
        {
        }

        public class Foo1 : IFoo
        {
        }

        public class Foo2 : IFoo
        {
        }

        public class Foo3 : IFoo
        {
        }

        [Fact(DisplayName = "Exercises a problem in a previous version, to make sure older Autofac.dll isn't picked up")
        ]
        public void EnumerablesFromDifferentLifetimeScopesShouldReturnDifferentCollections()
        {
            var rootBuilder = new ContainerBuilder();
            rootBuilder.RegisterType<Foo1>().As<IFoo>();
            var rootContainer = rootBuilder.Build();

            var scopeA = rootContainer.BeginLifetimeScope(
                scopeBuilder => scopeBuilder.RegisterType<Foo2>().As<IFoo>());
            var arrayA = scopeA.Resolve<IEnumerable<IFoo>>().ToArray();

            var scopeB = rootContainer.BeginLifetimeScope(
                scopeBuilder => scopeBuilder.RegisterType<Foo3>().As<IFoo>());
            var arrayB = scopeB.Resolve<IEnumerable<IFoo>>().ToArray();

            Assert.Equal(2, arrayA.Count());
            Assert.True(arrayA.Any(x => x.GetType() == typeof(Foo1)));
            Assert.True(arrayA.Any(x => x.GetType() == typeof(Foo2)));

            Assert.Equal(2, arrayB.Count());
            Assert.True(arrayB.Any(x => x.GetType() == typeof(Foo1)));
            Assert.True(arrayB.Any(x => x.GetType() == typeof(Foo3)));
        }
    }
}