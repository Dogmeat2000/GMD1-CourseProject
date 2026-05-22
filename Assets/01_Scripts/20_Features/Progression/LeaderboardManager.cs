using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._20_Features.Progression
{
    /// <summary>
    /// Managers the LeaderBoard.
    /// </summary>
    public class LeaderboardManager : MonoBehaviour
    { 
        /// <summary>
        /// Returns a Singleton reference to this Manager.
        /// </summary>
        public static LeaderboardManager Instance { get; private set; }

        [Header("Leaderboard Settings")]
        [Tooltip("Maximum number of entries to show on the leaderboard")]
        [SerializeField] private int maxLeaderboardSize = 10;

        private LeaderboardSaveData _saveData = new();
        private const string LEADERBOARD_SAVE_KEY = "Nereus_Leaderboard_SaveData";

        private void Awake() {
            if (Instance && Instance != this) {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            
            DontDestroyOnLoad(gameObject); 
            LoadLeaderboard();
        }

        /// <summary>
        /// Submits a finalized player score to the persistent leaderboard data structure.
        /// </summary>
        public void SubmitScore(string playerName, int score) {
            ScoreEntry newEntry = new ScoreEntry(playerName, score);
            _saveData.entries.Add(newEntry);
            _saveData.entries.Sort();
            
            if (_saveData.entries.Count > maxLeaderboardSize)
                _saveData.entries.RemoveRange(maxLeaderboardSize, _saveData.entries.Count - maxLeaderboardSize);
            SaveLeaderboard();
        }

        /// <summary>
        /// Returns a projected Rank that the provided score would receive, compared against the current Leaderboard.
        /// </summary>
        /// <param name="scoreToCheck"></param>
        /// <returns></returns>
        public int GetProjectedRank(int scoreToCheck) {
            int rank = 1; 
            foreach (var entry in _saveData.entries) {
                if (scoreToCheck >= entry.score) return rank;
                rank++;
            }
            return rank;
        }

        /// <summary>
        /// Returns an immutable list of the current top scores on this Leaderboard.
        /// </summary>
        /// <returns></returns>
        public IReadOnlyList<ScoreEntry> GetTopScores() {
            return _saveData.entries.AsReadOnly();
        }
        
        private void SaveLeaderboard() {
            string json = JsonUtility.ToJson(_saveData, true);
            PlayerPrefs.SetString(LEADERBOARD_SAVE_KEY, json);
            PlayerPrefs.Save();
        }

        private void LoadLeaderboard() {
            if (PlayerPrefs.HasKey(LEADERBOARD_SAVE_KEY)) {
                string json = PlayerPrefs.GetString(LEADERBOARD_SAVE_KEY);
                _saveData = JsonUtility.FromJson<LeaderboardSaveData>(json);
            } else {
                _saveData = new LeaderboardSaveData();
            }
        }
    }
}
