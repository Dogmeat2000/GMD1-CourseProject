using _01_Scripts._20_Features.Progression;
using UnityEngine;

namespace _01_Scripts._40_UI.Menus
{
    /// <summary>
    /// Responsible for presenting the basic Leaderboard.
    /// </summary>
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
            if (rowObj.TryGetComponent<LeaderboardRow>(out var rowScript))
                rowScript.Initialize(rank, playerName, score);
        }
    }
}
