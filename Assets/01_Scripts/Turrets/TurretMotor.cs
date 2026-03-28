using UnityEngine;

namespace _01_Scripts.Turrets
{
    public class TurretMotor : MonoBehaviour
    {
        [Header("Mechanical Components")]
        public Transform turretBase; 
        public Transform barrelBase; 

        [Header("Operational Constraints")]
        [Tooltip("Limits the downward elevation of the barrel, to prevent mesh clipping")]
        public float minPitch = -15f; 
        [Tooltip("Limits the upward elevation of the barrel, to prevent mesh clipping")]
        public float maxPitch = 45f;  

        private float _currentPitch = 0f;

        public void RotateJoints(float yawDelta, float pitchDelta)
        {
            // Pitch Axis (Up/Down)
            if (barrelBase) 
            {
                _currentPitch += pitchDelta;
                _currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);

                // Rotate along Barrel Y-Axis (Up/Down)
                barrelBase.localRotation = Quaternion.Euler(0f, _currentPitch, 0f);
            }

            // Yaw Axis (Left/Right)
            if (turretBase) 
            {
                // Rotate along Turret Y-Axis (Left/Right)
                turretBase.Rotate(Vector3.forward * yawDelta, Space.Self);
            }
        }

        public void PullTrigger()
        {
            // Execution for main weapon fire
            Debug.Log("Main Weapon Fired");
        }
    }
}