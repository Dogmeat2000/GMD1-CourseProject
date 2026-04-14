using UnityEngine;

namespace _01_Scripts.Core.Enemies
{
    [CreateAssetMenu(fileName = "NewEnemyProfile", menuName = "Game/Enemies/Enemy Profile")]
    public class EnemyProfile : ScriptableObject
    {
        [Tooltip("The actual prefab to spawn (Must implement IPoolable)")]
        [field: SerializeField] 
        public GameObject Prefab { get; private set; }
        
        [Tooltip("How much of the Wave Budget this unit consumes")]
        [field: SerializeField] 
        public int ThreatCost { get; private set; } = 10;
        
        [Tooltip("Optional: A descriptive name for this enemy type")]
        [field: SerializeField] 
        public string UnitDesignation { get; private set; } = "Kamikaze Drone";
    }
}
