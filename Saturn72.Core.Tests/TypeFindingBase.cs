using System;
using Automation.Core.Infrastructure;
using Automation.Core.Tests.Fakes;
using Automation.Extensions.UnitTesting;

namespace Automation.Core.Tests
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