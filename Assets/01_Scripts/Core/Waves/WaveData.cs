using System;
using System.Collections.Generic;
using _01_Scripts.Core.Enemies;
using UnityEngine;

namespace _01_Scripts.Core.Waves
{
    [Serializable]
    public struct WaveData
    {
        [Tooltip("Total points the Director can spend on this wave")]
        [field: SerializeField]
        public int ThreatBudget { get; set; }
        
        [Tooltip("The types of enemies allowed to spawn in this wave")]
        [field: SerializeField]
        public List<EnemyProfile> AllowedEnemies { get; set; }
    }
}