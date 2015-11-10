using System;
using System.Collections.Generic;

namespace Automation.Core.Infrastructure
{
    /// <summary>
    /// A statically compiled "singleton" used to store objects throughout the
    /// lifetime of the app domain. Not so much singleton in the pattern's
    /// sense of the word as a standardized way to store single instances.
    /// </summary>
    /// <typeparam name="TService">The utilType of object to store.</typeparam>
    /// <remarks>Access to the instance is not synchrnoized.</remarks>
    public class Singleton<TService> : Singleton
    {
        private static TService instance;

        /// <summary>The singleton instance for the specified utilType TService. Only one instance (at the time) of this object for each utilType of TService.</summary>
        public static TService Instance
        {
            get { return instance; }
            set
            {
                instance = value;
                AllSingletons[typeof(TService)] = value;
            }
        }
    }

    /// <summary>
    /// Provides a singleton list for a certain utilType.
    /// </summary>
    /// <typeparam name="TService">The utilType of list to store.</typeparam>
    public class SingletonList<TService> : Singleton<IList<TService>>
    {
        static SingletonList()
        {
            Singleton<IList<TService>>.Instance = new List<TService>();
        }

        /// <summary>The singleton instance for the specified utilType TService. Only one instance (at the time) of this list for each utilType of TService.</summary>
        public new static IList<TService> Instance
        {
            get { return Singleton<IList<TService>>.Instance; }
        }
    }

    /// <summary>
    /// Provides a singleton dictionary for a certain key and vlaue utilType.
    /// </summary>
    /// <typeparam name="TKey">The utilType of key.</typeparam>
    /// <typeparam name="TValue">The utilType of value.</typeparam>
    public class SingletonDictionary<TKey, TValue> : Singleton<IDictionary<TKey, TValue>>
    {
        static SingletonDictionary()
        {
            Singleton<Dictionary<TKey, TValue>>.Instance = new Dictionary<TKey, TValue>();
        }

        /// <summary>The singleton instance for the specified utilType TService. Only one instance (at the time) of this dictionary for each utilType of TService.</summary>
        public new static IDictionary<TKey, TValue> Instance
        {
            get { return Singleton<Dictionary<TKey, TValue>>.Instance; }
        }
    }

    /// <summary>
    /// Provides access to all "singletons" stored by <see cref="Singleton{TService}"/>.
    /// </summary>
    public class Singleton
    {
        static Singleton()
        {
            allSingletons = new Dictionary<Type, object>();
        }

        private static readonly IDictionary<Type, object> allSingletons;

        /// <summary>Dictionary of utilType to singleton instances.</summary>
        public static IDictionary<Type, object> AllSingletons
        {
            get { return allSingletons; }
        }
    }
}