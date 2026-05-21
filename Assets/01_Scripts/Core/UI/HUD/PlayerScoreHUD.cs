using _01_Scripts.Core.Scoring;
using TMPro;
using UnityEngine;

namespace _01_Scripts.Core.UI.HUD
{
    /// <summary>
    /// Presenter class responsible for displaying each Player's score on the screen.
    /// </summary>
    public class PlayerScoreHUD : MonoBehaviour
    {
        [Header("Telemetry Links")]
        [Tooltip("The score tracker attached to the player's turret")]
        [SerializeField] private PlayerScore targetPlayer;
        
        [Tooltip("The TextMeshPro element displaying the score on screen")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Formatting")]
        [Tooltip("Optional prefix, e.g., 'P1 Score: '")]
        [SerializeField] private string prefix = "Score: ";

        private void OnEnable() {
            if (!targetPlayer) 
                return;
            
            targetPlayer.OnScoreChanged += UpdateScoreDisplay;
            UpdateScoreDisplay(targetPlayer.CurrentScore);
        }

        private void OnDisable() {
            if (targetPlayer)
                targetPlayer.OnScoreChanged -= UpdateScoreDisplay;
        }

        private void UpdateScoreDisplay(int newScore) {
            if (scoreText)
                scoreText.text = $"{prefix}{newScore:D6}";
        }
    }
}
