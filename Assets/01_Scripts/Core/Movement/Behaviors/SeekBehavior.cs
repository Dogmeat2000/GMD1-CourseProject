using _01_Scripts.Core.Interfaces;
using UnityEngine;

namespace _01_Scripts.Core.Movement.Behaviors
{
    /// <summary>
    /// A Behavior subtype that applies a seeking movement pattern to entities,
    /// causing them to more straight towards a given target.
    /// This is achieved by adjusting the Vector3 that is used to propel entities forward in each frame.
    /// </summary>
    public class SeekBehavior : IMovementBehavior
    {
        public Vector3 CalculateDirection(Transform entityTransform, Vector3 targetPosition) {
            return (targetPosition - entityTransform.position).normalized;
        }
    }
}