using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static _01_Scripts.Core.Utilities.CursorUtilities;

namespace _01_Scripts.Core.UI
{
    /**
     * <p>Handles the Pause Menu, that can be activated while a game level is in progress</p>
     */
    public class PauseManager : MonoBehaviour
    {
        [Header("UI Overlay")]
        [Tooltip("The Panel to be displayed when Pause is invoked")]
        [SerializeField] 
        private GameObject pauseMenuPanel;
        
        [Header("Scene Navigation")]
        [Tooltip("The exact name of the Main Menu scene to load upon exit")]
        [SerializeField] 
        private string mainMenuSceneName = "SCN_MainMenu";

        private bool _isPaused = false;

        private void Start() {
            
            if (UnityEngine.EventSystems.EventSystem.current) {
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            }
            
            if (pauseMenuPanel) {
                pauseMenuPanel.SetActive(false);
            }
            
            Time.timeScale = 1f;
            _isPaused = false;
            LockAndHideCursor();
        }

        private void Update() {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame) {
                TogglePause();
            }
        }

        public void TogglePause() {
            _isPaused = !_isPaused;

            if (_isPaused) {
                Time.timeScale = 0f;
                pauseMenuPanel.SetActive(true);
                UnlockAndShowCursor();
            } else {
                Time.timeScale = 1f;
                pauseMenuPanel.SetActive(false);
                LockAndHideCursor();
            }
        }
        
        public void ResumeGame() {
            if (_isPaused) TogglePause();
        }
        
        public void ReturnToMainMenu() {
            Time.timeScale = 1f;
            SceneManager.LoadSceneAsync(mainMenuSceneName);
        }
    }
}