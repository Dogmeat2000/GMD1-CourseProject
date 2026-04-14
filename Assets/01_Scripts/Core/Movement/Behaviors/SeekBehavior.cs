using _01_Scripts.Core.Interfaces;
using UnityEngine;

namespace _01_Scripts.Core.Movement.Behaviors
{
    public class SeekBehavior : IMovementBehavior
    {
        public Vector3 CalculateDirection(Transform entityTransform, Vector3 targetPosition) {
            return (targetPosition - entityTransform.position).normalized;
        }
    }
}