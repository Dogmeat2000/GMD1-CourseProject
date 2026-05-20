using UnityEngine;

namespace _01_Scripts.Core.Combat
{
    /// <summary>
    /// Core methods required for warhead type objects.
    /// </summary>
    public abstract class Warhead : MonoBehaviour {
        [Header("Configuration")]
        [Tooltip("Which layers trigger the detonation? (e.g., PlayerShip, Structures)")]
        [SerializeField] 
        protected LayerMask validTargetLayers;
        
        [Tooltip("Optional: The composite VFX prefab to spawn upon detonation.")]
        [SerializeField] 
        protected GameObject explosionVfxPrefab;
        
        /// <summary>
        /// Holds the game object that fired this warhead.
        /// Especially useful for tracking which player should receive the score for a kill.
        /// </summary>
        public GameObject Instigator { get; set; }
        
        /// <summary>
        /// The impact this warhead has on the target. Implementation of this class need
        /// to specify whether this is damage, healing, or something else.
        /// </summary>
        public int ImpactAmount { get; set; }
    }
}
