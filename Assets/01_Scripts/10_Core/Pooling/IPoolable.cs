using System;
using UnityEngine;

namespace _01_Scripts._10_Core.Pooling
{
    public interface IPoolable
    {
        GameObject gameObject { get; } 
        
        /// <summary>
        /// Links the object to its specific release command in the pool manager.
        /// </summary>
        void Initialize(Action<IPoolable> returnAction);
        
        /// <summary>
        /// Called automatically when the object is pulled from the armory.
        /// </summary>
        void OnSpawned();
        
        /// <summary>
        /// Called automatically when the object is returned to the armory.
        /// </summary>
        void OnDespawned();
    }
}