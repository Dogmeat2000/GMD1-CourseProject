using TMPro;
using UnityEngine;

namespace _01_Scripts.Core.Scoring
{
    public class LeaderboardRow : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI rankText;
        
        [SerializeField] 
        private TextMeshProUGUI nameText;
        
        [SerializeField] 
        private TextMeshProUGUI scoreText;

        public void Initialize(int rank, string playerName, int score) {
            rankText.text = $"{rank}.";
            nameText.text = playerName;
            scoreText.text = score.ToString("N0"); 
        }
    }
}
