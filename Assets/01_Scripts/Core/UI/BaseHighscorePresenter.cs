using UnityEngine;

namespace _01_Scripts.Core.UI
{
    public abstract class BaseHighscorePresenter : MonoBehaviour
    {
        [Header("Base Leaderboard Panels")]
        [Tooltip("The Layout Group container where rows are spawned")]
        [SerializeField] protected Transform leaderboardContainer;
        
        [Tooltip("Prefab for a standard, un-editable leaderboard row")]
        [SerializeField] protected GameObject staticScoreRowPrefab;

        protected void ClearBoard() {
            foreach (Transform child in leaderboardContainer) {
                Destroy(child.gameObject);
            }
        }

        protected void InjectStaticRow(int rank, string playerName, int score) {
            GameObject rowObj = Instantiate(staticScoreRowPrefab, leaderboardContainer);
            if (rowObj.TryGetComponent<Scoring.LeaderboardRow>(out var rowScript)) {
                rowScript.Initialize(rank, playerName, score);
            }
        }
    }
}
