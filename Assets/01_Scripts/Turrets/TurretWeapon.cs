using _01_Scripts.Turrets;
using UnityEngine;
using UnityEngine.Pool;

public class TurretWeapon : MonoBehaviour
{
    [Header("Hardware Settings")]
    public TurretProjectile projectilePrefab;
    public Transform muzzleExit;
    public float fireRate = 0.15f;
    
    [Header("Magazine (Pool) Settings")]
    public int defaultCapacity = 50;
    public int maxSize = 200;
    
    [Header("Acoustics")]
    [Tooltip("The speaker attached to the turret")]
    public AudioSource weaponAudioSource;
    [Tooltip("The sound file to play upon firing")]
    public AudioClip fireSound;

    private IObjectPool<TurretProjectile> _projectilePool;
    private float _nextFireTime;

    private void Awake()
    {
        _projectilePool = new ObjectPool<TurretProjectile>(
            createFunc: CreateProjectile,
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnedToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: true,
            defaultCapacity: defaultCapacity,
            maxSize: maxSize
        );
    }

    // --- POOL LIFECYCLE DELEGATES ---
    private TurretProjectile CreateProjectile()
    {
        TurretProjectile projectile = Instantiate(projectilePrefab);
        projectile.SetPool(_projectilePool);
        return projectile;
    }

    private void OnTakeFromPool(TurretProjectile projectile)
    {
        projectile.transform.position = muzzleExit.position;
        projectile.transform.rotation = muzzleExit.rotation;
        projectile.gameObject.SetActive(true);
    }

    private void OnReturnedToPool(TurretProjectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }

    private void OnDestroyPoolObject(TurretProjectile projectile)
    {
        Destroy(projectile.gameObject);
    }

    public void Fire()
    {
        if (Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + fireRate;
            _projectilePool.Get(); 
            
            if (weaponAudioSource && fireSound)
            {
                weaponAudioSource.PlayOneShot(fireSound);
            }
        }
    }
}