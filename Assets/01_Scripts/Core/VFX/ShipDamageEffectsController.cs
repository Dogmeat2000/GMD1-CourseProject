using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Settings;
using _01_Scripts.Core.Targeting;
using _01_Scripts.Turrets;
using _01_Scripts.Turrets.Player;
using UnityEngine;

namespace _01_Scripts.Core.VFX
{
    [RequireComponent(typeof(HealthManager))]
    public class ShipDamageEffectsController : MonoBehaviour
    { 
        [Header("Damage Thresholds (Percentages)")]
        [Range(0f, 1f)] [SerializeField] private float lightSmokeThreshold = 0.80f;
        [Range(0f, 1f)] [SerializeField] private float heavySmokeThreshold = 0.50f;
        [Range(0f, 1f)] [SerializeField] private float fireThreshold = 0.30f;

        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem lightSmokeVfx;
        [SerializeField] private ParticleSystem heavySmokeVfx;
        [SerializeField] private ParticleSystem fireVfx;
        
        [Tooltip("The VFX prefab to spawn upon ship destruction")]
        [SerializeField] private GameObject shipDestructionVfxPrefab;

        private HealthManager _healthManager;
        private bool _isLightlySmoking = false;
        private bool _isHeavilySmoking = false;
        private bool _isOnFire = false;

        private void Awake() {
            _healthManager = GetComponent<HealthManager>();
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
            
            if (!_isLightlySmoking && healthPercentage <= lightSmokeThreshold) {
                _isLightlySmoking = true;
                if (lightSmokeVfx) 
                    lightSmokeVfx.Play(true);
            } else if (_isLightlySmoking && healthPercentage > lightSmokeThreshold) {
                _isLightlySmoking = true;
                lightSmokeVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            
            if (!_isHeavilySmoking && healthPercentage <= heavySmokeThreshold) {
                _isHeavilySmoking = true;
                if (heavySmokeVfx) 
                    heavySmokeVfx.Play(true);
            } else if (_isHeavilySmoking && healthPercentage > heavySmokeThreshold) {
                _isHeavilySmoking = true;
                heavySmokeVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            
            if (!_isOnFire && healthPercentage <= fireThreshold) {
                _isOnFire = true;
                if (fireVfx) 
                    fireVfx.Play(true);
            } else if (_isOnFire && healthPercentage > fireThreshold) {
                _isOnFire = true;
                fireVfx.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
        }

        private void HandleDestruction(HealthManager source, GameObject killer) {
            if (lightSmokeVfx) 
                lightSmokeVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            if (heavySmokeVfx) 
                heavySmokeVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            if (fireVfx) 
                fireVfx.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            
            if (shipDestructionVfxPrefab && UniversalPoolService.Instance) {
                LevelSettings settings = ServiceLocator.Get<LevelManager>().Settings;
                UniversalPoolService.Instance.Spawn(
                    shipDestructionVfxPrefab, 
                    transform.position, 
                    Quaternion.identity, 
                    settings.DefaultObjectPoolSize, 
                    settings.MaxDefaultObjectPoolSize
                );
            }
            
            Transform shipRoot = transform.parent ? transform.parent : transform;
            
            if (TryGetComponent<RadarTransponder>(out var transponder)) 
                transponder.enabled = false;
            
            foreach (var input in shipRoot.GetComponentsInChildren<TurretPlayerInput>()) {
                input.enabled = false;
            }
            
            foreach (var motor in shipRoot.GetComponentsInChildren<TurretMotor>()) {
                motor.enabled = false;
            }
            
            foreach (var col in shipRoot.GetComponentsInChildren<Collider>()) {
                col.enabled = false;
            }
            
            foreach (var mesh in shipRoot.GetComponentsInChildren<MeshRenderer>()) {
                mesh.enabled = false;
            }
        }
    }
}
