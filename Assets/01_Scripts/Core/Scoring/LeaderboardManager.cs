using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Core.Scoring
{
    public class LeaderboardManager : MonoBehaviour
    { 
        public static LeaderboardManager Instance { get; private set; }

        [Header("Leaderboard Settings")]
        [SerializeField] private int maxLeaderboardSize = 10;

        private LeaderboardSaveData _saveData = new();
        private const string LEADERBOARD_SAVE_KEY = "Nereus_Leaderboard_SaveData";

        private void Awake() {
            // Singleton enforcement
            if (Instance && Instance != this) {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Persist across scenes
            DontDestroyOnLoad(gameObject); 
            LoadLeaderboard();
        }

        public void SubmitScore(string playerName, int score) {
            ScoreEntry newEntry = new ScoreEntry(playerName, score);
            _saveData.entries.Add(newEntry);
            _saveData.entries.Sort();
            
            if (_saveData.entries.Count > maxLeaderboardSize) {
                _saveData.entries.RemoveRange(maxLeaderboardSize, _saveData.entries.Count - maxLeaderboardSize);
            }
            SaveLeaderboard();
        }

        // TODO: Call this at round end to tell the player exactly where they stand
        public int GetProjectedRank(int scoreToCheck) {
            int rank = 1; 
            foreach (var entry in _saveData.entries) {
                if (scoreToCheck >= entry.score) return rank;
                rank++;
            }
            return rank;
        }

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
