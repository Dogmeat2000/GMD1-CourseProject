using UnityEngine;
using UnityEngine.Events;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Utilities;

namespace _01_Scripts.Core.Combat
{
    [RequireComponent(typeof(Collider))]
    public class ImpactWarhead : MonoBehaviour
    {
        [Header("Ordnance Configuration")]
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

        private void OnCollisionEnter(Collision collision) {
            if (_hasDetonated) 
                return;
            
            if (!validTargetLayers.Contains(collision.gameObject.layer))
                return; 
            
            IDamageable targetHealth = collision.collider.GetComponentInParent<IDamageable>();
            
            if (targetHealth != null) {
                Detonate(targetHealth);
            }
        }

        private void Detonate(IDamageable target) {
            _hasDetonated = true;
            target.TakeDamage(PayloadDamage, Instigator ? Instigator : gameObject);
            if (explosionVfxPrefab) {
                UniversalPoolService.Instance.Spawn(explosionVfxPrefab, transform.position, Quaternion.identity);
            }
            onDetonate?.Invoke();
        }
    }
}