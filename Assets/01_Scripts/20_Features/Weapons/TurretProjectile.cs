using System;
using _01_Scripts._10_Core.Pooling;
using UnityEngine;

namespace _01_Scripts._20_Features.Weapons
{
    /// <summary>
    /// A Concrete Projectile type, that is fired from Turrets.
    /// </summary>
    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(Warhead))]
    public class TurretProjectile : MonoBehaviour, IProjectile
    {
        [Header("Ballistics")]
        [Tooltip("The speed that this Projectile moves with (m/s)")]
        [SerializeField] private float speed = 500f;
        
        [Tooltip("How long time [s] this projectile should live, before being despawned.")]
        [SerializeField] private float lifeTime = 3f;
        
        [Tooltip("The amount of Damage this projectile inflicts")]
        [SerializeField] private int damageAmount = 50; // TODO I believe I define this in multiple places! Confirm whether I want this as a serialized field here, or if it should always be defined by the shooting entity!

        private float _currentLifeTime;
        private Rigidbody _rb;
        private Warhead _warhead;
        private Collider _myCollider;
        
        private Collider[] _ignoredColliders;
        private Action<IPoolable> _returnToPoolCommand;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update() {
            _currentLifeTime += Time.deltaTime;
            if (_currentLifeTime >= lifeTime)
                ReturnToPool();
        }

        private void FixedUpdate() {
            if (_rb)
                _rb.linearVelocity = transform.forward * speed;
        }

        /// <summary>
        /// Configures the projectile, usually upon firing, embedding identity of the shooter and disabling provided colliders.
        /// This allows for easily determining the identity of any entity that deals damage or kills another.
        /// It also allows for disabling friendly fire (or similar) by providing colliders to disable collision with.
        /// </summary>
        /// <param name="shooterIdentity"></param>
        /// <param name="ignoredColliders"></param>
        public void ConfigureProjectile(GameObject shooterIdentity, Collider[] ignoredColliders) {
            _ignoredColliders = ignoredColliders;
            
            if (_warhead)
                _warhead.Instigator = shooterIdentity;
            
            Collider myCollider = GetComponent<Collider>();
            if (myCollider && ignoredColliders != null) {
                foreach (var c in ignoredColliders) {
                    Physics.IgnoreCollision(myCollider, c);
                }
            }
        }
        
        public void Initialize(Action<IPoolable> returnAction) {
            _returnToPoolCommand = returnAction;
            
            if (!_rb) 
                _rb = GetComponent<Rigidbody>();
            
            if (!_warhead) 
                _warhead = GetComponent<Warhead>();
            
            if (!_myCollider) 
                _myCollider = GetComponent<Collider>();
        }
        
        public void OnSpawned() {
            _currentLifeTime = 0f;
                
            if (_rb) {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            
            if (_warhead)
                _warhead.ImpactAmount = damageAmount;
        }
        
        public void OnDespawned() {
            if (_myCollider && _ignoredColliders != null) {
                foreach (var col in _ignoredColliders) {
                    if (col) Physics.IgnoreCollision(_myCollider, col, false);
                }
            }
            
            _ignoredColliders = null;
            
            if (_rb) {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
        }
        
        public void ReturnToPool() {
            if (_returnToPoolCommand != null)
                _returnToPoolCommand.Invoke(this);
            else
                Destroy(gameObject);
        }
    }
}