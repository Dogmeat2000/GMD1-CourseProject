using UnityEngine;

namespace _01_Scripts.Core.Utilities
{
    // TODO Add description
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