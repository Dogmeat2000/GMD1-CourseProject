using UnityEngine;

namespace _01_Scripts.Core.Interfaces
{
    public interface IProjectile : IPoolable
    {
        /// <summary>
        /// Initializes and fires the munition.
        /// </summary>
        /// <param name="shooterIdentity">The entity claiming the kill.</param>
        /// <param name="ignoredColliders">Colliders to bypass (e.g., the turret's own barrel).</param>
        void Fire(GameObject shooterIdentity, Collider[] ignoredColliders);
        
        /// <summary>
        /// Implementation to return the projectile to its Object Pool
        /// </summary>
        void ReturnToPool();
    }
}
