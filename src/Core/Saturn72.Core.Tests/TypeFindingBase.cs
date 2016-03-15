using System;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Tests.Fakes;
using Saturn72.TestSdk.UnitTesting;

namespace Saturn72.Core.Tests
{
    public abstract class TypeFindingBase : TestsBase
    {
        protected ITypeFinder TypeFinder;
        protected abstract Type[] GetTypes();

        public override void SetUp()
        {
            base.SetUp();
            TypeFinder = new FakeTypeFinder(typeof (TypeFindingBase).Assembly, GetTypes());
        }
    }
}