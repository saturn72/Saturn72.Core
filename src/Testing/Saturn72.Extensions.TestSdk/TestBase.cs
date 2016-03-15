using System;
using System.Security.Principal;
using Moq;

namespace Saturn72.Extensions.TestSdk
{
    public abstract class TestsBase : IDisposable
    {
        protected MockRepository Mocks;

        protected TestsBase()
        {
            SetUp();
        }

        public void Dispose()
        {
            TearDown();
        }

        public virtual void SetUp()
        {
            Mocks = new MockRepository(MockBehavior.Default);
        }

        protected virtual void TearDown()
        {
            if (Mocks != null)
            {
              //  mocks.ReplayAll();
                Mocks.VerifyAll();
            }
        }

        protected static IPrincipal CreatePrincipal(string name, params string[] roles)
        {
            return new GenericPrincipal(new GenericIdentity(name, "TestIdentity"), roles);
        }
    }
}