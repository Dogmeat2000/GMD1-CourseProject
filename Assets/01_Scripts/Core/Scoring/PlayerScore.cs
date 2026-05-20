using System;
using UnityEngine;

namespace _01_Scripts.Core.Scoring
{
    // TODO Add class description
    public class PlayerScore : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Temporary ID (e.g., 'Player 1') before they enter a custom name")]
        [SerializeField] private string defaultPlayerName = "Unknown";
        
        private int _currentScore;

        // TODO Add description
        public event Action<int> OnScoreChanged;

        // TODO Add description
        public int CurrentScore => _currentScore;
        
        // TODO Add description
        public string DefaultPlayerName  => defaultPlayerName;

        // TODO Add description
        public void AddScore(int points) {
            if (points <= 0) 
                return;
            
            _currentScore += points;
            OnScoreChanged?.Invoke(_currentScore);
        }
        
        // TODO Add description
        public void CommitScore(string customName = "") {
            string finalName = string.IsNullOrWhiteSpace(customName) ? defaultPlayerName : customName;
            LeaderboardManager.Instance.SubmitScore(finalName, _currentScore);
        }
    }
}
