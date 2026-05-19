using System;
using System.Collections.Generic;
using _01_Scripts.Core.Combat;
using _01_Scripts.Core.Scoring;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Settings;
using UnityEngine;

namespace _01_Scripts.Core.Enemies
{
    /// <summary>
    /// The central control system for hostile entities. Bridges physical movement, 
    /// combat telemetry, animations, and object pooling lifecycle management.
    /// </summary>
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
        
        [Tooltip("The exact name of the Animator state this entity should snap to upon spawning.")]
        [SerializeField] 
        private string defaultRespawnState = "Idle";
        
        [Tooltip("The exact name of the Animator trigger parameter to fire when health reaches zero.")]
        [SerializeField] 
        private string deathTriggerName = "Die";
        
        [Tooltip("List of any Animator Triggers (i.e. 'Hit' or 'Shoot') that must be aborted when this unit dies.")]
        [SerializeField] 
        private string[] interruptTriggersToPurge = { "Hit", "Shoot" };
        
        [Header("Combat Physics")]
        [Tooltip("How much physical force is applied when hit")]
        [SerializeField] 
        private float knockbackForce = 15f;
        
        [Tooltip("The base damage this unit inflicts on collision. Scales with difficulty.")]
        [SerializeField] 
        private int baseCollisionDamage = 25;
        
        [Tooltip("OPTIONAL: The warhead component responsible for delivering the damage payload upon impact.")]
        [SerializeField] 
        private ImpactWarhead warhead;
        
        public event Action<EnemyController> OnRemovedFromBoard;
        private Action<IPoolable> _returnToPoolCommand;
        private Collider _collider;
        private int _deathTriggerHash;
        private int[] _interruptHashes;
        
        private LevelManager _levelManager;
        
        private HealthManager _healthManager;
        private Animator _animator;
        private Rigidbody _rb;
        private IEntityBrain _brain;
        
        private struct TransformBlueprint {
            public Transform Child;
            public Vector3 LocalPos;
            public Quaternion LocalRot;
        }
        
        private readonly List<TransformBlueprint> _structuralBlueprint = new();
        
        private void Awake() {
            _levelManager = ServiceLocator.Get<LevelManager>();
            _healthManager = GetComponent<HealthManager>();
            _animator = GetComponent<Animator>();
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
            _brain = GetComponent<IEntityBrain>();
            _deathTriggerHash = Animator.StringToHash(deathTriggerName);
            
            Transform[] allChildren = GetComponentsInChildren<Transform>(true); 
            foreach (Transform child in allChildren) {
                if (child == transform) 
                    continue;
                
                _structuralBlueprint.Add(new TransformBlueprint {
                    Child = child,
                    LocalPos = child.localPosition,
                    LocalRot = child.localRotation
                });
            }
            
            _interruptHashes = new int[interruptTriggersToPurge.Length];
            for (int i = 0; i < interruptTriggersToPurge.Length; i++) {
                _interruptHashes[i] = Animator.StringToHash(interruptTriggersToPurge[i]);
            }
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
            if (_animator) {
                _animator.enabled = true;
                _animator.ResetTrigger(_deathTriggerHash);
                
                if (!string.IsNullOrEmpty(defaultRespawnState)) {
                    _animator.Play(defaultRespawnState, 0, 0f); 
                }
            }
            
            _healthManager.ResetHealth();
            _collider.enabled = true;
            _rb.isKinematic = false;
            _rb.useGravity = true;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            if (warhead && _levelManager) {
                GameDifficulty currentDiff = _levelManager.CurrentDifficulty;
                float difficultyMultiplier = _levelManager.Settings.GetDifficultyMultiplier(currentDiff);
                warhead.PayloadDamage = Mathf.RoundToInt(baseCollisionDamage * difficultyMultiplier);
            }
            
            _brain?.WakeUp();
        }

        public void OnDespawned() {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
        
        private void HandleHit(int currentHealth, int maxHealth, GameObject shooter) {
            if (currentHealth <= 0 || !shooter) 
                return;
            
            Vector3 knockbackDirection = (centerMassBone.transform.position - shooter.transform.position).normalized;
            knockbackDirection.y = 0;
            _rb.AddForce(knockbackDirection * knockbackForce, ForceMode.Impulse);
        }

        private void HandleDeath(HealthManager source, GameObject killer) {
            if (_animator) {
                foreach (int hash in _interruptHashes) {
                    _animator.ResetTrigger(hash);
                }
                
                _animator.SetTrigger(_deathTriggerHash); 
            }
            
            if (killer && killer.TryGetComponent<PlayerScore>(out var scoreComponent)) {
                scoreComponent.AddScore(pointValue);
            }
            
            GetComponent<Collider>().enabled = false;
            _rb.isKinematic = true; 
            _rb.useGravity = false;
            
            _brain?.ShutDown();
        }

        /// <summary>
        /// Called by the ImpactWarhead UnityEvent after it successfully strikes a target.
        /// The warhead handles its own VFX, so this simply removes the drone from the board.
        /// </summary>
        public void DespawnRoutine() {
            OnRemovedFromBoard?.Invoke(this);
            
            if (_animator) {
                _animator.Rebind();
                _animator.Update(0f);
            }
            
            foreach (var blueprint in _structuralBlueprint) {
                if (blueprint.Child) {
                    blueprint.Child.localPosition = blueprint.LocalPos;
                    blueprint.Child.localRotation = blueprint.LocalRot;
                }
            }
            
            if (_returnToPoolCommand != null) {
                _returnToPoolCommand.Invoke(this);
            } else {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Called by an Animation Event on the final frame of the Death animation.
        /// Handles spawning any optional pooled VFX before removing the entity.
        /// </summary>
        public void FinalizeDeathSequence() {
            LevelSettings settings = ServiceLocator.Get<LevelManager>().Settings;
            
            if (deathVfxPrefab && UniversalPoolService.Instance) {
                Vector3 spawnPoint = centerMassBone ? centerMassBone.transform.position : transform.position;
                UniversalPoolService.Instance.Spawn(deathVfxPrefab, spawnPoint, Quaternion.identity, settings.DefaultObjectPoolSize, settings.MaxDefaultObjectPoolSize);
            }
            
            DespawnRoutine();
        }
    }
}

