using System;
using _01_Scripts.Core.Combat;
using _01_Scripts.Core.Interfaces;
using UnityEngine;

namespace _01_Scripts.Turrets
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider), typeof(ImpactWarhead))]
    public class TurretProjectile : MonoBehaviour, IProjectile
    {
        [Header("Ballistics")]
        [SerializeField] private float speed = 500f;
        [SerializeField] private float lifeTime = 3f;
        [SerializeField] private int damageAmount = 50;

        private float _currentLifeTime;
        private Rigidbody _rb;
        private ImpactWarhead _warhead;
        private Collider _myCollider;
        
        private Collider[] _ignoredColliders;
        private Action<IPoolable> _returnToPoolCommand;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
        }

        private void Update() {
            _currentLifeTime += Time.deltaTime;
            if (_currentLifeTime >= lifeTime) {
                ReturnToPool();
            }
        }

        private void FixedUpdate() {
            if (_rb) {
                _rb.linearVelocity = transform.forward * speed;
            }
        }

        public void Fire(GameObject shooterIdentity, Collider[] ignoredColliders) {
            _ignoredColliders = ignoredColliders;
            
            if (_warhead) {
                _warhead.Instigator = shooterIdentity;
            }
            
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
                _warhead = GetComponent<ImpactWarhead>();
            
            if (!_myCollider) 
                _myCollider = GetComponent<Collider>();
        }
        public void OnSpawned() {
            _currentLifeTime = 0f;
                
            if (_rb) {
                _rb.linearVelocity = Vector3.zero;
                _rb.angularVelocity = Vector3.zero;
            }
            
            if (_warhead) {
                _warhead.PayloadDamage = damageAmount;
            }
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
            if (_returnToPoolCommand != null) {
                _returnToPoolCommand.Invoke(this);
            } else {
                Destroy(gameObject);
            }
        }
    }
}