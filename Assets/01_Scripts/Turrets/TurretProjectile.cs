using UnityEngine;
using UnityEngine.Pool;

namespace _01_Scripts.Turrets
{
    public class TurretProjectile : MonoBehaviour
    {
        [Header("Ballistics")]
        public float speed = 500f;
        public float lifeTime = 3f;

        // The interface linking this bullet back to the turret's "magazine"
        private IObjectPool<TurretProjectile> _managedPool;
        private float _currentLifeTime;

        // Called by the Turret exactly once when this bullet is forged
        public void SetPool(IObjectPool<TurretProjectile> pool)
        {
            _managedPool = pool;
        }

        private void OnEnable()
        {
            // Reset the timer every time the bullet is pulled from the pool
            _currentLifeTime = 0f;
        }

        private void Update()
        {
            // Move the bullet forward (Z-axis)
            transform.Translate(Vector3.forward * (speed * Time.deltaTime));

            // Auto-return to pool if it flies off into space
            _currentLifeTime += Time.deltaTime;
            if (_currentLifeTime >= lifeTime)
            {
                ReturnToPool();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // TODO: Apply damage to enemies here
        
            // Return to pool upon impact
            ReturnToPool();
        }

        private void ReturnToPool()
        {
            if (gameObject.activeSelf)
            {
                _managedPool?.Release(this);
            }
        }
    }
}