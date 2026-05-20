using UnityEngine;

namespace _01_Scripts.Turrets
{
    /// <summary>
    /// Acts as the hardpoint on a ship hull where a turret can be dynamically instantiated.
    /// </summary>
    public class TurretHardpoint : MonoBehaviour
    {
        [Header("Specifications")]
        [Tooltip("The maximum size of weapon this hardpoint can support.")]
        public TurretClass maxSupportedSize = TurretClass.Main;
        
        /// <summary>
        /// Verifies if the requested weapon class can fit on this hardpoint.
        /// </summary>
        public bool CanEquip(TurretClass weaponClass) {
            if (transform.childCount > 0) 
                return false;
            
            if (weaponClass == TurretClass.Main && maxSupportedSize == TurretClass.Auxiliary)
                return false; 
            
            return true;
        }
        
        /// <summary>
        /// Instantiates the provided turret prefab as a child of this hardpoint.
        /// </summary>
        public GameObject EquipTurret(GameObject turretPrefab, TurretClass weaponClass) {
            if (!CanEquip(weaponClass)) {
                Debug.LogWarning($"Cannot equip a {weaponClass} weapon on a {maxSupportedSize} hardpoint at {gameObject.name}.");
                return null;
            }
            
            return Instantiate(turretPrefab, transform.position, transform.rotation, transform);
        }
    }
}
