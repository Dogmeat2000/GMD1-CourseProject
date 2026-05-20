using _01_Scripts.Core.Entities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Core.UI.HUD
{
    /// <summary>
    /// Controller for an individual allied ship's readout on the Fleet Status HUD.
    /// Extracts identity data automatically and monitors health status.
    /// </summary>
    public class AlliedShipStatusRow : MonoBehaviour
    {
        [Header("Telemetry Link")]
        [Tooltip("Link to the actual allied ship's HealthManager from the scene.")]
        [SerializeField] private HealthManager targetShip;

        [Header("UI Bindings")]
        // TODO Add description
        [SerializeField] private Image shipIconImage;
        
        // TODO Add description
        [SerializeField] private TextMeshProUGUI shipNameText;
        
        // TODO Add description
        [SerializeField] private TextMeshProUGUI healthPercentageText;
        
        // TODO Add description
        [SerializeField] private Slider healthSlider;
        
        // TODO Add description
        [SerializeField] private Image sliderFillImage;

        [Header("Health Thresholds")]
        // TODO Add description
        [SerializeField] private Color healthyColor = Color.cyan;
        
        // TODO Add description
        [SerializeField] private Color warningColor = Color.yellow;
        
        // TODO Add description
        [SerializeField] private Color criticalColor = Color.red;

        // TODO Add description
        [Range(0f, 1f)] 
        [SerializeField] private float warningThreshold = 0.5f;
        
        // TODO Add description
        [Range(0f, 1f)] 
        [SerializeField] private float criticalThreshold = 0.25f;

        private void OnEnable() {
            if (!targetShip) 
                return;
            
            if (targetShip.TryGetComponent<ShipIdentity>(out var identity)) {
                if (shipNameText) 
                    shipNameText.text = identity.DisplayName;
                
                if (shipIconImage && identity.HudIcon) 
                    shipIconImage.sprite = identity.HudIcon;
                
            } else {
                Debug.LogWarning($"No ShipIdentity found on {targetShip.name}. Cannot extract Name/Icon.");
            }
            
            targetShip.OnHealthChanged += UpdateHealthDisplay;
            targetShip.OnZeroHealth += HandleDestruction;
            UpdateHealthDisplay(targetShip.CurrentHealth, targetShip.MaxHealth, null);
        }

        private void OnDisable() {
            if (targetShip) {
                targetShip.OnHealthChanged -= UpdateHealthDisplay;
                targetShip.OnZeroHealth -= HandleDestruction;
            }
        }

        private void UpdateHealthDisplay(int currentHealth, int maxHealth, GameObject instigator) {
            float healthPercent = (float) currentHealth / maxHealth;

            if (healthPercentageText)
                healthPercentageText.text = $"[ {(int) (healthPercent * 100)}% ]";

            if (healthSlider) {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
            
            if (sliderFillImage) {
                Color targetColor = healthyColor;
                
                if (healthPercent <= criticalThreshold)
                    targetColor = criticalColor;
                else if (healthPercent <= warningThreshold)
                    targetColor = warningColor;

                sliderFillImage.color = targetColor;
                
                if (healthPercentageText) 
                    healthPercentageText.color = targetColor;
            }
        }

        private void HandleDestruction(HealthManager source, GameObject killer) {
            if (healthPercentageText) {
                healthPercentageText.text = "[ DESTROYED ]";
                healthPercentageText.color = criticalColor; 
            }

            if (sliderFillImage) 
                sliderFillImage.color = criticalColor;
            
            if (healthSlider) 
                healthSlider.value = 0;
            
            if (shipIconImage) {
                shipIconImage.color = new Color(0.3f, 0.3f, 0.3f, 0.5f); // TODO Perhaps make this a serialized field, so the "destroyed" color can be set in the inspector?
            }
        }
    }
}
