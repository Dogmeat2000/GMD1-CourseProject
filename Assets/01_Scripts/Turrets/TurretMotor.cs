using UnityEngine;
using UnityEngine.EventSystems;

namespace _01_Scripts.Turrets
{
    public class TurretMotor : MonoBehaviour
    {
        [Header("Mechanical Components")]
        public Transform turretBase; 
        public Transform barrelBase; 

        [Header("Weapons Systems")]
        [Tooltip("The Fire Control System managing the object pool")]
        public TurretWeapon mainWeapon;
        
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
            // Do not fire, if game is paused
            if (Time.timeScale <= 0f) return;
            
            // Do not fire, if player is clicking on UI elements
            if (EventSystem.current && EventSystem.current.IsPointerOverGameObject()) return;
            
            if (mainWeapon)
            {
                mainWeapon.Fire(); 
            }
            else
            {
                Debug.LogWarning("The mainWeapon reference is not assigned in the TurretMotor inspector!");
            }
            Debug.Log("Main Weapon Fired");
        }
    }
}