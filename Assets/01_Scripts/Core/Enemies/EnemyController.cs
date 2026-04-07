using _01_Scripts.Core.Scoring;
using UnityEngine;

namespace _01_Scripts.Core.Enemies
{
    [RequireComponent(typeof(HealthManager), typeof(Animator))]
    public class EnemyController : MonoBehaviour
    { 
        [Header("Enemy Details")]
        [Tooltip("The number of highscore points, this type of Enemy is worth")]
        [SerializeField] private int pointValue = 10;
        
        [Tooltip("The specific bone/transform that moves downward during the death animation")]
        [SerializeField] private GameObject centerMassBone;
        
        [Header("Combat Physics")]
        [Tooltip("How much physical force is applied when hit")]
        [SerializeField] private float knockbackForce = 15f;
        
        [Header("Explosive Payload")]
        [Tooltip("The particle system prefab to spawn upon destruction")]
        [SerializeField] private GameObject explosionVfxPrefab;
        
        private HealthManager _healthManager;
        private Animator _animator;
        private Rigidbody _rb;
        private static readonly int DeathTrigger = Animator.StringToHash("Die");

        private void Awake() {
            _healthManager = GetComponent<HealthManager>();
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
        }

        private void OnEnable() {
            _healthManager.OnHealthChanged += HandleHit;
            _healthManager.OnZeroHealth += HandleDeath;
        }

        private void OnDisable() {
            _healthManager.OnHealthChanged -= HandleHit;
            _healthManager.OnZeroHealth -= HandleDeath;
        }

        private void HandleHit(int currentHealth, int maxHealth, GameObject shooter) {
            if (currentHealth <= 0 || !shooter) 
                return;
            
            Vector3 knockbackDirection = (centerMassBone.transform.position - shooter.transform.position).normalized;
            knockbackDirection.y = 0;
            _rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
            // TODO trigger particle effects or sound here
        }

        private void HandleDeath(GameObject killer) {
            _animator.SetTrigger(DeathTrigger); 
            
            if (killer && killer.TryGetComponent<PlayerScore>(out var scoreComponent)) {
                scoreComponent.AddScore(pointValue);
            }
            
            GetComponent<Collider>().enabled = false;
            _rb.isKinematic = true; 
            _rb.useGravity = false;
        }

        private void DetonatePayload() {
            if (explosionVfxPrefab) {
                Vector3 detonationPoint = centerMassBone ? centerMassBone.transform.position : transform.position;
                Instantiate(explosionVfxPrefab, detonationPoint, Quaternion.identity);
            }
            
            Destroy(gameObject); 
            // TODO: Use Object Pool pattern for enemies combined with dynamic spawning... Release them here instead of Destroy
        }
    }
}

