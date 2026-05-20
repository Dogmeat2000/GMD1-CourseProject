using _01_Scripts.Core.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        [SerializeField] 
        private GameObject pauseMenuPanel;
        
        [Header("Scene Navigation")]
        [Tooltip("The exact name of the Main Menu scene to load upon exit")]
        [SerializeField] 
        private string mainMenuSceneName = "SCN_MainMenu";
        
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
        
        // TODO Add description
        public void ResumeGame() => _gameState.ResumeGame();
        
        // TODO Add description
        public void ReturnToMainMenu() {
            _gameState.ResumeGame();
            SceneManager.LoadSceneAsync(mainMenuSceneName);
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