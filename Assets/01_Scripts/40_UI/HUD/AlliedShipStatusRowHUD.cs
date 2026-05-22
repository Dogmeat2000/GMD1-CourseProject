using _01_Scripts._20_Features.Vitals;
using _01_Scripts._30_Actors.Ships;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts._40_UI.HUD
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
        [Tooltip("The Image GameObject to use as HUD Icon for this ship")]
        [SerializeField] private Image shipIconImage;
        
        [Tooltip("The TextMeshPro GameObject that should display the ship name in the HUD")]
        [SerializeField] private TextMeshProUGUI shipNameText;
        
        [Tooltip("The TextMeshPro GameObject that should display the ship health percentage in the HUD")]
        [SerializeField] private TextMeshProUGUI healthPercentageText;
        
        [Tooltip("The Slider GameObject that should show how much health this ship has left, in the HUD")]
        [SerializeField] private Slider healthSlider;
        
        [Tooltip("The Image that should be used to fill in the slider for the ship health in the HUD")]
        [SerializeField] private Image sliderFillImage;

        [Header("Health Thresholds")]
        [Tooltip("Color to use for Ship health Slider and Text, when in good condition")]
        [SerializeField] private Color healthyColor = Color.cyan;
        
        [Tooltip("Color to use for Ship health Slider and Text, when in worn condition")]
        [SerializeField] private Color warningColor = Color.yellow;
        
        [Tooltip("Color to use for Ship health Slider and Text, when in critical condition")]
        [SerializeField] private Color criticalColor = Color.red;
        
        [Tooltip("Which color to repaint the Ship HUD Icon with, when this ship is destroyed")]
        [SerializeField] private Color destroyedColor = new (0.3f, 0.3f, 0.3f, 0.5f);

        [Tooltip("When to begin using the Warning Color (0.7 = 70% health remaining)")]
        [Range(0f, 1f)] 
        [SerializeField] private float warningThreshold = 0.7f;
        
        [Tooltip("When to begin using the Critical Color (0.3 = 30% health remaining)")]
        [Range(0f, 1f)] 
        [SerializeField] private float criticalThreshold = 0.3f;

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
                shipIconImage.color = destroyedColor;
            }
        }
    }
}
