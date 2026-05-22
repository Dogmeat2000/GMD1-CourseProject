using System.Collections;
using _01_Scripts._10_Core;
using _01_Scripts._10_Core.Persistence;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _01_Scripts._40_UI.Menus
{
    ///<summary>
    /// Coordinates asynchronous scene loading, UI triggers, and Input state.
    ///</summary>
    [RequireComponent(typeof(MainMenuUIPresenter), typeof(MainMenuInput))]
    public class MainMenuController : MonoBehaviour
    {
        private MainMenuUIPresenter _ui;
        private MainMenuInput _input;

        private void Awake() {
            _ui = GetComponent<MainMenuUIPresenter>();
            _input = GetComponent<MainMenuInput>();
        }

        private void OnEnable() {
            _input.OnPlayerReadyStatusChanged += _ui.UpdatePlayerStatus;
        }

        private void OnDisable() {
            _input.OnPlayerReadyStatusChanged -= _ui.UpdatePlayerStatus;
        }

        private void Start() {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            _ui.Initialize();
        }

        public void LoadGameLevel(string sceneName) {
            Debug.Log($"Loading Game Level: {sceneName}");
            StartCoroutine(LoadGameLevelAsync(sceneName));
        }

        private IEnumerator LoadGameLevelAsync(string sceneName) {
            _ui.ShowLoadingScreen(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f) {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                _ui.UpdateProgress(progress, Mathf.RoundToInt(progress * 100));
                yield return null;
            }

            ConfigureGameModeReadiness();
            _ui.ShowReadyUpPhase(_input.RequiresTwoPlayers);

            _input.ResetInput();
            _input.IsAwaitingPlayers = true;

            yield return new WaitUntil(IsAllPlayersReady);

            _input.IsAwaitingPlayers = false;
            _ui.ShowLaunching();

            operation.allowSceneActivation = true;
        }

        private void ConfigureGameModeReadiness() {
            GameMode currentMode = GameMode.SinglePlayer;
            
            if (GlobalManager.Instance && GlobalManager.Instance.GlobalSettings)
                currentMode = GlobalManager.Instance.GlobalSettings.ActiveGameMode;

            _input.RequiresTwoPlayers = (currentMode == GameMode.CoopOneShip || currentMode == GameMode.CoopTwoShips);
        }

        private bool IsAllPlayersReady() {
            if (_input.RequiresTwoPlayers) 
                return _input.IsP1Ready && _input.IsP2Ready;

            return _input.IsP1Ready;
        }

        public void QuitGame() {
            Debug.Log("Terminating the application.");
            Application.Quit();
        }
    }
}
