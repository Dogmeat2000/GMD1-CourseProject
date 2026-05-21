using _01_Scripts.Turrets;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Core.UI.HUD
{
    /// <summary>
    /// Presenter class responsible for presenting information about the Players Capacitor on the Player HUD.
    /// </summary>
    public class PlayerCapacitorHUD : MonoBehaviour
    { 
        [Header("Telemetry Link")]
        [Tooltip("The TurretCapacitor of the player's weapon")]
        [SerializeField] private TurretCapacitor capacitor;

        [Header("Readout Displays")]
        [Tooltip("Color to use while this Capacitor has a good amounts of energy left")]
        [SerializeField] private Color healthyColor = Color.yellow;
        
        [Tooltip("Color to use while this Capacitor has a mediocre amounts of energy left")]
        [SerializeField] private Color warningColor = Color.orange;
        
        [Tooltip("Color to use while this Capacitor has a very low amounts of energy left")]
        [SerializeField] private Color criticalColor = Color.red;
        
        [Tooltip("Percentage (0.0 to 1.0) when the bar turns warningColor")]
        [Range(0f, 1f)]
        [SerializeField] private float warningThreshold = 0.5f;
        
        [Tooltip("Percentage (0.0 to 1.0) when the bar turns criticalColor")]
        [Range(0f, 1f)]
        [SerializeField] private float criticalThreshold = 0.25f;
        
        [Tooltip("Optional: A UI Image or Slider to visually represent capacity")]
        [SerializeField] private Slider slider;
        
        [Tooltip("The Fill component of the slider, required for color shifting")]
        [SerializeField] private Image sliderFillImage;

        private void OnEnable() {
            if (!capacitor) 
                return;
            
            capacitor.OnEnergyPercentageChanged += UpdateCapacitorDisplay;
        }

        private void OnDisable() {
            if (capacitor)
                capacitor.OnEnergyPercentageChanged += UpdateCapacitorDisplay;
        }

        private void UpdateCapacitorDisplay(float currentCapacityPercentage) {
            if (slider) {
                slider.maxValue = 1;
                slider.value = currentCapacityPercentage;
            }
            
            if (sliderFillImage) {
                if (currentCapacityPercentage <= criticalThreshold)
                    sliderFillImage.color = criticalColor;
                else if (currentCapacityPercentage <= warningThreshold)
                    sliderFillImage.color = warningColor;
                else
                    sliderFillImage.color = healthyColor;
            }
        }
    }
}
