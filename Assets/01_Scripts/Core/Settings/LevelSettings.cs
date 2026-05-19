using _01_Scripts.Core.Targeting;
using UnityEngine;

namespace _01_Scripts.Core.Settings
{
    [CreateAssetMenu(fileName = "NewLevelSettings", menuName = "Game/Settings/Level Settings", order = 1)]
    public class LevelSettings : ScriptableObject
    { 
        [Header("Environment")]
        [Tooltip("The Y-coordinate of the sea floor or underwater spawn origin")]
        [field: SerializeField] 
        public float SpawnDepthY { get; private set; } = -50f;
        
        [Tooltip("The Y-coordinate of the ocean surface for VFX triggers")]
        [field: SerializeField] 
        public float OceanSurfaceY { get; private set; } = 0f;

        [Header("Spawn Geometry")]
        [Tooltip("Minimum distance [m] from the main player to spawn an enemy")]
        [field: SerializeField] 
        public float MinSpawnDistance { get; private set; } = 500f;
        
        [Tooltip("Maximum distance [m] from the main player to spawn an enemy")]
        [field: SerializeField] 
        public float MaxSpawnDistance { get; private set; } = 1000f;
        
        [Tooltip("The maximum angle [degrees] to the left and right of the main player's forward vector.")]
        [Range(0, 180)]
        [field: SerializeField] 
        public float SpawnAngleLimit { get; private set; } = 60f;

        [Header("Wave Timing")]
        [Tooltip("Minimum seconds it takes to spawn all enemies in a wave")]
        [field: SerializeField] 
        public float WaveSpawnDurationMin { get; private set; } = 10f;
        
        [Tooltip("Maximum seconds it takes to spawn all enemies in a wave")]
        [field: SerializeField] 
        public float WaveSpawnDurationMax { get; private set; } = 30f;

        [Header("Enemy Specifics: Kamikaze Drone")]
        [Tooltip("Minimum altitude [m] the Kamikaze drone ascends to after breaching")]
        [field: SerializeField] 
        public float KamikazeBreachHeightMin { get; private set; } = 50f;
        
        [Tooltip("Maximum altitude [m] the Kamikaze drone ascends to after breaching")]
        [field: SerializeField] 
        public float KamikazeBreachHeightMax { get; private set; } = 150f;
        
        [Header("Enemy Specifics: BioSwarmer Drone")]
        [Tooltip("Minimum altitude [m] the drone ascends to after breaching")]
        [field: SerializeField] 
        public float BioSwarmerBreachHeightMin { get; private set; } = 75;
        
        [Tooltip("Maximum altitude [m] the drone ascends to after breaching")]
        [field: SerializeField] 
        public float BioSwarmerBreachHeightMax { get; private set; } = 200;

        [Header("Difficulty Multipliers")]
        [Tooltip("Budget multiplier for Easy difficulty")]
        [field: SerializeField] 
        public float EasyDifficultyMultiplier { get; private set; } = 0.75f;
        
        [Tooltip("Budget multiplier for Normal difficulty")]
        [field: SerializeField] 
        public float NormalDifficultyMultiplier { get; private set; } = 1.0f;
        
        [Tooltip("Budget multiplier for Hard difficulty")]
        [field: SerializeField] 
        public float HardDifficultyMultiplier { get; private set; } = 1.5f;
        
        [Tooltip("Budget multiplier for Nightmare difficulty")]
        [field: SerializeField] 
        public float NightmareDifficultyMultiplier { get; private set; } = 2.5f;
        
        [Header("Targeting Priorities (Lottery Weights)")]
        [Tooltip("Weight for Low priority targets (e.g., Decoy flares)")]
        [field: SerializeField] 
        public int PriorityWeightLow { get; private set; } = 1;

        [Tooltip("Weight for Standard priority targets (e.g., Player Frigates)")]
        [field: SerializeField] 
        public int PriorityWeightStandard { get; private set; } = 10;

        [Tooltip("Weight for High priority targets (e.g., Cargo Ships)")]
        [field: SerializeField] 
        public int PriorityWeightHigh { get; private set; } = 50;

        [Tooltip("Weight for Critical priority targets (e.g., Kamikaze Drones)")]
        [field: SerializeField] 
        public int PriorityWeightCritical { get; private set; } = 100;
        
        [Header("Player Configuration")]
        [Tooltip("Distance to horizon, where the targeting reticle is painted on the camera HUD")]
        [field: SerializeField] 
        public float MaxTargetingDistance { get; private set; } = 750f;

        [Header("Performance")]
        [Tooltip("Sets the initial default size an object pool may be.")]
        [field: SerializeField]
        public int DefaultObjectPoolSize { get; private set; } = 30;
        
        [Tooltip("Sets the maximum default size an object pool may be.")]
        [field: SerializeField]
        public int MaxDefaultObjectPoolSize { get; private set; } = 500;
        
        [Header("Allied AI Configuration")]
        [Tooltip("AI Turret Max Engagement Distance")]
        [field: SerializeField] 
        public float maxTargetingDistance = 350;
        
        [Tooltip("How often [s] the AI Turret sensor pings the radar to update target lists.")]
        [field: SerializeField] 
        public float scanInterval = 0.1f;

        /// <summary>
        /// Translates a target's priority label into its active mathematical weight.
        /// </summary>
        public int GetPriorityWeight(TargetPriority priorityLabel) {
            return priorityLabel switch {
                TargetPriority.Low => PriorityWeightLow,
                TargetPriority.Standard => PriorityWeightStandard,
                TargetPriority.High => PriorityWeightHigh,
                TargetPriority.Critical => PriorityWeightCritical,
                _ => PriorityWeightStandard
            };
        }
        
        /// <summary>
        /// Translates the active difficulty level into its mathematical multiplier.
        /// </summary>
        public float GetDifficultyMultiplier(GameDifficulty currentDifficulty) {
            return currentDifficulty switch {
                GameDifficulty.Easy => EasyDifficultyMultiplier,
                GameDifficulty.Normal => NormalDifficultyMultiplier,
                GameDifficulty.Hard => HardDifficultyMultiplier,
                GameDifficulty.Nightmare => NightmareDifficultyMultiplier,
                _ => NormalDifficultyMultiplier
            };
        }
        
        /// <summary>
        /// Unity callback to validate data integrity in the editor.
        /// Acts as a safety check to ensure Min values never exceed Max values.
        /// </summary> 
        private void OnValidate() {
            if (MinSpawnDistance > MaxSpawnDistance)
                MaxSpawnDistance = MinSpawnDistance;

            if (WaveSpawnDurationMin > WaveSpawnDurationMax)
                WaveSpawnDurationMax = WaveSpawnDurationMin;

            if (KamikazeBreachHeightMin > KamikazeBreachHeightMax)
                KamikazeBreachHeightMax = KamikazeBreachHeightMin;
            
            if (EasyDifficultyMultiplier <= 0.1f) 
                EasyDifficultyMultiplier = 0.1f;
            
            if (NormalDifficultyMultiplier <= 0.1f) 
                NormalDifficultyMultiplier = 0.1f;
            
            if (HardDifficultyMultiplier <= 0.1f) 
                HardDifficultyMultiplier = 0.1f;
            
            if (NightmareDifficultyMultiplier <= 0.1f) 
                NightmareDifficultyMultiplier = 0.1f;
            
            if (PriorityWeightLow <= 0) 
                PriorityWeightLow = 1;
            
            if (PriorityWeightStandard <= 0) 
                PriorityWeightStandard = 1;
            
            if (PriorityWeightHigh <= 0) 
                PriorityWeightHigh = 1;
            
            if (PriorityWeightCritical <= 0) 
                PriorityWeightCritical = 1;
        }
    }
}