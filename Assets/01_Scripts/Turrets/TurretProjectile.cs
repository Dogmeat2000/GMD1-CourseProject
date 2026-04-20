using _01_Scripts.Core.Combat;
using UnityEngine;
using UnityEngine.Pool;

namespace _01_Scripts.Turrets
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(ImpactWarhead))]
    public class TurretProjectile : MonoBehaviour
    {
        [Header("Ballistics")]
        [SerializeField] private float speed = 500f;
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private int damageAmount = 50;

        private IObjectPool<TurretProjectile> _managedPool;
        private float _currentLifeTime;
        private Rigidbody _rb;
        private ImpactWarhead _warhead;
        
        private Collider _myCollider;
        private Collider[] _ignoredColliders;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
            _warhead = GetComponent<ImpactWarhead>();
            _myCollider = GetComponent<Collider>();
        }
        
        public void SetPool(IObjectPool<TurretProjectile> pool) {
            _managedPool = pool;
        }
        
        public void SetShooter(GameObject shooter, Collider[] cachedColliders) {
            if (_warhead) {
                _warhead.Instigator = shooter;
            }
            
            _ignoredColliders = cachedColliders;
            
            foreach (var col in _ignoredColliders) {
                Physics.IgnoreCollision(_myCollider, col, true);
            }
        }

        private void OnEnable() {
            _currentLifeTime = 0f;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            
            if (_warhead) {
                _warhead.PayloadDamage = damageAmount;
            }
        }
        
        private void OnDisable() {
            if (_ignoredColliders != null) {
                foreach (var col in _ignoredColliders) {
                    if (col) Physics.IgnoreCollision(_myCollider, col, false);
                }
                _ignoredColliders = null;
            }
        }

        private void Update() {
            _currentLifeTime += Time.deltaTime;
            if (_currentLifeTime >= lifeTime) {
                DespawnRoutine();
            }
        }

        private void FixedUpdate() {
            _rb.linearVelocity = transform.forward * speed;
        }
        
        public void DespawnRoutine() {
            if (!gameObject.activeSelf) 
                return;
            
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            _managedPool?.Release(this);
        }
    }
}