﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Saturn72.Core.Infrastructure;

namespace Saturn72.Core.Tests.Fakes
{
    public class FakeTypeFinder : ITypeFinder
    {
        public FakeTypeFinder(Assembly assembly, params Type[] types)
        {
            Assemblies = new[] {assembly};
            Types = types;
        }

        public FakeTypeFinder(params Type[] types)
        {
            Assemblies = new Assembly[0];
            Types = types;
        }

        public FakeTypeFinder(params Assembly[] assemblies)
        {
            Assemblies = assemblies;
        }

        public Assembly[] Assemblies { get; set; }
        public Type[] Types { get; set; }

        public IList<Assembly> GetAssemblies()
        {
            return Assemblies.ToList();
        }


        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, bool onlyConcreteClasses = true)
        {
            return (from t in Types
                where
                    !t.IsInterface && assignTypeFrom.IsAssignableFrom(t) &&
                    (!onlyConcreteClasses || (t.IsClass && !t.IsAbstract))
                select t).ToList();
        }

        public IEnumerable<Type> FindClassesOfType<T>(bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof (T), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType(Type assignTypeFrom, IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(assignTypeFrom, onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfType<T>(IEnumerable<Assembly> assemblies, bool onlyConcreteClasses = true)
        {
            return FindClassesOfType(typeof (T), onlyConcreteClasses);
        }

        public IEnumerable<Type> FindClassesOfAttribute<TAttribute>(bool onlyConcreteClasses = true)
            where TAttribute : Attribute
        {
            return new Type[] {};
        }

        public IEnumerable<Type> FindClassesOfAttribute<TAttribute>(IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true) where TAttribute : Attribute
        {
            return FindClassesOfAttribute<TAttribute>();
        }

        public IEnumerable<MethodInfo> FindMethodsOfReturnType<TReturnType>(bool onlyConcreteClasses = true)
        {
            return FindMethodsOfReturnType(typeof (TReturnType), onlyConcreteClasses);
        }

        public IEnumerable<MethodInfo> FindMethodsOfReturnType(Type returnType, bool onlyConcreteClasses = true)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MethodInfo> FindMethodsOfReturnType(Type returnType, IEnumerable<Assembly> assemblies,
            bool onlyConcreteClasses = true)
        {
            return FindMethodsOfReturnType(returnType, onlyConcreteClasses);
        }

        public IEnumerable<MethodInfo> FindMethodsOfAttribute<TAttribute>() where TAttribute : Attribute
        {
            throw new NotImplementedException();
        }

        public IEnumerable<MethodInfo> FindMethodsOfAttribute<TAttribute>(IEnumerable<Assembly> assemblies)
            where TAttribute : Attribute
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Assembly> FindAssembliesWithTypeDerivatives<TType>()
        {
            throw new NotImplementedException();
        }
    }
}