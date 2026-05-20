using UnityEngine;

namespace _01_Scripts.Core.UI.HUD
{
    // TODO Add description
    public class FloatingHealthBar : MonoBehaviour
    {
        [Header("Telemetry Link")]
        [Tooltip("The HealthManager to track")]
        [SerializeField] private HealthManager targetHealth;
        
        [Header("Visual Components")]
        [Tooltip("The 3D sprite acting as the health block")]
        [SerializeField] private SpriteRenderer healthSprite;
        
        [Tooltip("The color transition from 0% health (left) to 100% health (right)")]
        [SerializeField] private Gradient healthGradient;
        
        private void OnEnable() {
            if (!targetHealth) 
                return;
            
            targetHealth.OnHealthChanged += UpdateHealthBar;
            UpdateHealthBar(targetHealth.CurrentHealth, targetHealth.MaxHealth, null);
        }

        private void OnDisable() {
            if (targetHealth)
                targetHealth.OnHealthChanged -= UpdateHealthBar;
        }

        private void UpdateHealthBar(int currentHealth, int maxHealth, GameObject instigator) {
            if (!healthSprite) 
                return;
            
            bool shouldShow = currentHealth < maxHealth && currentHealth > 0;
            healthSprite.enabled = shouldShow;
            
            if (!shouldShow) 
                return;
            
            float healthPercentage = (float) currentHealth / maxHealth;
            healthSprite.color = healthGradient.Evaluate(healthPercentage);
        }
    }
}