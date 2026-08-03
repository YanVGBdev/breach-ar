using UnityEngine;
using System;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// Simplified service provider for dependency injection
    /// </summary>
    public class ServiceProvider : MonoBehaviour
    {
        public static ServiceProvider Instance { get; private set; }

        private Dictionary<Type, object> services;
        private Dictionary<Type, object> singletons;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            services = new Dictionary<Type, object>();
            singletons = new Dictionary<Type, object>();
        }

        /// <summary>
        /// Register a service
        /// </summary>
        public void Register<T>(T service, bool singleton = false) where T : class
        {
            Type type = typeof(T);

            if (singleton)
            {
                singletons[type] = service;
            }
            else
            {
                services[type] = service;
            }

            Debug.Log($"[ServiceProvider] Registered: {type.Name}");
        }

        /// <summary>
        /// Get a service
        /// </summary>
        public T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (singletons.TryGetValue(type, out object singletonService))
            {
                return singletonService as T;
            }

            if (services.TryGetValue(type, out object service))
            {
                return service as T;
            }

            Debug.LogWarning($"[ServiceProvider] Service not found: {type.Name}");
            return null;
        }

        /// <summary>
        /// Check if service is registered
        /// </summary>
        public bool Has<T>() where T : class
        {
            Type type = typeof(T);
            return singletons.ContainsKey(type) || services.ContainsKey(type);
        }

        /// <summary>
        /// Unregister a service
        /// </summary>
        public void Unregister<T>() where T : class
        {
            Type type = typeof(T);
            services.Remove(type);
            singletons.Remove(type);
        }

        /// <summary>
        /// Clear all services
        /// </summary>
        public void Clear()
        {
            services.Clear();
            singletons.Clear();
        }
    }

    /// <summary>
    /// Attribute to mark a class as a service
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public bool Singleton { get; }

        public ServiceAttribute(bool singleton = false)
        {
            Singleton = singleton;
        }
    }

    /// <summary>
    /// Extension methods for ServiceProvider
    /// </summary>
    public static class ServiceProviderExtensions
    {
        /// <summary>
        /// Get service or create default
        /// </summary>
        public static T GetOrCreate<T>(this ServiceProvider provider) where T : class, new()
        {
            T service = provider.Get<T>();
            if (service == null)
            {
                service = new T();
                provider.Register(service);
            }
            return service;
        }

        /// <summary>
        /// Get service or throw
        /// </summary>
        public static T GetRequired<T>(this ServiceProvider provider) where T : class
        {
            T service = provider.Get<T>();
            if (service == null)
            {
                throw new InvalidOperationException($"Required service not found: {typeof(T).Name}");
            }
            return service;
        }
    }
}
