using UnityEngine;

namespace _01_Scripts._10_Core.Utilities
{
    /// <summary>
    /// Utility class for helper methods relating to Layers in Unity.
    /// </summary>
    public static class LayerMaskUtilities
    {
        /// <summary>
        /// Checks if a specific layer index is included within this LayerMask.
        /// </summary>
        public static bool Contains(this LayerMask mask, int layer) {
            // Bitwise math
            return (mask.value & (1 << layer)) != 0;
        }
    }
}