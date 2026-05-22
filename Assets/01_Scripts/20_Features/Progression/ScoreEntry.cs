using System;
using System.Collections.Generic;

namespace _01_Scripts._20_Features.Progression
{
    /// <summary>
    /// Serializable class that is used to persist each Score entry, that is used in the Leaderboard rankings.
    /// </summary>
    [Serializable]
    public class ScoreEntry : IComparable<ScoreEntry>
    {
        public string playerName;
        public int score;
        
        public ScoreEntry(string name, int scoreValue) {
            playerName = name;
            score = scoreValue;
        }
        
        /// <summary>
        /// Compare this Score Entry to another. Returns indicate whether this score should be sorted AFTER the other score (Descending).
        /// <p>Return larger than 0: This score should be sorted after the other (it is smaller).</p>
        /// <p>Return equal to 0: Both scores are equal.</p>
        /// <p>Return less than 0: This score should be sorter before the other (it is larger).</p>
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ScoreEntry other) {
            return other == null ? 1 : other.score.CompareTo(score);
        }
    }
    
    [Serializable]
    public class LeaderboardSaveData {
        public List<ScoreEntry> entries = new();
    }
}
