using System;

namespace _01_Scripts.Core.Scoring
{
    [Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;
        public string dateAchieved;

        public ScoreEntry(string name, int scoreValue) {
            playerName = name;
            score = scoreValue;
            dateAchieved = DateTime.Now.ToString("yyyy-MM-dd HH:mm"); 
        }
        
        public int CompareTo(ScoreEntry other) {
            return other.score.CompareTo(this.score); 
        }
    }
    
    [Serializable]
    public class LeaderboardSaveData {
        public System.Collections.Generic.List<ScoreEntry> entries = new();
    }
}
