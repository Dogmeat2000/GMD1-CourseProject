using _01_Scripts.Core.Interfaces;
using UnityEngine;

namespace _01_Scripts.Core.Movement.Behaviors
{
    // TODO: Add interface description
    public class EvasiveBehavior : IMovementBehavior
    {
        private float _noiseOffset;
        private readonly float _evasionIntensity;
        private readonly float _evasionSpeed;

        public EvasiveBehavior(float intensity = 10f, float speed = 2f) {
            _evasionIntensity = intensity;
            _evasionSpeed = speed;
            _noiseOffset = Random.Range(0f, 1000f);
        }

        public Vector3 CalculateDirection(Transform entityTransform, Vector3 targetPosition) {
            float noiseTime = Time.time * _evasionSpeed + _noiseOffset;
            
            float offsetX = (Mathf.PerlinNoise(noiseTime, 0) * 2f) - 1f;
            float offsetY = (Mathf.PerlinNoise(0, noiseTime) * 2f) - 1f;
            
            Vector3 evasionVector = (entityTransform.right * offsetX) + (entityTransform.up * offsetY);
            return evasionVector * _evasionIntensity;
        }
    }
}