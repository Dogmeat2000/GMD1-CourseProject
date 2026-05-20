using System;
using _01_Scripts.Core.Combat;
using _01_Scripts.Core.Interfaces;
using UnityEngine;

namespace _01_Scripts.Turrets
{
    // TODO Add description
    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(Warhead))]
    public class TurretProjectile : MonoBehaviour, IProjectile
    {
        [Header("Ballistics")]
        // TODO Add description
        [SerializeField] private float speed = 500f;
        
        // TODO Add description
        [SerializeField] private float lifeTime = 3f;
        
        // TODO Add description
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

        // TODO Add description
        public void Fire(GameObject shooterIdentity, Collider[] ignoredColliders) {
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

        // TODO Add description
        public void Initialize(Action<IPoolable> returnAction) {
            _returnToPoolCommand = returnAction;
            
            if (!_rb) 
                _rb = GetComponent<Rigidbody>();
            
            if (!_warhead) 
                _warhead = GetComponent<Warhead>();
            
            if (!_myCollider) 
                _myCollider = GetComponent<Collider>();
        }
        
        // TODO Add description
        public void OnSpawned() {
            _currentLifeTime = 0f;
                
            if (_rb) {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            
            if (_warhead)
                _warhead.ImpactAmount = damageAmount;
        }
        
        // TODO Add description
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
        
        // TODO Add description
        public void ReturnToPool() {
            if (_returnToPoolCommand != null)
                _returnToPoolCommand.Invoke(this);
            else
                Destroy(gameObject);
        }
    }
}