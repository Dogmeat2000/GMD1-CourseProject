using UnityEngine;
using UnityEngine.Events;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Settings;
using _01_Scripts.Core.Utilities;
using _01_Scripts.Turrets;

namespace _01_Scripts.Core.Combat
{
    /// <summary>
    /// Handles the logic and acoustic/visual feedback related to warheads and explosives.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ImpactWarhead : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The composite VFX prefab to spawn upon detonation.")]
        [SerializeField] 
        private GameObject explosionVfxPrefab;
        
        [Tooltip("Which layers trigger the detonation? (e.g., PlayerShip, Structures)")]
        [SerializeField] 
        private LayerMask validTargetLayers;
        
        [Tooltip("Event triggered upon detonation.")]
        [SerializeField] 
        private UnityEvent onDetonate;
        
        public GameObject Instigator { get; set; }
        public int PayloadDamage { get; set; }
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
            
            if (targetHealth != null) {
                Detonate(targetHealth);
            }
        }

        private void Detonate(IDamageable target) {
            LevelSettings settings = ServiceLocator.Get<LevelManager>().Settings;
            
            _hasDetonated = true;
            target.TakeDamage(PayloadDamage, Instigator ? Instigator : gameObject);
            if (explosionVfxPrefab) {
                UniversalPoolService.Instance.Spawn(explosionVfxPrefab, transform.position, Quaternion.identity, settings.DefaultObjectPoolSize , settings.MaxDefaultObjectPoolSize);
            }
            
            onDetonate?.Invoke();
            
            if (TryGetComponent(out IProjectile projectile)) {
                projectile.ReturnToPool();
            } else {
                Destroy(gameObject);
            }
        }
    }
}