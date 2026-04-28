using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Scoring;
using UnityEngine;
using UnityEngine.Pool;

namespace _01_Scripts.Turrets
{
    /// <summary>
    /// Hardware controller for the turret's weapon systems. Manages the localized 
    /// object pool for projectiles, firing rates, acoustics, and recoil animations.
    /// </summary>
    public class TurretWeapon : MonoBehaviour
    {
        [Header("Hardware Settings")]
        [SerializeField] 
        private TurretProjectile projectilePrefab;
        
        [SerializeField] 
        private Transform muzzleExit;
        
        [SerializeField] 
        private float fireRate = 0.15f;
        
        [Tooltip("The scoring manager that assigned to this player")]
        [SerializeField] 
        private PlayerScore ownerScore;
    
        [Header("Magazine (Pool) Settings")]
        [SerializeField] 
        private int defaultCapacity = 40;
        
        [SerializeField] 
        private int maxSize = 150;
    
        [Header("Visuals")] 
        [Tooltip("The procedural recoil script attached to the visual barrel mesh")]
        [SerializeField] 
        private TurretBarrelRecoil barrelRecoil;
        
        [Header("Acoustics")]
        [Tooltip("The speaker attached to the turret")]
        [SerializeField] 
        private AudioSource weaponAudioSource;
        
        [Tooltip("The sound file to play upon firing")]
        [SerializeField] 
        private AudioClip fireSound;

        private IObjectPool<TurretProjectile> _projectilePool;
        private float _nextFireTime;
        private Collider[] _myColliders;

        private void Awake() {
            _projectilePool = new ObjectPool<TurretProjectile>(
                createFunc: CreateProjectile,
                actionOnGet: OnTakeFromPool,
                actionOnRelease: OnReturnedToPool,
                actionOnDestroy: OnDestroyPoolObject,
                collectionCheck: true,
                defaultCapacity: defaultCapacity,
                maxSize: maxSize
            );
            
            if (weaponAudioSource && GlobalManager.Instance) {
                weaponAudioSource.outputAudioMixerGroup = GlobalManager.Instance.GlobalSettings.SfxMixerGroup;
            }
            
            GameObject shooterIdentity = ownerScore ? ownerScore.gameObject : transform.root.gameObject;
            _myColliders = shooterIdentity.GetComponentsInChildren<Collider>();
        }
        
        private TurretProjectile CreateProjectile() {
            TurretProjectile projectile = Instantiate(projectilePrefab);
            projectile.SetPool(_projectilePool);
            return projectile;
        }

        private void OnTakeFromPool(TurretProjectile projectile) {
            projectile.transform.position = muzzleExit.position;
            projectile.transform.rotation = muzzleExit.rotation;
            projectile.gameObject.SetActive(true);
        }

        private void OnReturnedToPool(TurretProjectile projectile) {
            projectile.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(TurretProjectile projectile) {
            Destroy(projectile.gameObject);
        }

        public void Fire() {
            if (!(Time.time >= _nextFireTime)) 
                return;
            
            _nextFireTime = Time.time + fireRate;
            TurretProjectile projectile = _projectilePool.Get(); 
            
            GameObject shooterIdentity = ownerScore ? ownerScore.gameObject : transform.root.gameObject;
            projectile.SetShooter(shooterIdentity, _myColliders);
            
            if (weaponAudioSource && fireSound) {
                weaponAudioSource.PlayOneShot(fireSound);
            }
            
            if (barrelRecoil) {
                barrelRecoil.TriggerRecoil();
            }
        }
    }
}