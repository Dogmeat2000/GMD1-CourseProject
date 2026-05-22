using UnityEngine;

namespace _01_Scripts._20_Features.Navigation
{
    /// <summary>
    /// Primary interface that attackers use to select appropriate movement behaviors, as they approach the players.
    /// </summary>
    public interface IMovementBehavior
    {
        /// <summary>
        /// Calculates the optimal direction vector for this specific behavior.
        /// </summary>
        Vector3 CalculateDirection(Transform entityTransform, Vector3 targetPosition);
    }
}