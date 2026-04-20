using UnityEngine;
using UnityEngine.InputSystem;
using static _01_Scripts.Core.Utilities.CursorUtilities;

namespace _01_Scripts.Core.UI
{
    /// <summary>
    /// <p>This script handles cursor locking in game scenes. It should be loaded as part of the setup for each Game Level,
    /// and it will cause the users mouse / cursor to be hidden when the user first interacts with the game window.</p>
    /// <p>This is crucial for a pleasant operation of the turret aiming and shooting mechanics.</p>
    /// <p>When the user presses ESC it is disabled.</p>
    /// </summary>
    public class CursorManager : MonoBehaviour
    {
        private void Start() {
            LockAndHideCursor();
        }

        private void Update() {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Time.timeScale > 0f) {
                LockAndHideCursor();
            }
        }
    }
}