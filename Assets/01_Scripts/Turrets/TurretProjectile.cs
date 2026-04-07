using _01_Scripts.Core;
using UnityEngine;
using UnityEngine.Pool;

namespace _01_Scripts.Turrets
{
    public class TurretProjectile : MonoBehaviour
    {
        [Header("Ballistics")]
        [SerializeField] private float speed = 500f;
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private int damageAmount = 50;

        private IObjectPool<TurretProjectile> _managedPool;
        private float _currentLifeTime;
        private GameObject _shooter;
        private Rigidbody _rb;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }
        
        public void SetPool(IObjectPool<TurretProjectile> pool) {
            _managedPool = pool;
        }
        
        public void SetShooter(GameObject shooter)
        {
            _shooter = shooter;
        }

        private void OnEnable() {
            // Reset the timer every time the bullet is pulled from the pool
            _currentLifeTime = 0f;
        }

        private void Update() {
            // Auto-return to pool if it flies off into space
            _currentLifeTime += Time.deltaTime;
            if (_currentLifeTime >= lifeTime) {
                ReturnToPool();
            }
        }

        private void FixedUpdate() {
            _rb.linearVelocity = transform.forward * speed;
        }

        private void OnTriggerEnter(Collider other) {
            if (other.transform.root.gameObject == _shooter) {
                // Ignore the collider from the shooter, to prevent bullet blowing up immediately when firing.
                return; 
            }
            
            if (other.TryGetComponent(out HealthManager targetHealth)) {
                targetHealth.TakeDamage(damageAmount, _shooter);
            }
            
            // Add impact FX:
            // TODO
            
            ReturnToPool();
        }

        private void ReturnToPool() {
            if (!gameObject.activeSelf) 
                return;
            
            _rb.linearVelocity = Vector3.zero;
            _managedPool?.Release(this);
        }
    }
}