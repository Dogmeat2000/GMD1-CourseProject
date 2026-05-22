using UnityEngine;

namespace _01_Scripts._30_Actors.Enemies
{
    [CreateAssetMenu(fileName = "NewEnemyProfile", menuName = "Game/Enemies/Enemy Profile")]
    public class EnemyProfile : ScriptableObject
    {
        [field: Header("Configuration")]
        [field: Tooltip("The actual prefab to spawn (Must implement IPoolable)")]
        [field: SerializeField] public GameObject Prefab { get; private set; }
        
        [field: Tooltip("The speed this entity breaches the water surface with")]
        [field: SerializeField] public float breachSpeed = 100f;
        
        [field: Tooltip("The speed this entity pursues player ships with")]
        [field: SerializeField] public float pursuitSpeed = 30f;
        
        [field: Tooltip("The speed this entity turns with")]
        [field: SerializeField] public float turnSpeed = 10f;
        
        [field: Tooltip("Which layer do allies of this entity belong to?")]
        [field: SerializeField] public LayerMask allyLayer;
        
        [field: Tooltip("The absolute minimum Y altitude before the entity pulls up.")]
        [field: SerializeField] public float hardDeckAltitude = 2.0f; 
        
        [field: Tooltip("How much of the Wave Budget this unit consumes")]
        [field: SerializeField] public int ThreatCost { get; private set; } = 10;
        
        [field: Tooltip("The number of High Score points, this type of Enemy is worth")]
        [field: SerializeField] public int pointValue = 10;
        
        [field: Tooltip("The base damage this unit inflicts on collision. Scales with difficulty.")]
        [field: Range(0f, 500f)]
        [field: SerializeField] public int baseCollisionDamage = 25;
        
        [field: Tooltip("The exact name of the Animator state this entity should snap to upon spawning.")]
        [field: SerializeField] public string defaultRespawnState = "Idle";
        
        [field: Tooltip("The exact name of the Animator trigger parameter to fire when health reaches zero.")]
        [field: SerializeField] public string deathTriggerName = "Die";
        
        [field: Tooltip("List of any Animator Triggers (i.e. 'Hit' or 'Shoot') that must be aborted when this unit dies.")]
        [field: SerializeField] public string[] interruptTriggersToPurge = { "Hit", "Shoot" };
        
        [field: Tooltip("Optional: A particle system to spawn when the death animation finishes (e.g., explosion, digital fade, blood splatter).")]
        [field: SerializeField] public GameObject deathVfxPrefab;
        
        [field: Tooltip("Optional: A descriptive name for this enemy type")]
        [field: SerializeField] public string UnitDesignation { get; private set; } = "Unknown Enemy";
    }
}
