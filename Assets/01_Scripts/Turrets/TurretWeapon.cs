using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Scoring;
using _01_Scripts.Core.Services;
using UnityEngine;

namespace _01_Scripts.Turrets
{
    /// <summary>
    /// Hardware controller for the turret's weapon systems.
    /// </summary>
    public class TurretWeapon : MonoBehaviour, IRangedWeapon
    {
        [Header("Configuration")]
        // TODO Add description
        [SerializeField] private GameObject projectilePrefab;
        
        // TODO Add description
        [SerializeField] private Transform muzzleExit;
        
        // TODO Add description
        [SerializeField] private float fireRate = 0.2f;
        
        [Tooltip("The scoring manager that assigned to this player")]
        [SerializeField] private PlayerScore ownerScore;
        
        [Header("Visuals")] 
        [Tooltip("The recoil script attached to the visual barrel mesh")]
        [SerializeField] private TurretBarrelRecoil barrelRecoil;
        
        [Tooltip("The VFX prefab spawned at the muzzle upon firing")]
        [SerializeField] private GameObject muzzleFlashPrefab;
        
        [Header("Acoustics")]
        [Tooltip("The speaker attached to the turret")]
        [SerializeField] private AudioSource weaponAudioSource;
        
        [Tooltip("The sound file to play upon firing")]
        [SerializeField] private AudioClip fireSound;
        
        [Header("Optionals")]
        [Tooltip("Assign a TurretCapacitor to enable overheat mechanics.")]
        [SerializeField] private TurretCapacitor energyCapacitor;

        private float _nextFireTime;
        private Collider[] _myColliders;
        private LevelManager _levelManager;

        private void Awake() {
            _levelManager = ServiceLocator.Get<LevelManager>();
            
            if (weaponAudioSource && GlobalManager.Instance)
                weaponAudioSource.outputAudioMixerGroup = GlobalManager.Instance.GlobalSettings.SfxMixerGroup;
            
            GameObject shooterIdentity = ownerScore ? ownerScore.gameObject : transform.root.gameObject;
            _myColliders = shooterIdentity.GetComponentsInChildren<Collider>();
        }

        /// <summary>
        /// Retrieves a projectile from the object pool and applies forward velocity.
        /// </summary>
        public void Fire() {
            if (energyCapacitor && !energyCapacitor.CanFire())
                return;
            
            if (Time.time < _nextFireTime) 
                return;
            
            if (!UniversalPoolService.Instance) {
                Debug.LogError("UniversalPoolService.Instance is not available!");
                return;
            }
            
            _nextFireTime = Time.time + fireRate;
            int poolSize = _levelManager.Settings.DefaultObjectPoolSize;
            int maxPoolSize = _levelManager.Settings.MaxDefaultObjectPoolSize;
            
            if (muzzleFlashPrefab)
                UniversalPoolService.Instance.Spawn(muzzleFlashPrefab, muzzleExit.position, muzzleExit.rotation, poolSize, maxPoolSize);
            
            IPoolable projInstance = UniversalPoolService.Instance.Spawn(projectilePrefab, muzzleExit.position, muzzleExit.rotation, poolSize, maxPoolSize);
            
            if (projInstance is IProjectile munition) {
                GameObject shooterIdentity = ownerScore ? ownerScore.gameObject : transform.root.gameObject;
                munition.Fire(shooterIdentity, _myColliders);
                
                if (energyCapacitor)
                    energyCapacitor.ConsumeEnergy();
            }

            if (weaponAudioSource && fireSound)
                weaponAudioSource.PlayOneShot(fireSound);
            
            if (barrelRecoil)
                barrelRecoil.TriggerRecoil();
        }
        
        /// <summary>
        /// Informs attached modules that the player has let go of the trigger.
        /// </summary>
        public void ReleaseTrigger() {
            if (energyCapacitor)
                energyCapacitor.NotifyTriggerReleased();
        }
    }
}