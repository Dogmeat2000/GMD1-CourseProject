using UnityEngine;

namespace _01_Scripts.Core.Interfaces
{
    public interface IProjectile : IPoolable
    {
        /// <summary>
        /// Initializes and fires the munition.
        /// </summary>
        void ConfigureProjectile(GameObject shooterIdentity, Collider[] ignoredColliders);
        
        /// <summary>
        /// Implementation to return the projectile to its Object Pool
        /// </summary>
        void ReturnToPool();
    }
}
