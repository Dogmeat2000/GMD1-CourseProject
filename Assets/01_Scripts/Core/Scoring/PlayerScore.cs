using System;
using UnityEngine;

namespace _01_Scripts.Core.Scoring
{
    /// <summary>
    /// Represents a single Players Score.
    /// </summary>
    public class PlayerScore : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Temporary ID (e.g., 'Player 1') before they enter a custom name")]
        [SerializeField] private string defaultPlayerName = "Unknown";
        
        private int _currentScore;

        /// <summary>
        /// Broadcast whenever the players score changes.
        /// Attribute: The new score of the player.
        /// </summary>
        public event Action<int> OnScoreChanged;

        /// <summary>
        /// Property containing a reference to the current score of this Player.
        /// </summary>
        public int CurrentScore => _currentScore;
        
        /// <summary>
        /// Property that holds the default name of this Player (i.e Player1 or Player2)
        /// </summary>
        public string DefaultPlayerName => defaultPlayerName;

        /// <summary>
        /// Adds the given amount of points to this Player's Score.
        /// </summary>
        /// <param name="points"></param>
        public void AddScore(int points) {
            if (points <= 0) 
                return;
            
            _currentScore += points;
            OnScoreChanged?.Invoke(_currentScore);
        }
        
        /// <summary>
        /// Submits this Score to the Leaderboard.
        /// If no CustomName is provided, it just commits the score under the Default Player Name.
        /// </summary>
        /// <param name="customName"></param>
        public void CommitScore(string customName = "") {
            string finalName = string.IsNullOrWhiteSpace(customName) ? defaultPlayerName : customName;
            LeaderboardManager.Instance.SubmitScore(finalName, _currentScore);
        }
    }
}
