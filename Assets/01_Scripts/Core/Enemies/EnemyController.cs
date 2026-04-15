using System;
using _01_Scripts.Core.Combat;
using _01_Scripts.Core.Scoring;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Settings;
using UnityEngine;

namespace _01_Scripts.Core.Enemies
{
    [RequireComponent(typeof(HealthManager), typeof(Animator))]
    public class EnemyController : MonoBehaviour, IPoolable
    { 
        [Header("Enemy Details")]
        [Tooltip("The number of highscore points, this type of Enemy is worth")]
        [SerializeField] 
        private int pointValue = 10;
        
        [Tooltip("The specific bone/transform that moves downward during the death animation")]
        [SerializeField] 
        private GameObject centerMassBone;
        
        [Tooltip("Optional: A particle system to spawn when the death animation finishes (e.g., explosion, digital fade, blood splatter).")]
        [SerializeField]
        private GameObject deathVfxPrefab;
        
        [Header("Combat Physics")]
        [Tooltip("How much physical force is applied when hit")]
        [SerializeField] 
        private float knockbackForce = 15f;
        
        [Tooltip("The base damage this unit inflicts on collision. Scales with difficulty.")]
        [SerializeField] 
        private int baseCollisionDamage = 25;
        
        [Tooltip("The warhead component responsible for delivering the damage payload upon impact.")]
        [SerializeField] 
        private ImpactWarhead warhead;
        
        public event Action<EnemyController> OnRemovedFromBoard;
        private Action<IPoolable> _returnToPoolCommand;
        private Collider _collider;
        
        private HealthManager _healthManager;
        private Animator _animator;
        private Rigidbody _rb;
        private static readonly int DeathTrigger = Animator.StringToHash("Die");
        private IEntityBrain _brain;

        private void Awake() {
            _healthManager = GetComponent<HealthManager>();
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _brain = GetComponent<IEntityBrain>();
        }

        private void OnEnable() {
            _healthManager.OnHealthChanged += HandleHit;
            _healthManager.OnZeroHealth += HandleDeath;
        }

        private void OnDisable() {
            _healthManager.OnHealthChanged -= HandleHit;
            _healthManager.OnZeroHealth -= HandleDeath;
        }

        public void Initialize(Action<IPoolable> returnAction) {
            _returnToPoolCommand = returnAction;
        }

        public void OnSpawned() {
            _healthManager.ResetHealth();
            _collider.enabled = true;
            _rb.isKinematic = false;
            _rb.useGravity = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            if (warhead && LevelManager.Instance) {
                GameDifficulty currentDiff = LevelManager.Instance.CurrentDifficulty;
                float difficultyMultiplier = LevelManager.Instance.Settings.GetDifficultyMultiplier(currentDiff);
                warhead.PayloadDamage = Mathf.RoundToInt(baseCollisionDamage * difficultyMultiplier);
            }
            
            _brain?.WakeUp();
        }

        public void OnDespawned() {
            // TODO: Anything I need to clean up after enemy death?
        }
        
        private void HandleHit(int currentHealth, int maxHealth, GameObject shooter) {
            if (currentHealth <= 0 || !shooter) 
                return;
            
            Vector3 knockbackDirection = (centerMassBone.transform.position - shooter.transform.position).normalized;
            knockbackDirection.y = 0;
            _rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }

        private void HandleDeath(HealthManager source, GameObject killer) {
            _animator.SetTrigger(DeathTrigger); 
            
            if (killer && killer.TryGetComponent<PlayerScore>(out var scoreComponent)) {
                scoreComponent.AddScore(pointValue);
            }
            
            GetComponent<Collider>().enabled = false;
            _rb.isKinematic = true; 
            _rb.useGravity = false;
            
            _brain?.ShutDown();
        }

        /** <summary>
         * Called by the ImpactWarhead UnityEvent after it successfully strikes a target.
         * The warhead handles its own VFX, so this simply removes the drone from the board.
         * </summary>
         */
        public void DespawnRoutine() {
            OnRemovedFromBoard?.Invoke(this);
            
            if (_returnToPoolCommand != null) {
                _returnToPoolCommand.Invoke(this);
            } else {
                Destroy(gameObject);
            }
        }
        
        /** <summary>
         * Called by an Animation Event on the final frame of the Death animation.
         * Handles spawning any optional pooled VFX before removing the entity.
         * </summary>
         */
        public void FinalizeDeathSequence() {
            if (deathVfxPrefab && UniversalPoolService.Instance) {
                Vector3 spawnPoint = centerMassBone ? centerMassBone.transform.position : transform.position;
                UniversalPoolService.Instance.Spawn(deathVfxPrefab, spawnPoint, Quaternion.identity);
            }
            
            DespawnRoutine();
        }
    }
}

