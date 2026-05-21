using UnityEngine;

namespace _01_Scripts.Core.Utilities
{
    /// <summary>
    /// Cursor utility helper class.
    /// </summary>
    public static class CursorUtilities
    { 
        /// <summary>
        /// Locks and hides the cursor.
        /// </summary>
        public static void LockAndHideCursor() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        /// <summary>
        /// Unlocks and displays the cursor.
        /// </summary>
        public static void UnlockAndShowCursor() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}