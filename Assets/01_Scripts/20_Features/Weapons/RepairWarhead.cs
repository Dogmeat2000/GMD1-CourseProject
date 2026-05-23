using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._10_Core.Persistence;
using _01_Scripts._10_Core.Pooling;
using _01_Scripts._10_Core.Utilities;
using _01_Scripts._20_Features.Progression;
using _01_Scripts._20_Features.Vitals;
using UnityEngine;
using UnityEngine.Events;

namespace _01_Scripts._20_Features.Weapons
{
    /// <summary>
    /// Delivers a healing payload to target upon impact.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class RepairWarhead : Warhead
    {
        [Header("Configuration")]
        [Tooltip("Event triggered upon detonation.")]
        [SerializeField] private UnityEvent onDetonate;
        
        private IObjectPoolProvider _poolProvider;
        
        private bool _hasDetonated;

        private void OnEnable() {
            _poolProvider = ServiceLocator.Get<IObjectPoolProvider>();
            _hasDetonated = false;
        }

        private void OnTriggerEnter(Collider other) {
            if (_hasDetonated || !validTargetLayers.Contains(other.gameObject.layer)) 
                return;
            
            HealthManager targetHealth = other.GetComponentInParent<HealthManager>();
            
            if (targetHealth)
                Detonate(targetHealth);
        }
        
        private void OnCollisionEnter(Collision collision) {
            if (_hasDetonated || !validTargetLayers.Contains(collision.gameObject.layer)) 
                return;
            
            HealthManager targetHealth = collision.collider.GetComponentInParent<HealthManager>();
            
            if (targetHealth)
                Detonate(targetHealth);
        }

        private void Detonate(HealthManager target) {
            _hasDetonated = true;
            target.Heal(ImpactAmount);

            if (explosionVfxPrefab && _poolProvider != null) {
                LevelSettings settings = ServiceLocator.Get<ILevelManager>().Settings;
                _poolProvider.Spawn(explosionVfxPrefab, transform.position, Quaternion.identity, settings.DefaultObjectPoolSize, settings.MaxDefaultObjectPoolSize);
            }
            
            onDetonate?.Invoke();

            if (TryGetComponent(out IProjectile projectile)) {
                projectile.ReturnToPool();
            } else if (!TryGetComponent(out IPoolable pooledEntity)) {
                Destroy(gameObject);
            }
        }
    }
}
