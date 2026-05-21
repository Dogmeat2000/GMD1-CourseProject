using _01_Scripts.Core.Services;
using _01_Scripts.Core.Utilities;
using UnityEngine;
using static _01_Scripts.Core.Utilities.CursorUtilities;

namespace _01_Scripts.Core.UI
{
    /// <summary>
    /// Handles the Pause UI overlay by listening to the GameStateService.
    /// </summary>
    public class PauseController : MonoBehaviour
    {
        [Header("UI Overlay")]
        [Tooltip("The Panel to be displayed when Pause is invoked")]
        [SerializeField] private GameObject pauseMenuPanel;
        
        private GameStateService _gameState;

        private void Awake() {
            _gameState = ServiceLocator.Get<GameStateService>();
        }
        
        private void OnEnable() {
            if (_gameState != null) 
                _gameState.OnStateChanged += HandleStateChanged;
        }
        
        private void OnDisable() {
            if (_gameState != null) 
                _gameState.OnStateChanged -= HandleStateChanged;
        }
        
        private void Start() {
            if (UnityEngine.EventSystems.EventSystem.current) 
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
            
            if (pauseMenuPanel) 
                pauseMenuPanel.SetActive(false);
            
            LockAndHideCursor();
        }
        
        /// <summary>
        /// Resumes the Game.
        /// </summary>
        public void ResumeGame() => _gameState.ResumeGame();
        
        /// <summary>
        /// Load the Main Menu scene.
        /// </summary>
        public void ReturnToMainMenu() {
            SceneNavigationUtilities.LoadMainMenu();
        }
        
        private void HandleStateChanged(GameState newState) {
            switch (newState) {
                case GameState.Paused:
                    pauseMenuPanel.SetActive(true);
                    UnlockAndShowCursor();
                    break;
                
                case GameState.Playing:
                    pauseMenuPanel.SetActive(false);
                    LockAndHideCursor();
                    break;
            }
        }
    }
}