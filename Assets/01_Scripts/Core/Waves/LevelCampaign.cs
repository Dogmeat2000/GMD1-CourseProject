using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Core.Waves
{
    // TODO This seems like repeating logic with shifting values! Refactor!
    // TODO: Consider moving this into another folder with scriptable objects?
    [CreateAssetMenu(fileName = "NewLevelCampaign", menuName = "Game/Waves/Level Campaign")]
    public class LevelCampaign : ScriptableObject {
        [Tooltip("The sequential list of waves for this specific level")]
        [field: SerializeField] public List<WaveData> Waves { get; set; }
    }
}