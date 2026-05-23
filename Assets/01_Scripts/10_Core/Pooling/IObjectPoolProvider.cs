using _01_Scripts._10_Core.DependencyInjection;
using UnityEngine;

namespace _01_Scripts._10_Core.Pooling
{
    public interface IObjectPoolProvider : IService
    {
        /// <summary>
        /// Requests an object from the pool associated with the given prefab.
        /// </summary>
        public IPoolable Spawn(GameObject prefab, Vector3 position, Quaternion rotation, int poolCapacity, int maxCapacity);
    }
}
