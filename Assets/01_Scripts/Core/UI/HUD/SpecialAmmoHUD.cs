using System.Collections.Generic;
using _01_Scripts.Turrets;
using UnityEngine;

namespace _01_Scripts.Core.UI.HUD
{
    /// <summary>
    /// Dynamically generates and manages a visual array of ammunition icons.
    /// </summary>
    public class SpecialAmmoHUD : MonoBehaviour
    { 
        [Header("Setup")]
        [Tooltip("The SpecialWeaponsSystem attached to the player's turret.")]
        [SerializeField] private SpecialWeaponsSystem targetWeapon;
        
        [Tooltip("The UI Panel equipped with a Vertical Layout Group.")]
        [SerializeField] private Transform iconContainer;
        
        [Tooltip("The UI Image Prefab representing a single missile.")]
        [SerializeField] private GameObject ammoIconPrefab;

        private readonly List<GameObject> _spawnedIcons = new List<GameObject>();
        private bool _isInitialized = false;

        private void OnEnable() {
            if (targetWeapon) 
                targetWeapon.OnAmmoChanged += UpdateAmmoDisplay;
        }

        private void OnDisable() {
            if (targetWeapon) 
                targetWeapon.OnAmmoChanged -= UpdateAmmoDisplay;
        }

        private void UpdateAmmoDisplay(int currentAmmo, int maxAmmo) {
            if (!_isInitialized) {
                InitializeIcons(maxAmmo);
            }
            
            for (int i = 0; i < _spawnedIcons.Count; i++) {
                _spawnedIcons[i].SetActive(i < currentAmmo);
            }
        }

        private void InitializeIcons(int maxAmmo) {
            foreach (Transform child in iconContainer) {
                Destroy(child.gameObject);
            }
            _spawnedIcons.Clear();
            
            for (int i = 0; i < maxAmmo; i++) {
                GameObject iconObj = Instantiate(ammoIconPrefab, iconContainer);
                _spawnedIcons.Add(iconObj);
            }
            
            _isInitialized = true;
        }
    }
}
