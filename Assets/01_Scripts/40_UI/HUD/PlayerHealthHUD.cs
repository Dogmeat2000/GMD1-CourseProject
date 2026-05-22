using _01_Scripts._20_Features.Vitals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts._40_UI.HUD
{
    /// <summary>
    /// Presenter class responsible for displaying information relating to the health of a Player on the Player HUD.
    /// </summary>
    public class PlayerHealthHUD : MonoBehaviour
    {
        [Header("Telemetry Link")]
        [Tooltip("The HealthManager of the player's ship")]
        [SerializeField] private HealthManager playerHealth;

        [Header("Readout Displays")]
        [Tooltip("Text element for percentage health (e.g., '100%')")]
        [SerializeField] private TextMeshProUGUI healthText;
        
        [Tooltip("Color to use for Player health Slider and Text, when in good condition")]
        [SerializeField] private Color healthyColor = Color.cyan;
        
        [Tooltip("Color to use for Player health Slider and Text, when in worn condition")]
        [SerializeField] private Color warningColor = Color.yellow;
        
        [Tooltip("Color to use for Player health Slider and Text, when in critical condition")]
        [SerializeField] private Color criticalColor = Color.red;
        
        [Tooltip("Percentage (0.0 to 1.0) when the bar turns warningColor")]
        [Range(0f, 1f)]
        [SerializeField] private float warningThreshold = 0.5f;
        
        [Tooltip("Percentage (0.0 to 1.0) when the bar turns criticalColor")]
        [Range(0f, 1f)]
        [SerializeField] private float criticalThreshold = 0.25f;
        
        [Tooltip("Optional: A UI Image or Slider to visually represent hull integrity")]
        [SerializeField] private Slider healthSlider;
        
        [Tooltip("The Fill component of the slider, required for color shifting")]
        [SerializeField] private Image sliderFillImage;

        private void OnEnable() {
            if (!playerHealth) 
                return;
            
            playerHealth.OnHealthChanged += UpdateHealthDisplay;
            UpdateHealthDisplay(playerHealth.CurrentHealth, playerHealth.MaxHealth, null);
        }

        private void OnDisable() {
            if (playerHealth)
                playerHealth.OnHealthChanged -= UpdateHealthDisplay;
        }

        private void UpdateHealthDisplay(int currentHealth, int maxHealth, GameObject instigator) {
            float healthPercent = (float) currentHealth / maxHealth;
            
            if (healthText) {
                healthText.text = $"{(int) (healthPercent*100)}%";
                if (healthPercent <= criticalThreshold)
                    healthText.color = criticalColor;
                else if (healthPercent <= warningThreshold)
                    healthText.color = warningColor;
                else
                    healthText.color = healthyColor;
            }

            if (healthSlider) {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
            
            if (sliderFillImage) {
                if (healthPercent <= criticalThreshold)
                    sliderFillImage.color = criticalColor;
                else if (healthPercent <= warningThreshold)
                    sliderFillImage.color = warningColor;
                else
                    sliderFillImage.color = healthyColor;
            }
        }
    }
}
