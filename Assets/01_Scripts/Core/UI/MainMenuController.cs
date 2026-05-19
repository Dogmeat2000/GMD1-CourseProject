using System.Collections;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Settings;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _01_Scripts.Core.UI
{
    ///<summary>
    /// <p>Handles loading new game levels.</p>
    ///</summary>
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Readouts")]
        [Tooltip("The parent object of the loading screen")]
        [SerializeField] private GameObject loadingScreenPanel;
    
        [Tooltip("Text element to display the loading percentage")]
        [SerializeField] private TextMeshProUGUI progressText;

        [Tooltip("Optional: A visual slider for loading progress")]
        [SerializeField] private Slider progressBar;

        [Header("Player Readiness")]
        [Tooltip("The parent panel containing the Ready text")]
        [SerializeField] private GameObject readyUpPanel;
        
        [Header("Player 1 UI")]
        [SerializeField] private GameObject p1Container;
        [SerializeField] private TextMeshProUGUI p1StatusText;
        
        [Header("Player 2 UI")]
        [SerializeField] private GameObject p2Container;
        [SerializeField] private TextMeshProUGUI p2StatusText;
        
        [Tooltip("Binding to Player1's Start Button")]
        [SerializeField] private InputActionReference p1ReadyAction;
        
        [Tooltip("Binding to Player1's Start Button")]
        [SerializeField] private InputActionReference p2ReadyAction;
        
        private bool _isP1Ready = false;
        private bool _isP2Ready = false;
        private bool _requiresTwoPlayers = false;
        private bool _isAwaitingPlayersReady = false;
        
        private void OnEnable() {
            if (p1ReadyAction) {
                p1ReadyAction.action.Enable();
                p1ReadyAction.action.performed += HandleP1Ready;
            }
            
            if (p2ReadyAction) {
                p2ReadyAction.action.Enable();
                p2ReadyAction.action.performed += HandleP2Ready;
            }
        }
        
        private void OnDisable() {
            if (p1ReadyAction)
                p1ReadyAction.action.performed -= HandleP1Ready;
            
            if (p2ReadyAction)
                p2ReadyAction.action.performed -= HandleP2Ready;
        }
        
        private void Start() {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (loadingScreenPanel)
                loadingScreenPanel.SetActive(false);
            
            if (readyUpPanel) 
                readyUpPanel.SetActive(false);
        }

        public void LoadGameLevel(string sceneName) {
            Debug.Log($"Loading Game Level: {sceneName}");
            StartCoroutine(LoadGameLevelAsync(sceneName));
        }

        private IEnumerator LoadGameLevelAsync(string sceneName) {
            if (loadingScreenPanel) 
                loadingScreenPanel.SetActive(true);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;
            
            while (operation.progress < 0.9f) {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                
                if (progressBar) 
                    progressBar.value = progress;
                
                if (progressText) 
                    progressText.SetText("DEPLOYING: {0}%", Mathf.RoundToInt(progress * 100));
                
                yield return null;
            }
            
            if (progressBar) 
                progressBar.gameObject.SetActive(false);
            
            if (progressText) 
                progressText.SetText("PRESS START TO BEGIN...");

            RequestPlayerReadiness();
            _isAwaitingPlayersReady = true;
            yield return new WaitUntil(IsSquadReady);
            
            _isAwaitingPlayersReady = false;
            if (progressText) 
                progressText.SetText("LAUNCHING...");
            
            operation.allowSceneActivation = true;
        }
        
        private void RequestPlayerReadiness() {
            if (readyUpPanel) 
                readyUpPanel.SetActive(true);

            GameMode currentMode = GameMode.SinglePlayer;
            if (GlobalManager.Instance && GlobalManager.Instance.GlobalSettings) {
                currentMode = GlobalManager.Instance.GlobalSettings.ActiveGameMode;
            }

            _requiresTwoPlayers = (currentMode == GameMode.CoopOneShip || currentMode == GameMode.CoopTwoShips);

            if (p1Container) 
                p1Container.SetActive(true);
            
            _isP1Ready = false;
            UpdatePlayerStatusUI(1, false);

            if (p2Container) {
                p2Container.SetActive(_requiresTwoPlayers);
                _isP2Ready = false;
                if (_requiresTwoPlayers) UpdatePlayerStatusUI(2, false);
            }
        }
        
        private void HandleP1Ready(InputAction.CallbackContext context) {
            if (!_isAwaitingPlayersReady) 
                return;
            
            _isP1Ready = !_isP1Ready;
            UpdatePlayerStatusUI(1, _isP1Ready);
        }

        private void HandleP2Ready(InputAction.CallbackContext context) {
            if (!_isAwaitingPlayersReady || !_requiresTwoPlayers) 
                return; 
            
            _isP2Ready = !_isP2Ready;
            UpdatePlayerStatusUI(2, _isP2Ready);
        }

        private void UpdatePlayerStatusUI(int playerNum, bool isReady) {
            string status = isReady ? "<color=green>READY</color>" : "<color=red>NOT READY</color>";
            
            if (playerNum == 1 && p1StatusText) {
                p1StatusText.text = $"PLAYER 1: {status}";
            } else if (playerNum == 2 && p2StatusText) {
                p2StatusText.text = $"PLAYER 2: {status}";
            }
        }

        private bool IsSquadReady() {
            if (_requiresTwoPlayers) 
                return _isP1Ready && _isP2Ready;
            
            return _isP1Ready;
        }

        /// <summary>
        /// Terminates the application, shutting down the game.
        /// </summary>
        public void QuitGame() {
            Debug.Log("Terminating the application.");
            Application.Quit();
        }
    }
}