using _01_Scripts.Core.Scoring;
using UnityEngine;

namespace _01_Scripts.Core.UI
{
    public class MainMenuHighscorePresenter : BaseHighscorePresenter
    {
        private void OnEnable() {
            GenerateStaticLeaderboard();
        }

        public void GenerateStaticLeaderboard() {
            if (!LeaderboardManager.Instance) {
                Debug.LogWarning("LeaderboardManager is missing in Main Menu Scene.");
                return;
            }

            ClearBoard();

            var topScores = LeaderboardManager.Instance.GetTopScores();
            
            int currentRank = 1;
            int previousScore = -1;

            for (int i = 0; i < 10; i++) {
                if (i < topScores.Count) {
                    var data = topScores[i];
                    
                    if (i > 0 && data.score != previousScore) {
                        currentRank = i + 1;
                    }

                    InjectStaticRow(currentRank, data.playerName, data.score);
                    previousScore = data.score;
                } else {
                    InjectStaticRow(i + 1, "---", 0);
                }
            }
        }
    }
}
