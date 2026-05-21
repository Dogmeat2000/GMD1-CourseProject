using _01_Scripts.Core.Services;
using UnityEngine;

namespace _01_Scripts.Turrets
{
    /// <summary>
    /// The primary motor, responsible for moving a Turret and initiating function such as firing a weapon.
    /// This raw Engine script takes either input from a Player or from an AI brain and uses these to move the Turret in physical world space (aim, rotate, shoot).
    /// </summary>
    public class TurretMotor : MonoBehaviour
    {
        [Header("Mechanical Components")]
        [Tooltip("The base of the Turret, around which the turret rotates (left/right)")]
        [SerializeField] private Transform turretBase; 
        
        [Tooltip("The base of the barrels/weapons, that can be aimed up/down.")]
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
        
        private void Awake() {
            _gameStateService = ServiceLocator.Get<GameStateService>();
        }

        /// <summary>
        /// Applies rotation and pitch to the Turret and its weapons.
        /// </summary>
        /// <param name="yawDelta"></param>
        /// <param name="pitchDelta"></param>
        public void RotateJoints(float yawDelta, float pitchDelta) {
            if (barrelBase) {
                _currentPitch += pitchDelta;
                _currentPitch = Mathf.Clamp(_currentPitch, minPitch, maxPitch);
                barrelBase.localRotation = Quaternion.Euler(0f, _currentPitch, 0f);
            }
            
            if (turretBase) {
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