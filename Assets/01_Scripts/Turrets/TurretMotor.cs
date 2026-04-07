using UnityEngine;

namespace _01_Scripts.Turrets
{
    public class TurretMotor : MonoBehaviour
    {
        [Header("Mechanical Components")]
        [SerializeField] private Transform turretBase; 
        [SerializeField] private Transform barrelBase; 

        [Header("Weapons Systems")]
        [Tooltip("The Fire Control System managing the object pool")]
        [SerializeField] private TurretWeapon mainWeapon;
        
        [Header("Operational Constraints")]
        [Tooltip("Limits the downward elevation of the barrel, to prevent mesh clipping")]
        [SerializeField] private float minPitch = -15f; 
        [Tooltip("Limits the upward elevation of the barrel, to prevent mesh clipping")]
        [SerializeField] private float maxPitch = 45f;  

        private float _currentPitch = 0f;

        public void RotateJoints(float yawDelta, float pitchDelta) {
            // Pitch Axis (Up/Down)
            if (barrelBase) {
                _currentPitch += pitchDelta;
                _currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);

                // Rotate along Barrel Y-Axis (Up/Down)
                barrelBase.localRotation = Quaternion.Euler(0f, _currentPitch, 0f);
            }

            // Yaw Axis (Left/Right)
            if (turretBase) {
                // Rotate along Turret Y-Axis (Left/Right)
                turretBase.Rotate(Vector3.forward * yawDelta, Space.Self);
            }
        }

        public void PullTrigger() { 
            // Do not fire, if game is paused
            if (Time.timeScale <= 0f) 
                return;
            
            if (mainWeapon) {
                mainWeapon.Fire(); 
            }
        }
    }
}