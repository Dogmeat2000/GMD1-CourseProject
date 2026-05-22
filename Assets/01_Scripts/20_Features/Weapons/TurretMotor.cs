using System.Collections.Generic;
using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._20_Features.Progression;
using UnityEngine;

namespace _01_Scripts._20_Features.Weapons
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
        [Tooltip("Assign the GameObject holding the IRangedWeapon script for each slot.")]
        [SerializeField] private GameObject mainWeaponMount;
        [SerializeField] private GameObject auxiliaryWeaponMount;
        [SerializeField] private GameObject specialWeapon1Mount;
        
        
        [Header("Constraints")]
        [Tooltip("Limits the downward elevation of the barrel, to prevent mesh clipping")]
        [SerializeField] private float minPitch = -15f; 
        
        [Tooltip("Limits the upward elevation of the barrel, to prevent mesh clipping")]
        [SerializeField] private float maxPitch = 45f;  

        private readonly Dictionary<WeaponSlot, IRangedWeapon> _weaponArsenal = new Dictionary<WeaponSlot, IRangedWeapon>();
        
        private float _currentPitch = 0f;
        private IGameStateProvider _gameStateService;
        
        private void Awake() {
            _gameStateService = ServiceLocator.Get<IGameStateProvider>();
            
            if (mainWeaponMount && mainWeaponMount.TryGetComponent(out IRangedWeapon mainWep)) {
                _weaponArsenal.Add(WeaponSlot.Main, mainWep);
            }
            
            if (auxiliaryWeaponMount && auxiliaryWeaponMount.TryGetComponent(out IRangedWeapon auxWep)) {
                _weaponArsenal.Add(WeaponSlot.Auxiliary, auxWep);
            }
            
            if (specialWeapon1Mount && specialWeapon1Mount.TryGetComponent(out IRangedWeapon specialWep)) {
                _weaponArsenal.Add(WeaponSlot.Special1, specialWep);
            }
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
            
            if (_weaponArsenal.TryGetValue(slot, out IRangedWeapon weapon))
                weapon.Fire();
            else
                Debug.LogWarning($"No weapon mounted on slot: {slot}");
        }
        
        /// <summary>
        /// Informs the weapon systems that the player has released the trigger.
        /// </summary>
        public void ReleaseTrigger(WeaponSlot slot = WeaponSlot.Main) {
            if (_weaponArsenal.TryGetValue(slot, out IRangedWeapon weapon)) {
                weapon.ReleaseTrigger(); 
            }
        }
    }
}