using UnityEngine;

namespace _01_Scripts.Core.Interfaces
{
    public interface IMovementBehavior
    {
        /// <summary>
        /// Calculates the optimal direction vector for this specific behavior.
        /// </summary>
        Vector3 CalculateDirection(Transform entityTransform, Vector3 targetPosition);
    }
}