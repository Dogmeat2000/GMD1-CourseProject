using UnityEngine;

namespace _01_Scripts.Core.UI.HUD
{
    // TODO Add description
    public class PlayerDeathHUDPresenter : MonoBehaviour
    {
        [Header("Config")]
        [Tooltip("The HealthManager of the player's ship")]
        [SerializeField] private HealthManager playerHealth;

        [Header("UI Elements")]
        [Tooltip("The 'YOU ARE DEAD' Canvas Group or Panel")]
        [SerializeField] private GameObject deathScreenPanel;

        private void OnEnable() {
            if (deathScreenPanel) 
                deathScreenPanel.SetActive(false);
                
            if (playerHealth) 
                playerHealth.OnZeroHealth += ExecuteDeathScreen;
        }

        private void OnDisable() {
            if (playerHealth) 
                playerHealth.OnZeroHealth -= ExecuteDeathScreen;
        }

        private void ExecuteDeathScreen(HealthManager source, GameObject killer) {
            if (deathScreenPanel) 
                deathScreenPanel.SetActive(true);
        }
    }
}
