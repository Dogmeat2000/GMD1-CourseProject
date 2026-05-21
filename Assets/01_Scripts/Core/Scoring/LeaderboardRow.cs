using TMPro;
using UnityEngine;

namespace _01_Scripts.Core.Scoring
{
    /// <summary>
    /// A standard non-interactable row on the Leaderboard.
    /// </summary>
    public class LeaderboardRow : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("TextMeshPro object to display rank within")]
        [SerializeField] private TextMeshProUGUI rankText;
        
        [Tooltip("TextMeshPro object to display player name within")]
        [SerializeField] private TextMeshProUGUI nameText;
        
        [Tooltip("TextMeshPro object to display player score within")]
        [SerializeField] private TextMeshProUGUI scoreText;

        public void Initialize(int rank, string playerName, int score) {
            rankText.text = $"{rank}.";
            nameText.text = playerName;
            scoreText.text = score.ToString("N0"); 
        }
    }
}
