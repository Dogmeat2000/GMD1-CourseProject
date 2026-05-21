using System;
using _01_Scripts.Core;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01_Scripts.Turrets
{
    /// <summary>
    /// A weapon system to handle special ammunition with limited ammunition and multiple potential spawn points (the two missile bays on the turret).
    /// </summary>
    public class SpecialWeaponsSystem : MonoBehaviour
    { 
        [Header("Ammunition")]
        [Tooltip("Maximum number of shots available per game.")]
        [SerializeField] private int maxAmmo = 3;
        
        [Tooltip("Maximum number of shots pr second.")]
        [SerializeField] private float fireRate = 1.0f;

        [Header("Mandatory")]
        [Tooltip("The projectile prefab to spawn (Must implement IProjectile)")]
        [SerializeField] private GameObject projectilePrefab;
        
        [Tooltip("Optional: Force spawned projectiles to align with the rotation of another Transform, i.e. the barrels that can be elevated up/down.")]
        [SerializeField] private Transform aimReference;
        
        [Tooltip("Array of possible exit points. A random one is chosen per shot.")]
        [SerializeField] private Transform[] muzzleExits;

        [Header("Visuals")]
        [Tooltip("The Prefab containing the VFX effect to apply upon firing this weapons system")]
        [SerializeField] private GameObject muzzleFlashPrefab;
        
        [Tooltip("Optional: The Audio Source to use to play any provided sound effects from.")]
        [SerializeField] private AudioSource weaponAudioSource;
        
        [Tooltip("Optional: The AudioClip to play when firing this weapon.")]
        [SerializeField] private AudioClip fireSound;
        
        [Tooltip("Optional: The AudioClip to play when failing to fire this weapon.")]
        [SerializeField] private AudioClip emptyClickSound;

        /// <summary>
        /// Broadcasts Current Ammo and Max Ammo to the UI.
        /// </summary>
        public event Action<int, int> OnAmmoChanged;

        private int _currentAmmo;
        private float _nextFireTime;
        private Collider[] _myColliders;
        private LevelManager _levelManager;

        private void Awake() {
            _levelManager = ServiceLocator.Get<LevelManager>();
            
            Transform currentAncestor = transform;
            Transform confirmedShipRoot = transform.root; 
            
            while (currentAncestor) {
                HealthManager foundHull = currentAncestor.GetComponentInChildren<HealthManager>();
                
                if (foundHull) {
                    confirmedShipRoot = currentAncestor;
                    break;
                }
                
                currentAncestor = currentAncestor.parent;
            }
            
            _myColliders = confirmedShipRoot.GetComponentsInChildren<Collider>();
            _currentAmmo = maxAmmo;
        }

        private void Start() {
            OnAmmoChanged?.Invoke(_currentAmmo, maxAmmo);
        }

        /// <summary>
        /// Fires this weapon.
        /// </summary>
        public void Fire() {
            if (Time.time < _nextFireTime) 
                return;

            if (_currentAmmo <= 0) {
                if (weaponAudioSource && emptyClickSound)
                    weaponAudioSource.PlayOneShot(emptyClickSound);
                
                _nextFireTime = Time.time + 0.25f;
                return;
            }

            if (muzzleExits == null || muzzleExits.Length == 0) {
                Debug.LogWarning("No muzzle exits assigned!");
                return;
            }

            _nextFireTime = Time.time + fireRate;
            _currentAmmo--;
            OnAmmoChanged?.Invoke(_currentAmmo, maxAmmo);
            
            Transform selectedMuzzle = muzzleExits[Random.Range(0, muzzleExits.Length)];

            Quaternion launchRotation = aimReference ? aimReference.rotation : selectedMuzzle.rotation;
            
            int poolSize = _levelManager.Settings.DefaultObjectPoolSize;
            int maxPoolSize = _levelManager.Settings.MaxDefaultObjectPoolSize;

            if (muzzleFlashPrefab && UniversalPoolService.Instance)
                UniversalPoolService.Instance.Spawn(muzzleFlashPrefab, selectedMuzzle.position, selectedMuzzle.rotation, poolSize, maxPoolSize);

            if (UniversalPoolService.Instance) {
                IPoolable projInstance = UniversalPoolService.Instance.Spawn(projectilePrefab, selectedMuzzle.position, launchRotation, poolSize, maxPoolSize);
                
                if (projInstance is IProjectile munition)
                    munition.ConfigureProjectile(transform.root.gameObject, _myColliders);
            }

            if (weaponAudioSource && fireSound)
                weaponAudioSource.PlayOneShot(fireSound);
        }
    }
}
