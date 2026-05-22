using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._20_Features.Progression
{
    /// <summary>
    /// Configuration class for how each campaign/mission should progress in difficulty and encounters across waves of attack.
    /// </summary>
    [CreateAssetMenu(fileName = "NewLevelCampaign", menuName = "Game/Waves/Level Campaign")]
    public class LevelCampaign : ScriptableObject {
        [Tooltip("The sequential list of waves for this specific level")]
        [field: SerializeField] public List<WaveData> Waves { get; set; }
    }
}