using System;
using Saturn72.Core.Infrastructure;
using Saturn72.Core.Tests.Fakes;

namespace Saturn72.Core.Tests
{
    public abstract class TypeFindingBase : TestsBase
    {
        protected ITypeFinder typeFinder;
        protected abstract Type[] GetTypes();

        public override void SetUp()
        {
            base.SetUp();
            typeFinder = new FakeTypeFinder(typeof (TypeFindingBase).Assembly, GetTypes());
        }
    }
}