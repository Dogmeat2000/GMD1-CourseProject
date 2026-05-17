using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using _01_Scripts.Core.Interfaces;

namespace _01_Scripts.Core.Services
{
    public class UniversalPoolService : MonoBehaviour
    {
        public static UniversalPoolService Instance { get; private set; }
        
        private readonly Dictionary<int, IObjectPool<IPoolable>> _pools = new();

        private void Awake() {
            if (Instance && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Requests an object from the pool associated with the given prefab.
        /// </summary>
        public IPoolable Spawn(GameObject prefab, Vector3 position, Quaternion rotation, int poolCapacity, int maxCapacity) {
            int prefabId = prefab.GetInstanceID();
            
            if (!_pools.ContainsKey(prefabId)) {
                _pools[prefabId] = CreateNewPool(prefab, poolCapacity, maxCapacity);
            }

            IPoolable pooledObj = _pools[prefabId].Get();
            pooledObj.gameObject.transform.SetPositionAndRotation(position, rotation);
            
            return pooledObj;
        }

        private IObjectPool<IPoolable> CreateNewPool(GameObject prefab, int poolCapacity, int maxCapacity) {
            return new ObjectPool<IPoolable>(
                createFunc: () => {
                    GameObject instance = Instantiate(prefab, transform);
                    IPoolable poolable = instance.GetComponent<IPoolable>();
                    
                    poolable.Initialize((obj) => _pools[prefab.GetInstanceID()].Release(obj));
                    return poolable;
                },
                actionOnGet: (obj) => {
                    obj.gameObject.SetActive(true);
                    obj.OnSpawned();
                },
                actionOnRelease: (obj) => {
                    obj.OnDespawned();
                    obj.gameObject.SetActive(false);
                },
                actionOnDestroy: (obj) => Destroy(obj.gameObject),
                collectionCheck: false,
                defaultCapacity: poolCapacity,
                maxSize: maxCapacity
            );
        }
    }
}