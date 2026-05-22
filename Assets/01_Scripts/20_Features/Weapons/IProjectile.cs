using _01_Scripts._10_Core.Pooling;
using UnityEngine;

namespace _01_Scripts._20_Features.Weapons
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
