using System;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Settings;
using UnityEngine;

namespace _01_Scripts.Turrets
{
    /// <summary>
    /// Regulates energy consumption, recharge, and overheat states.
    /// </summary>
    public class TurretCapacitor : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("Maximum energy storage.")]
        [SerializeField] private float maxEnergy = 100f;
        
        [Tooltip("Energy consumed per shot fired.")]
        [SerializeField] private float energyCostPerShot = 2.5f;
        
        [Tooltip("How much energy regenerates per second while not firing.")]
        [SerializeField] private float rechargeRatePerSecond = 15f;
        
        [Tooltip("Delay [s] after firing before recharge begins.")]
        [SerializeField] private float rechargeDelayAfterFire = 0.5f;
        
        [Tooltip("Mandatory cooldown time [s] when overheated before it begins recharging.")]
        [SerializeField] private float overheatPenaltyTime = 3f;

        [Header("Visual Feedback")]
        [Tooltip("The VFX prefab to spawn from the UniversalPoolService when overheated.")]
        [SerializeField] private GameObject overheatVfxPrefab;
        
        [Tooltip("The transform representing the muzzle exit.")]
        [SerializeField] private Transform muzzleExit;
        
        /// <summary>
        /// Broadcasts the current energy level (0.0 to 1.0) to update HUD elements.
        /// </summary>
        public event Action<float> OnEnergyPercentageChanged;
        
        private float _currentEnergy;
        private float _timeSinceLastFire;
        private float _overheatTimer;
        private bool _isOverheated;
        private bool _triggerReleasedSinceOverheat = true;
        
        private void Awake() {
            _currentEnergy = maxEnergy;
        }

        private void Start() {
            OnEnergyPercentageChanged?.Invoke(_currentEnergy);
        }
        
        private void Update() {
            _timeSinceLastFire += Time.deltaTime;

            if (_isOverheated) {
                if (_overheatTimer > 0)
                    _overheatTimer -= Time.deltaTime;
                else
                    ResolveOverheat();
            } else if (_timeSinceLastFire >= rechargeDelayAfterFire) {
                Recharge(rechargeRatePerSecond * Time.deltaTime);
            }
        }
        
        private void Recharge(float amount) {
            if (_currentEnergy < maxEnergy) {
                _currentEnergy = Mathf.Clamp(_currentEnergy + amount, 0, maxEnergy);
                OnEnergyPercentageChanged?.Invoke(_currentEnergy / maxEnergy);
            }
        }
        
        // TODO Add description
        public bool CanFire() {
            if (_isOverheated) 
                return false;
            
            if (!_triggerReleasedSinceOverheat) 
                return false; 
            
            return _currentEnergy >= energyCostPerShot;
        }
        
        // TODO Add description
        public void ConsumeEnergy() {
            _currentEnergy -= energyCostPerShot;
            _timeSinceLastFire = 0f;
            
            OnEnergyPercentageChanged?.Invoke(_currentEnergy / maxEnergy);

            if (_currentEnergy < energyCostPerShot) {
                _currentEnergy = 0;
                TriggerOverheat();
            }
        }
        
        // TODO Add description
        public void NotifyTriggerReleased() {
            _triggerReleasedSinceOverheat = true;
        }
        
        private void TriggerOverheat() {
            _isOverheated = true;
            _triggerReleasedSinceOverheat = false;
            _overheatTimer = overheatPenaltyTime;
            
            if (overheatVfxPrefab && muzzleExit && UniversalPoolService.Instance) {
                LevelSettings settings = ServiceLocator.Get<LevelManager>().Settings;
                UniversalPoolService.Instance.Spawn(
                    overheatVfxPrefab, 
                    muzzleExit.position, 
                    muzzleExit.rotation, 
                    settings.DefaultObjectPoolSize, 
                    settings.MaxDefaultObjectPoolSize
                );
            }
        }
        
        private void ResolveOverheat() {
            _isOverheated = false;
        }
    }
}
