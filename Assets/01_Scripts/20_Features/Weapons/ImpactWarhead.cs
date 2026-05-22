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
    /// Handles the logic and acoustic/visual feedback related to warheads and explosives.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ImpactWarhead : Warhead
    {
        [Header("Configuration")]
        [Tooltip("Event triggered upon detonation.")]
        [SerializeField] private UnityEvent onDetonate;
        
        private bool _hasDetonated;

        private void OnEnable() {
            _hasDetonated = false;
        }

        private void OnTriggerEnter(Collider other) {
            if (_hasDetonated) 
                return;
            
            if (!validTargetLayers.Contains(other.gameObject.layer))
                return; 
            
            IDamageable targetHealth = other.GetComponentInParent<IDamageable>();
            
            if (targetHealth != null)
                Detonate(targetHealth);
        }
        
        private void OnCollisionEnter(Collision collision) {
            if (_hasDetonated) 
                return;
            
            if (!validTargetLayers.Contains(collision.gameObject.layer))
                return; 
            
            IDamageable targetHealth = collision.collider.GetComponentInParent<IDamageable>();
            
            if (targetHealth != null)
                Detonate(targetHealth);
        }

        private void Detonate(IDamageable target) {
            LevelSettings settings = ServiceLocator.Get<LevelManager>().Settings;
            
            _hasDetonated = true;
            target.TakeDamage(ImpactAmount, Instigator ? Instigator : gameObject);
            
            if (explosionVfxPrefab)
                UniversalPoolService.Instance.Spawn(explosionVfxPrefab, transform.position, Quaternion.identity, settings.DefaultObjectPoolSize , settings.MaxDefaultObjectPoolSize);
            
            onDetonate?.Invoke();
            
            if (TryGetComponent(out IProjectile projectile)) {
                projectile.ReturnToPool();
            } else if (!TryGetComponent(out IPoolable pooledEntity)) {
                Destroy(gameObject);
            }
        }
    }
}