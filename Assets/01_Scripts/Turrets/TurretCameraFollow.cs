using UnityEngine;

namespace _01_Scripts.Turrets
{
    public class TurretCameraFollow : MonoBehaviour
    {
        [Header("Tracking Target")]
        public Transform barrelPivot;

        [Header("Damping Settings")]
        [Range(0f, 1f)]
        [Tooltip("0 = Fixed Horizon, 1 = Full 1:1 Follow.")]
        public float followFactor = 0.25f;
        
        private Quaternion _initialLocalRotation;

        private void Start()
        {
            _initialLocalRotation = transform.localRotation;
        }

        private void LateUpdate()
        {
            if (!barrelPivot) 
                return;

            // Read the barrel's current pitch (Local Y-Axis)
            float currentBarrelPitch = barrelPivot.localEulerAngles.y;

            // Standardize Unity's 0-360 angles to -180/180
            if (currentBarrelPitch > 180) currentBarrelPitch -= 360;

            // Calculate the delta (camera tilt)
            float trackingOffset = currentBarrelPitch * followFactor;

            // Apply the offset RELATIVE to the starting -90 degree position
            transform.localRotation = _initialLocalRotation * Quaternion.Euler(-trackingOffset, 0f, 0f);
        }
    }
}