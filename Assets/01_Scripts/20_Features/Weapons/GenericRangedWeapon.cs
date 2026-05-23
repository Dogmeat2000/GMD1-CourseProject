using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._10_Core.Pooling;
using _01_Scripts._20_Features.Progression;
using UnityEngine;

namespace _01_Scripts._20_Features.Weapons
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
        private ILevelManager _levelManager;
        private IObjectPoolProvider _poolProvider;

        private void Awake() {
            _levelManager = ServiceLocator.Get<ILevelManager>();
            _poolProvider = ServiceLocator.Get<IObjectPoolProvider>();
            _myColliders = transform.root.GetComponentsInChildren<Collider>();
        }

        public void Fire() {
            if (!projectilePrefab || !muzzleExit) 
                return;

            int poolSize = _levelManager != null ? _levelManager.Settings.DefaultObjectPoolSize : 10;
            int maxPoolSize = _levelManager != null ? _levelManager.Settings.MaxDefaultObjectPoolSize : 50;

            if (muzzleFlashPrefab && _poolProvider != null)
                _poolProvider.Spawn(muzzleFlashPrefab, muzzleExit.position, muzzleExit.rotation, poolSize, maxPoolSize);

            if (_poolProvider != null) {
                IPoolable projInstance = _poolProvider.Spawn(projectilePrefab, muzzleExit.position, muzzleExit.rotation, poolSize, maxPoolSize);
                
                if (projInstance is IProjectile munition) {
                    munition.ConfigureProjectile(transform.root.gameObject, _myColliders);
                }
            }

            if (weaponAudioSource && fireSound)
                weaponAudioSource.PlayOneShot(fireSound);
        }

        public void ReleaseTrigger() {
            // Do Nothing.
        }
    }
}
