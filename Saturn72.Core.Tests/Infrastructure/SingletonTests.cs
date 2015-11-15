using System;
using Saturn72.Core.Infrastructure;
using Saturn72.TestSdk.UnitTesting;
using Xunit;

namespace Saturn72.Core.Tests.Infrastructure
{
    public class SingletonTests
    {
        [Fact]
        public void Singleton_IsNullByDefault()
        {
            var instance = Singleton<SingletonTests>.Instance;
            Assert.Null(instance);
        }

        [Fact]
        public void Singletons_ShareSame_SingletonsDictionary()
        {
            Singleton<int>.Instance = 1;
            Singleton<double>.Instance = 2.0;

            Singleton.AllSingletons.ShouldEqual(Singleton.AllSingletons);
            Singleton.AllSingletons[typeof (int)].ShouldEqual(1);
            Singleton.AllSingletons[typeof (double)].ShouldEqual(2.0);
        }

        [Fact]
        public void SingletonDictionary_IsCreatedByDefault()
        {
            var instance = SingletonDictionary<SingletonTests, object>.Instance;
            Assert.NotNull(instance);
        }

        [Fact]
        public void SingletonDictionary_CanStoreStuff()
        {
            var instance = SingletonDictionary<Type, SingletonTests>.Instance;
            instance[typeof (SingletonTests)] = this;
            instance[typeof (SingletonTests)].ShouldEqual(this);
        }

        [Fact]
        public void SingletonList_IsCreatedByDefault()
        {
            var instance = SingletonList<SingletonTests>.Instance;
            Assert.NotNull(instance);
        }

        [Fact]
        public void SingletonList_CanStoreItems()
        {
            var instance = SingletonList<SingletonTests>.Instance;
            instance.Insert(0, this);
            instance[0].ShouldEqual(this);
        }
    }
}