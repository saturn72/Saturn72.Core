using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Autofac;
using Autofac.Core.Lifetime;
using Autofac.Integration.Mvc;

namespace Saturn72.Core.Infrastructure.DependencyManagement
{
    public class ContainerManager
    {
        #region Fields

        private readonly IContainer _container;

        #endregion Fields

        #region ctor

        public ContainerManager(IContainer container)
        {
            _container = container;
        }

        #endregion ctor

        #region Properties

        public IContainer Container => _container;

        #endregion Properties

        public virtual TService Resolve<TService>(object key = null, ILifetimeScope scope = null) where TService : class
        {
            if (scope == null)
                scope = Scope();

            return key == null
                ? scope.Resolve<TService>()
                : scope.ResolveKeyed<TService>(key);
        }

        public virtual object Resolve(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.Resolve(type);
        }

        public virtual TService[] ResolveAll<TService>(object key = null, ILifetimeScope scope = null)
            where TService : class
        {
            if (scope == null)
                scope = Scope();

            return (key == null
                ? scope.Resolve<IEnumerable<TService>>()
                : scope.ResolveKeyed<IEnumerable<TService>>(key)).ToArray();
        }

        public virtual TService ResolveUnregistered<TService>(ILifetimeScope scope = null) where TService : class
        {
            return ResolveUnregistered(typeof (TService), scope) as TService;
        }

        public virtual object ResolveUnregistered(Type type, ILifetimeScope scope = null)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            var constructors = type.GetConstructors();
            foreach (var constructor in constructors)
            {
                try
                {
                    var parameters = constructor.GetParameters();
                    var parameterInstances = new List<object>();
                    foreach (var parameter in parameters)
                    {
                        var service = Resolve(parameter.ParameterType, scope);
                        if (service == null) throw new Saturn72Exception("Unkown dependency");
                        parameterInstances.Add(service);
                    }
                    return Activator.CreateInstance(type, parameterInstances.ToArray());
                }
                catch (Saturn72Exception)
                {
                }
            }
            throw new Saturn72Exception("No contructor was found that had all the dependencies satisfied.");
        }

        public virtual bool TryResolve(Type serviceType, ILifetimeScope scope, out object instance)
        {
            if (scope == null)
            {
                //no scope specified
                scope = Scope();
            }
            return scope.TryResolve(serviceType, out instance);
        }

        public virtual ILifetimeScope Scope()
        {
            try
            {
                if (HttpContext.Current != null)
                    return AutofacDependencyResolver.Current.RequestLifetimeScope;

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
            catch (Exception)
            {
                //we can get an exception here if RequestLifetimeScope is already disposed
                //for example, requested in or after "Application_EndRequest" handler
                //but note that usually it should never happen

                //when such lifetime scope is returned, you should be sure that it'll be disposed once used (e.g. in schedule tasks)
                return Container.BeginLifetimeScope(MatchingScopeLifetimeTags.RequestLifetimeScopeTag);
            }
        }
    }
}