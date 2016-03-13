using System;
using System.Linq;
using Saturn72.Extensions;

namespace Saturn72.Core.Infrastructure
{
    public static class TypeFinderExtensions
    {
        /// <summary>
        /// Finds all types of TService and run action
        /// </summary>
        /// <typeparam name="TService">The Service</typeparam>
        /// <param name="typeFinder">Type finder <see cref="ITypeFinder"/></param>
        /// <param name="action">Action to run on type</param>
        public static void FindTypeAndRunMethod<TService>(this ITypeFinder typeFinder,Action<TService> action)
        {
            FindTypeAndRunMethod(typeFinder, action, null);
        }

        /// <summary>
        /// Finds all types of TService and run action
        /// </summary>
        /// <typeparam name="TService">The Service</typeparam>
        /// <param name="typeFinder">Type finder <see cref="ITypeFinder"/></param>
        /// <param name="action">Action to run on type</param>
        /// <param name="orderedBy">Execution order</param>
        public static void FindTypeAndRunMethod<TService>(this ITypeFinder typeFinder, Action<TService> action, Func<TService, int> orderedBy)
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