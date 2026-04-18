using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _01_Scripts.Core.UI
{
    /**
     * <p>Handles loading new game levels.</p>
     */
    public class MainMenuController : MonoBehaviour
    {
        [Header("UI Readouts")]
        [Tooltip("The parent object of the loading screen")]
        [SerializeField] private GameObject loadingScreenPanel;
    
        [Tooltip("Text element to display the loading percentage")]
        [SerializeField] private TextMeshProUGUI progressText;

        [Tooltip("Optional: A visual slider for loading progress")]
        [SerializeField] private Slider progressBar;

        private void Start() {
            if (loadingScreenPanel) {
                loadingScreenPanel.SetActive(false);
            }
        }

        public void LoadGameLevel(string sceneName) {
            Debug.Log($"Loading Game Level: {sceneName}");
            StartCoroutine(LoadGameLevelAsync(sceneName));
        }

        private IEnumerator LoadGameLevelAsync(string sceneName) {
            if (loadingScreenPanel) loadingScreenPanel.SetActive(true);
            
            // Ensure the cursor is ready for gameplay during the transition
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

            // Prevent the scene from activating instantly if you want to hold players on the loading screen
            // TODO: REVISIT THIS PART LATER! COULD BE GOOD WITH MULTIPLAYER SUPPORT!
            // operation.allowSceneActivation = false; // (Uncomment later if you want a "Press A to Start" prompt)
            
            while (operation is { isDone: false }) {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                
                if (progressBar) progressBar.value = progress;
                if (progressText) progressText.text = $"DEPLOYING: {Mathf.RoundToInt(progress * 100)}%";
                yield return null;
            }
        }

        public void AbortMission() {
            Debug.Log("Mission Aborted. Terminating application.");
            Application.Quit();
        }
    }
}