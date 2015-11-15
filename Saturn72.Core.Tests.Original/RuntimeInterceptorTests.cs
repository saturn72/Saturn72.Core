using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Extras.DynamicProxy2;
using Automation.Core.Aspects;
using Automation.Core.Services.Aspects;
using Automation.Extensions.UnitTesting;
using Castle.DynamicProxy;
using Xunit;


namespace Automation.Core.Tests
{
    public class RuntimeInterceptorTests
    {
        public RuntimeInterceptorTests()
        {
            Setup();
        }

        public void Setup()
        {
            var builder = new ContainerBuilder();
            builder.Register(i => new RuntimeInterceptorStub()).Named<IInterceptor>(RuntimeInterceptor.InterceptionKey);
            builder.RegisterType<SomeType>()
                .As<ISomeInterface>()
                .EnableInterfaceInterceptors()
                .InterceptedBy(RuntimeInterceptor.InterceptionKey);
            _container = builder.Build();
        }

        private IContainer _container;

        [Fact]
        public void Intercepet_RunInterception()
        {
            _container.Resolve<ISomeInterface>()
                .ShouldBeIntercepted("test")
                .ShouldEqual("test-->>PreInvocation-->>Invocation-->> Intercepted Method code-->>PostInvocation");
        }

        [Fact]
        public void Intercepet_SkipInterception()
        {
            _container.Resolve<ISomeInterface>().SkipInterception("test").ShouldEqual(10);
        }
    }

    public class RuntimeInterceptorStub : RuntimeInterceptor
    {
        protected override IEnumerable<Type> LoadReturnTypestoIntercept()
        {
            return new[] {typeof (string)};
        }
    }

    public interface ISomeInterface
    {
        int SkipInterception(string message);
        string ShouldBeIntercepted(string message);
    }

    public class SomeType : ISomeInterface
    {
        public virtual int SkipInterception(string message)
        {
            return 10;
        }

        public string ShouldBeIntercepted(string message)
        {
            return "-->> Intercepted Method code";
        }
    }

    internal class PreInvokeAspect : IPreInvocationAspect
    {
        public int Order
        {
            get { return 0; }
        }

        public void Action(AspectMessage aspectMessage)
        {
            var msg = aspectMessage.Invocation.Arguments[0];
            aspectMessage.Invocation.Arguments[0] = msg + "-->>PreInvocation";
        }
    }

    internal class InvokeAspect : IInvocationAspect
    {
        public int Order
        {
            get { return 8; }
        }

        public void Action(AspectMessage aspectMessage)
        {
            var msg = aspectMessage.Invocation.Arguments[0];
            aspectMessage.Invocation.Arguments[0] = msg + "-->>Invocation";
        }
    }

    internal class PostInvokeAspect : IPostInvocationAspect
    {
        public int Order
        {
            get { return 7; }
        }

        public void Action(AspectMessage aspectMessage)
        {
            var retVal = (string)aspectMessage.Invocation.Arguments[0] + aspectMessage.Invocation.ReturnValue;
            aspectMessage.Invocation.ReturnValue = retVal + "-->>PostInvocation";
        }
    }
}