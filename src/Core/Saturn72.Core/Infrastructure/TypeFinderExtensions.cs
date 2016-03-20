using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Saturn72.Extensions;

namespace Saturn72.Core.Infrastructure
{
    public static class TypeFinderExtensions
    {
        /// <summary>
        /// Gets all assemblies containing reuired type
        /// </summary>
        /// <typeparam name="TType">Type to scan</typeparam>
        /// <returns><see cref="IEnumerable{T}"/></returns>
        public static IEnumerable<Assembly> FindAssembliesWithTypeDerivatives<TType>(this ITypeFinder typeFinder)
        {
            var types = typeFinder.FindClassesOfType<TType>();

            return types.Select(x => x.Assembly).Distinct().ToArray();
        }

        /// <summary>
        /// Finds all types of TService and run action
        /// </summary>
        /// <typeparam name="TService">The Service</typeparam>
        /// <param name="typeFinder">Type finder <see cref="ITypeFinder"/></param>
        /// <param name="action">Action to run on type</param>
        public static void FindClassesOfTypeAndRunMethod<TService>(this ITypeFinder typeFinder,Action<TService> action)
        {
            FindClassesOfTypeAndRunMethod(typeFinder, action, null);
        }

        /// <summary>
        /// Finds all types of TService and run action
        /// </summary>
        /// <typeparam name="TService">The Service</typeparam>
        /// <param name="typeFinder">Type finder <see cref="ITypeFinder"/></param>
        /// <param name="action">Action to run on type</param>
        /// <param name="orderedBy">Execution order</param>
        public static void FindClassesOfTypeAndRunMethod<TService>(this ITypeFinder typeFinder, Action<TService> action, Func<TService, int> orderedBy)
        {
            var allSeriveInstances = typeFinder.FindClassesOfType<TService>();

            var serviceList = allSeriveInstances.Select(s => (TService) Activator.CreateInstance(s)).ToList();

            if(orderedBy.NotNull())//sort
                serviceList = serviceList.OrderBy(orderedBy).ToList();

            foreach (var service in serviceList)
                action(service);
        }
    }
}