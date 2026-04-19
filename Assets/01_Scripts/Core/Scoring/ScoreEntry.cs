using System;

namespace _01_Scripts.Core.Scoring
{
    [Serializable]
    public class ScoreEntry : IComparable<ScoreEntry>
    {
        public string playerName;
        public int score;

        public ScoreEntry(string name, int scoreValue) {
            playerName = name;
            score = scoreValue;
        }
        
        public int CompareTo(ScoreEntry other) {
            return other == null ? 1 : other.score.CompareTo(score);
        }
    }
    
    [Serializable]
    public class LeaderboardSaveData {
        public System.Collections.Generic.List<ScoreEntry> entries = new();
    }
}
