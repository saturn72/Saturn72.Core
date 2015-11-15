using System;
using Automation.Core.Infrastructure;
using Automation.Extensions.UnitTesting;
using Xunit;

namespace Automation.Core.Tests.Infrastructure
{
    public class SingletonTests
    {
        [Fact]
        [Trait("Category", "unit_test")]
        public void Singleton_IsNullByDefault()
        {
            var instance = Singleton<SingletonTests>.Instance;
            Assert.Null(instance);
        }

        [Fact]
        [Trait("Category", "unit_test")]
        public void Singletons_ShareSame_SingletonsDictionary()
        {
            Singleton<int>.Instance = 1;
            Singleton<double>.Instance = 2.0;

            Singleton.AllSingletons.ShouldEqual(Singleton.AllSingletons);
            Singleton.AllSingletons[typeof (int)].ShouldEqual(1);
            Singleton.AllSingletons[typeof (double)].ShouldEqual(2.0);
        }

        [Fact]
        [Trait("Category", "unit_test")]
        public void SingletonDictionary_IsCreatedByDefault()
        {
            var instance = SingletonDictionary<SingletonTests, object>.Instance;
            Assert.NotNull(instance);
        }

        [Fact]
        [Trait("Category", "unit_test")]
        public void SingletonDictionary_CanStoreStuff()
        {
            var instance = SingletonDictionary<Type, SingletonTests>.Instance;
            instance[typeof (SingletonTests)] = this;
            instance[typeof (SingletonTests)].ShouldEqual(this);
        }

        [Fact]
        [Trait("Category", "unit_test")]
        public void SingletonList_IsCreatedByDefault()
        {
            var instance = SingletonList<SingletonTests>.Instance;
            Assert.NotNull(instance);
        }

        [Fact]
        [Trait("Category", "unit_test")]
        public void SingletonList_CanStoreItems()
        {
            var instance = SingletonList<SingletonTests>.Instance;
            instance.Insert(0, this);
            instance[0].ShouldEqual(this);
        }
    }
}