using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Core.UI.HUD
{
    public class PlayerHealthHUD : MonoBehaviour
    {
        [Header("Telemetry Link")]
        [Tooltip("The HealthManager of the player's ship")]
        [SerializeField] 
        private HealthManager playerHealth;

        [Header("Readout Displays")]
        [Tooltip("Text element for numeric health (e.g., 'HULL: 850 / 1000')")]
        [SerializeField] 
        private TextMeshProUGUI healthText;
        
        [Tooltip("Optional: A UI Image or Slider to visually represent hull integrity")]
        [SerializeField] 
        private Slider healthSlider;

        private void OnEnable() {
            if (!playerHealth) 
                return;
            
            playerHealth.OnHealthChanged += UpdateHealthDisplay;
            
            UpdateHealthDisplay(playerHealth.CurrentHealth, playerHealth.MaxHealth, null);
        }

        private void OnDisable() {
            if (playerHealth) {
                playerHealth.OnHealthChanged -= UpdateHealthDisplay;
            }
        }

        private void UpdateHealthDisplay(int currentHealth, int maxHealth, GameObject instigator) {
            if (healthText) {
                healthText.text = $"HULL: {currentHealth} / {maxHealth}";
                healthText.color = ((float)currentHealth / maxHealth) <= 0.25f ? Color.red : Color.white;
            }

            if (healthSlider) {
                healthSlider.maxValue = maxHealth;
                healthSlider.value = currentHealth;
            }
        }
    }
}
