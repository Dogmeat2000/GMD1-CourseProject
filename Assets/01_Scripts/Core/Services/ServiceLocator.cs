using System;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Core.Services
{
    /** <summary>
    * A lightweight Dependency Injection container that registers and provides access 
    * to level-specific services, preventing cross-scene Singleton contamination.
    * </summary>
    */
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new Dictionary<Type, object>();

        /** <summary>
        * Registers a service to the active level.
        * </summary>
        */
        public static void Register<T>(T service) {
            var type = typeof(T);
            if (!Services.TryAdd(type, service)) {
                Debug.LogWarning($"Service {type} is already registered. Overwriting.");
                Services[type] = service;
            }
        }

        /** <summary>
        * Retrieves a registered service from the network.
        * </summary>
        */
        public static T Get<T>() {
            var type = typeof(T);
            if (Services.TryGetValue(type, out var service)) {
                return (T)service;
            }
            Debug.LogError($"Service {type} requested but not found in ServiceLocator!");
            return default;
        }

        /** <summary>
        * Purges all services from memory. Called during scene teardown.
        * </summary>
        */
        public static void Clear() {
            Services.Clear();
        }
    }
}