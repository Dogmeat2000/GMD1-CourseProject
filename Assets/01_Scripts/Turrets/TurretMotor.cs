using _01_Scripts.Core.Services;
using UnityEngine;

namespace _01_Scripts.Turrets
{
    public class TurretMotor : MonoBehaviour
    {
        [Header("Mechanical Components")]
        [SerializeField] private Transform turretBase; 
        [SerializeField] private Transform barrelBase; 

        [Header("Weapons Systems")]
        [Tooltip("The Main Weapon")]
        [SerializeField] private TurretWeapon mainWeapon;
        
        [Tooltip("The Auxiliary Weapon (Optional)")]
        [SerializeField] private TurretWeapon auxiliaryWeapon;
        
        [Tooltip("Special Weapon 1 (Optional)")]
        [SerializeField] private SpecialWeaponsSystem specialWeapon1;
        
        [Header("Constraints")]
        [Tooltip("Limits the downward elevation of the barrel, to prevent mesh clipping")]
        [SerializeField] private float minPitch = -15f; 
        
        [Tooltip("Limits the upward elevation of the barrel, to prevent mesh clipping")]
        [SerializeField] private float maxPitch = 45f;  

        private float _currentPitch = 0f;
        private GameStateService _gameStateService;
        
        /// <summary>
        /// Defines the specific weapon hardware to fire.
        /// </summary>
        public enum WeaponSlot {
            Main,
            Auxiliary,
            Special1,
            Both
        }
        
        
        private void Awake() {
            _gameStateService = ServiceLocator.Get<GameStateService>();
        }

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

        /// <summary>
        /// Fires the specified weapon system. Defaults to the Main weapon if no parameter is provided.
        /// </summary>
        public void PullTrigger(WeaponSlot slot = WeaponSlot.Main) { 
            if (_gameStateService != null && _gameStateService.CurrentState != GameState.Playing) 
                return;
            
            switch (slot) {
                case WeaponSlot.Main:
                    if (mainWeapon) 
                        mainWeapon.Fire(); 
                    break;
                
                case WeaponSlot.Auxiliary:
                    if (auxiliaryWeapon) 
                        auxiliaryWeapon.Fire(); 
                    break;
                
                case WeaponSlot.Special1:
                    if(specialWeapon1)
                        specialWeapon1.Fire();
                    break;
                    
                case WeaponSlot.Both:
                    if (mainWeapon) 
                        mainWeapon.Fire();
                    
                    if (auxiliaryWeapon) 
                        auxiliaryWeapon.Fire();
                    break;
            }
        }
        
        /// <summary>
        /// Informs the weapon systems that the player has released the trigger.
        /// </summary>
        public void ReleaseTrigger(WeaponSlot slot = WeaponSlot.Main) {
            switch (slot) {
                case WeaponSlot.Main:
                    if (mainWeapon) 
                        mainWeapon.ReleaseTrigger(); 
                    break;
                
                case WeaponSlot.Auxiliary:
                    if (auxiliaryWeapon) 
                        auxiliaryWeapon.ReleaseTrigger(); 
                    break;
                
                case WeaponSlot.Both:
                    if (mainWeapon) 
                        mainWeapon.ReleaseTrigger();
                    
                    if (auxiliaryWeapon) 
                        auxiliaryWeapon.ReleaseTrigger();
                    break;
            }
        }
    }
}