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
            for (int displayRank = 1; displayRank <= 10; displayRank++) {
                if (displayRank <= topScores.Count) {
                    var data = topScores[displayRank - 1];
                    InjectStaticRow(displayRank, data.playerName, data.score);
                } else {
                    InjectStaticRow(displayRank, "---", 0);
                }
            }
        }
    }
}
