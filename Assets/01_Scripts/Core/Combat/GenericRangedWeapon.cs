using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using UnityEngine;

namespace _01_Scripts.Core.Combat
{
    /// <summary>
    /// A basic implementation of IRangedWeapon designed for enemy AI.
    /// </summary>
    public class GenericRangedWeapon : MonoBehaviour, IRangedWeapon
    {
        [Header("Configuration")]
        [Tooltip("The projectile prefab, implementing IProjectile, to spawn")]
        [SerializeField] private GameObject projectilePrefab;
        
        [Tooltip("The exact transform where the projectile spawns and aligns to.")]
        [SerializeField] private Transform muzzleExit;

        [Header("Optional")]
        [Tooltip("Optional: VFX (Muzzle flash or similar) to spawn upon firing.")]
        [SerializeField] private GameObject muzzleFlashPrefab;
        
        [Tooltip("Optional: Audio Source to play AudioCLip(s) provided below, from.")]
        [SerializeField] private AudioSource weaponAudioSource;
        
        [Tooltip("Optional: SFX to be played when firing this weapon.")]
        [SerializeField] private AudioClip fireSound;

        private Collider[] _myColliders;
        private LevelManager _levelManager;

        private void Awake() {
            _levelManager = ServiceLocator.Get<LevelManager>();
            _myColliders = transform.root.GetComponentsInChildren<Collider>();
        }

        public void Fire() {
            if (!projectilePrefab || !muzzleExit) 
                return;

            int poolSize = _levelManager ? _levelManager.Settings.DefaultObjectPoolSize : 10;
            int maxPoolSize = _levelManager ? _levelManager.Settings.MaxDefaultObjectPoolSize : 50;

            if (muzzleFlashPrefab && UniversalPoolService.Instance)
                UniversalPoolService.Instance.Spawn(muzzleFlashPrefab, muzzleExit.position, muzzleExit.rotation, poolSize, maxPoolSize);

            if (UniversalPoolService.Instance) {
                IPoolable projInstance = UniversalPoolService.Instance.Spawn(projectilePrefab, muzzleExit.position, muzzleExit.rotation, poolSize, maxPoolSize);
                
                if (projInstance is IProjectile munition) {
                    munition.Fire(transform.root.gameObject, _myColliders);
                }
            }

            if (weaponAudioSource && fireSound)
                weaponAudioSource.PlayOneShot(fireSound);
        }
    }
}
