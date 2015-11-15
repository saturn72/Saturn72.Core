using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Automation.Core.Activity;
using Automation.Core.Aspects;
using Automation.Core.Domain.Logging;
using Automation.Core.Events;
using Automation.Core.Infrastructure;
using Automation.Core.Services.Events.Invocation;
using Automation.Core.Validation;
using Automation.Extensions;
using Castle.DynamicProxy;

namespace Automation.Core.Services.Aspects
{
    public class RuntimeInterceptor : IRuntimeInterceptor
    {
        #region Consts

        public const string InterceptionKey = "40283DCB-DC50-493A-AA1F-3B32545E70E5";

        #endregion

        public void Intercept(IInvocation invocation)
        {
            var action = ShouldBeIntercepted(invocation.Method)
                ? new Action(() => RunInterceptionFlow(invocation, GenerateInvocationId()))
                : () => invocation.Proceed();

            action();
        }

        #region Fields

        private IEnumerable<Type> _returnTypestoIntercept;
        private IEnumerable<ILogger> _loggers;
        private IEventPublisher _eventPublisher;

        #endregion

        #region Properties

        protected virtual IEnumerable<ILogger> Loggers
        {
            get { return _loggers ?? (_loggers = EngineContext.Current.ResolveAll<ILogger>()); }
        }

        protected virtual IEventPublisher EventPublisher
        {
            get { return _eventPublisher ?? (_eventPublisher = EngineContext.Current.Resolve<IEventPublisher>()); }
        }

        public IEnumerable<Type> ReturnTypesToIntercept
        {
            get { return _returnTypestoIntercept ?? (_returnTypestoIntercept = LoadReturnTypestoIntercept()); }
        }

        protected virtual IEnumerable<Type> LoadReturnTypestoIntercept()
        {
            return new[]
            {
                typeof (RequestBase<>),
                typeof (RequestBase)
            };
        }

        protected virtual IEnumerable<IPreInvocationAspect> PreInvocationAspects
        {
            get { return GetSingletonInvocationAspect<IPreInvocationAspect>(); }
        }

        protected virtual IEnumerable<IPostInvocationAspect> PostInvocationAspects
        {
            get { return GetSingletonInvocationAspect<IPostInvocationAspect>(); }
        }

        protected virtual IEnumerable<IInvocationAspect> InvocationAspects
        {
            get { return GetSingletonInvocationAspect<IInvocationAspect>(); }
        }

        public virtual IEnumerable<IAspect> Aspects
        {
            get
            {
                return Singleton<IEnumerable<IAspect>>.Instance ??
                       (Singleton<IEnumerable<IAspect>>.Instance = GetSystemAspects());
            }
        }

        public IEnumerable<MethodInfo> MethodsToIntercept { get; set; }

        public IEnumerable<Type> TypesToIntercept { get; set; }

        #endregion

        #region Utilities

        protected virtual void RunInterceptionFlow(IInvocation invocation, object invocationId)
        {
            var aspectRequest = BuildAspectMessageFromInvocation(invocation, invocationId);
            Loggers.Log(l => l.Information("Start interception flow on {0}".AsFormat(invocation.Method.Name)));
            ExecutePreInvocationAspects(aspectRequest);
            Invoke(aspectRequest);
            ExecutePostInvocationAspects(aspectRequest);
            Loggers.Log(l => l.Information("Finish interception flow for {0}".AsFormat(invocation.Method.Name)));
        }

        private void ExecutePreInvocationAspects(AspectMessage aspectRequest)
        {
            PreInvocationAspects.OrderBy(a => a.Order).ForEachItem(a =>
            {
                Loggers.Log(l => l.Information("Execute PRE-INVOCATION aspect of type: {0}".AsFormat(l.ToString())));
                a.Action(aspectRequest);
            });
        }

        private void ExecutePostInvocationAspects(AspectMessage aspectRequest)
        {
            PostInvocationAspects.OrderBy(a => a.Order).ForEachItem(a =>
            {
                Loggers.Log(l => l.Information("Execute POST-INVOCATION aspect of type: {0}".AsFormat(l.ToString())));
                a.Action(aspectRequest);
            });
        }

        protected virtual void Invoke(AspectMessage aspectRequest)
        {
            aspectRequest.InvocationStartOn = DateTime.Now;
            ExecuteInvocationAspects(aspectRequest);
            aspectRequest.InvocationEndOn = DateTime.Now;
        }

        private void ExecuteInvocationAspects(AspectMessage aspectRequest)
        {
            InvocationAspects.OrderBy(a => a.Order).ForEachItem(a =>
            {
                Loggers.Log(l => l.Information("Execute INVOCATION aspects of type: {0}".AsFormat(l.ToString())));
                a.Action(aspectRequest);

                EventPublisher.Publish(
                    new ActivityResponseCreatedEvent
                    {
                        ActivityReponse = aspectRequest.Response
                    });
            });
        }

        protected virtual bool ShouldBeIntercepted(MethodInfo methodInfo)
        {
            return ReturnTypesToIntercept.Any(rt => rt.IsAssignableFrom(methodInfo.ReturnType));
        }

        protected virtual AspectMessage BuildAspectMessageFromInvocation(IInvocation invocation, object invocationId)
        {
            Guard.NotNull(invocation);

            invocation.Proceed();
            var request = invocation.ReturnValue as RequestBase;

            ArrangeValidationPoints(request, invocation);
            ArrangeSubRequests(request);
            return new AspectMessage
            {
                InvocationId = invocationId,
                CreatedOn = DateTime.Now,
                Invocation = invocation,
                Request = request,
                Response = new ActivityResponse
                {
                    Request = request
                }
            };
        }

        protected virtual void ArrangeValidationPoints(RequestBase request, IInvocation invocation)
        {
            var mInfo = invocation.GetConcreteMethod();

            //Get all validation keys
            var requestValidationKeys = new HashSet<string> {mInfo.Name};
            GetRequestValidationKeys(mInfo).ForEachItem(k => requestValidationKeys.Add(k));

            AddValidationPointsByRequestValidationKeys(request, requestValidationKeys);
        }

        protected virtual void ArrangeValidationPoints(RequestBase request)
        {
            var vKeys = new HashSet<string>();
            request.ValidationKeys.ForEachItem(vka => vKeys.Add(vka));

            //Todo: we cannot discover thevalidationkeys - the next line is broken
            var vkAtts = request.GetType().GetCustomAttributes<ValidationKeyAttribute>()
                .Select(vka => vka.Key);
            vkAtts.ForEachItem(vka => vKeys.Add(vka));

            AddValidationPointsByRequestValidationKeys(request, vKeys);
        }

        protected virtual IEnumerable<string> GetRequestValidationKeys(MethodInfo mInfo)
        {
            var vKeys = mInfo.GetCustomAttributes<ValidationKeyAttribute>().Select(vka => vka.Key.ToLower());
            return vKeys;
        }

        protected virtual void AddValidationPointsByRequestValidationKeys(RequestBase request,
            IEnumerable<string> requestValidationKeys)
        {
//locate validation funcs across the solution
            var tFinder = EngineContext.Current.Resolve<ITypeFinder>();
            var allValidationMethods = tFinder.FindMethodsOfAttribute<ValidatorAttribute>();

            var requestValidationPoints = allValidationMethods
                .Where(vm => vm.GetCustomAttributes<ValidatorAttribute>()
                    .Any(va => requestValidationKeys.Contains(va.ValidationKey)));

            var result = new List<Func<ValidationPoint>>();
            if (request.ValidationPoints.NotEmpty())
                result.AddRange(request.ValidationPoints);

            requestValidationPoints.ForEachItem(rvp =>
            {
                var instance = Activator.CreateInstance(rvp.DeclaringType);
                Guard.NotNull(instance);
                var mResult = rvp.Invoke(instance, new[] {DynamicHelper.ToPropertyDictionary(request.ValidationData)});

                var vPoints = (IEnumerable<Func<ValidationPoint>>) mResult;
                Guard.NotNull(vPoints);
                result.AddRange(vPoints);
            });

            request.ValidationPoints = result;
        }

        protected virtual void ArrangeSubRequests(RequestBase request)
        {
            Guard.NotNull(request);

            if (request.Setup.NotNull())
            {
                request.Setup.ParentRequest = request;
                ArrangeValidationPoints(request.Setup);
            }

            if (request.TearDown.NotNull())
            {
                request.TearDown.ParentRequest = request;
                ArrangeValidationPoints(request.TearDown);
            }
        }

        protected internal IEnumerable<IAspect> GetSystemAspects()
        {
            var aspectInstances = new List<IAspect>();
            var aspectTypes = EngineContext.Current.Resolve<ITypeFinder>().FindClassesOfType<IAspect>();
            aspectTypes.ForEachItem(aType => aspectInstances.Add(Activator.CreateInstance(aType) as IAspect));

            return aspectInstances;
        }

        protected internal IEnumerable<TAspect> GetRuntimeAspects<TAspect>() where TAspect : IAspect
        {
            return Aspects.IsNull()
                ? new List<TAspect>()
                : Aspects.OfType<TAspect>();
        }

        private IEnumerable<TAspect> GetSingletonInvocationAspect<TAspect>() where TAspect : IAspect
        {
            return Singleton<IEnumerable<TAspect>>.Instance ??
                   (Singleton<IEnumerable<TAspect>>.Instance =
                       GetRuntimeAspects<TAspect>());
        }

        private object GenerateInvocationId()
        {
            return DateTime.Now.ToTimeStamp();
        }

        #endregion
    }
}