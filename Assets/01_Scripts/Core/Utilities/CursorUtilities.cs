using UnityEngine;

namespace _01_Scripts.Core.Utilities
{
    // TODO Add description
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