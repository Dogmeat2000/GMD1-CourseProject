using System;

namespace _01_Scripts.Core.Scoring
{
    // TODO Add class description
    [Serializable]
    public class ScoreEntry : IComparable<ScoreEntry>
    {
        public string playerName;
        public int score;

        // TODO Add description
        public ScoreEntry(string name, int scoreValue) {
            playerName = name;
            score = scoreValue;
        }
        
        // TODO Add description
        public int CompareTo(ScoreEntry other) {
            return other == null ? 1 : other.score.CompareTo(score);
        }
    }
    
    // TODO Consider moving into its own class
    // TODO Add class description
    [Serializable]
    public class LeaderboardSaveData {
        public System.Collections.Generic.List<ScoreEntry> entries = new();
    }
}
