using System;
using UnityEngine;

namespace _01_Scripts.Core.Scoring
{
    public class PlayerScore : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Temporary ID (e.g., 'Player 1') before they enter a custom name")]
        [SerializeField] private string defaultPlayerName = "Unknown";
        
        private int _currentScore;

        // The HUD subscribes to this to update the UI instantly
        public event Action<int> OnScoreChanged;

        public int CurrentScore => _currentScore;

        public void AddScore(int points) {
            if (points <= 0) return;
            
            _currentScore += points;
            OnScoreChanged?.Invoke(_currentScore);
        }

        // TODO: Called at the end of the round by your Game Manager/UI
        public void CommitScore(string customName = "") {
            string finalName = string.IsNullOrWhiteSpace(customName) ? defaultPlayerName : customName;
            LeaderboardManager.Instance.SubmitScore(finalName, _currentScore);
        }
    }
}
