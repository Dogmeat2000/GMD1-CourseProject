using _01_Scripts._10_Core.Persistence;
using UnityEngine;

namespace _01_Scripts._20_Features.Progression
{
    public class LevelManager : MonoBehaviour, ILevelManager
    {
        [Header("Level Configuration")]
        [Tooltip("Slot the active LevelSettings ScriptableObject here")]
        [SerializeField] private LevelSettings activeSettings;
        
        [Header("Active Session State")]
        [Tooltip("Number of active players (e.g., 1 for Solo, 2 for Split-Screen)")]
        [field: SerializeField] public int ActivePlayerCount { get; set; } = 1;

        [Tooltip("The difficulty selected by the host")]
        [field: SerializeField] public GameDifficulty CurrentDifficulty { get; set; } = GameDifficulty.Normal;
        
        public LevelSettings Settings => activeSettings;
        
        public float GetDifficultyMultiplier() {
            if (!Settings) {
                Debug.LogError("Active Settings is NULL. Defaulting multiplier to 1.0f.");
                return 1.0f;
            }
            
            return activeSettings.GetDifficultyMultiplier(CurrentDifficulty);
        }
    }
}