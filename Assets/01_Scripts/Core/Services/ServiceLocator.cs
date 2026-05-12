using System;
using System.Collections.Generic;
using _01_Scripts.Core.Interfaces;
using UnityEngine;

namespace _01_Scripts.Core.Services
{
    /// <summary>
    /// A lightweight Dependency Injection container that registers and provides access 
    /// to level-specific services, preventing cross-scene Singleton contamination.
    /// </summary>
    public static class ServiceLocator
    {
        private static Dictionary<Type, IService> _services = new();

        /// <summary>
        /// Instantiates the dictionary or purges residual data from the previous level.
        /// </summary>
        public static void Initialize() {
            if (_services == null) {
                _services = new Dictionary<Type, IService>();
            } else {
                _services.Clear();
            }
        }
        
        /// <summary>
        /// Registers a service to the active level.
        /// </summary>
        public static void Register(Type type, IService service){
            if (_services == null) {
                Debug.LogError("Attempted to register a service before Initialization!");
                return;
            }

            if (!_services.TryAdd(type, service)) {
                Debug.LogWarning($"Service of type {type.Name} is already registered. Overwriting.");
                _services[type] = service;
            }
        }

        /// <summary>
        /// Retrieves a registered service from the network.
        /// </summary>
        public static T Get<T>() where T : class, IService {
            if (_services == null) {
                Debug.LogError("ServiceLocator is offline.");
                return null;
            }

            Type type = typeof(T);
            if (_services.TryGetValue(type, out var service)) {
                return (T)service;
            }

            Debug.LogError($"The service {type.Name} was requested but not found in the registry!");
            return null;
        }

        /// <summary>
        /// Purges all services from memory. Called during scene teardown.
        /// </summary>
        public static void Clear() {
            _services.Clear();
        }
    }
}