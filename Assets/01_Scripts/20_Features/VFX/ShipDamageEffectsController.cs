using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._10_Core.Persistence;
using _01_Scripts._10_Core.Pooling;
using _01_Scripts._20_Features.Progression;
using _01_Scripts._20_Features.Targeting;
using _01_Scripts._20_Features.Vitals;
using _01_Scripts._20_Features.Weapons;
using JetBrains.Annotations;
using UnityEngine;

namespace _01_Scripts._20_Features.VFX
{
    /// <summary>
    /// Controller class responsible to handling how damage effects are applied to ships.
    /// </summary>
    [RequireComponent(typeof(HealthManager))]
    public class ShipDamageEffectsController : MonoBehaviour
    { 
        [Header("Damage Thresholds (Percentages)")]
        [Tooltip("A which damage level, the ship should begin displaying signs of light damage (0.8 = 80% health)")]
        [Range(0f, 1f)] 
        [SerializeField] private float lightSmokeThreshold = 0.80f;
        
        [Tooltip("A which damage level, the ship should begin displaying signs of medium damage (0.5 = 50% health)")]
        [Range(0f, 1f)] 
        [SerializeField] private float heavySmokeThreshold = 0.50f;
        
        [Tooltip("A which damage level, the ship should begin displaying signs of heavy damage (0.3 = 30% health)")]
        [Range(0f, 1f)] 
        [SerializeField] private float fireThreshold = 0.30f;

        [Header("Visual Effects")]
        [Tooltip("The Particle System VFX to use to indicate light damage")]
        [SerializeField] private ParticleSystem lightSmokeVfx;
        
        [Tooltip("The Particle System VFX to use to indicate medium damage")]
        [SerializeField] private ParticleSystem heavySmokeVfx;
        
        [Tooltip("The Particle System VFX to use to indicate heavy damage")]
        [SerializeField] private ParticleSystem fireVfx;
        
        [Tooltip("The VFX prefab to spawn upon ship destruction")]
        [SerializeField] private GameObject shipDestructionVfxPrefab;

        private HealthManager _healthManager;
        private bool _isLightlySmoking = false;
        private bool _isHeavilySmoking = false;
        private bool _isOnFire = false;
        
        private IObjectPoolProvider _poolProvider;

        private void Awake() {
            _healthManager = GetComponent<HealthManager>();
            _poolProvider = ServiceLocator.Get<IObjectPoolProvider>();
        }

        private void OnEnable() {
            _healthManager.OnHealthChanged += EvaluateDamageState;
            _healthManager.OnZeroHealth += HandleDestruction;
            
            _isLightlySmoking = false; 
            _isHeavilySmoking = false; 
            _isOnFire = false;
            
            if (lightSmokeVfx) 
                lightSmokeVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            if (heavySmokeVfx) 
                heavySmokeVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            if (fireVfx) 
                fireVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        }

        private void OnDisable() {
            _healthManager.OnHealthChanged -= EvaluateDamageState;
            _healthManager.OnZeroHealth -= HandleDestruction;
        }

        private void EvaluateDamageState(int currentHealth, int maxHealth, GameObject instigator) {
            float healthPercentage = (float) currentHealth / maxHealth;
            ApplyDamageEffect(ref _isLightlySmoking, healthPercentage, lightSmokeThreshold, lightSmokeVfx);
            ApplyDamageEffect(ref _isHeavilySmoking, healthPercentage, heavySmokeThreshold, heavySmokeVfx);
            ApplyDamageEffect(ref _isOnFire, healthPercentage, fireThreshold, fireVfx);
        }

        private void ApplyDamageEffect(ref bool isEffectActive, float healthPercentage, float damageThreshold, [CanBeNull] ParticleSystem vfx) {
            if (!isEffectActive && healthPercentage <= damageThreshold) {
                isEffectActive = true;
                if (vfx) 
                    vfx.Play(true);
                
            } else if (isEffectActive && healthPercentage > damageThreshold) {
                isEffectActive = true;
                
                if (vfx) 
                    vfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        private void HandleDestruction(HealthManager source, GameObject killer) {
            if (lightSmokeVfx) 
                lightSmokeVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            if (heavySmokeVfx) 
                heavySmokeVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            if (fireVfx) 
                fireVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            if (shipDestructionVfxPrefab && _poolProvider != null) {
                LevelSettings settings = ServiceLocator.Get<ILevelManager>().Settings;
                _poolProvider.Spawn(
                    shipDestructionVfxPrefab, 
                    transform.position, 
                    Quaternion.identity, 
                    settings.DefaultObjectPoolSize, 
                    settings.MaxDefaultObjectPoolSize
                );
            }
            DeactivateShipComponents();
        }

        private void DeactivateShipComponents() {
            Transform shipRoot = transform.parent ? transform.parent : transform;
            
            if (TryGetComponent<RadarTransponder>(out var transponder)) 
                transponder.enabled = false;
            
            foreach (TurretPlayerInput input in shipRoot.GetComponentsInChildren<TurretPlayerInput>()) {
                input.enabled = false;
            }
            
            foreach (TurretAIBrain input in shipRoot.GetComponentsInChildren<TurretAIBrain>()) {
                input.enabled = false;
            }
            
            foreach (TurretAISensor input in shipRoot.GetComponentsInChildren<TurretAISensor>()) {
                input.enabled = false;
            }
            
            foreach (TurretMotor motor in shipRoot.GetComponentsInChildren<TurretMotor>()) {
                motor.enabled = false;
            }
            
            foreach (Collider col in shipRoot.GetComponentsInChildren<Collider>()) {
                col.enabled = false;
            }
            
            foreach (MeshRenderer mesh in shipRoot.GetComponentsInChildren<MeshRenderer>()) {
                mesh.enabled = false;
            }
        }
    }
}
