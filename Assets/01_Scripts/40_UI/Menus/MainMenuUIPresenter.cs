using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts._40_UI.Menus
{
    /// <summary>
    /// Handles the visual activation and text updates for the deployment screen.
    /// </summary>
    public class MainMenuUIPresenter : MonoBehaviour
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
        
        public void Initialize() {
            if (loadingScreenPanel) 
                loadingScreenPanel.SetActive(false);
            
            if (readyUpPanel) 
                readyUpPanel.SetActive(false);
        }

        public void ShowLoadingScreen(bool show) => loadingScreenPanel.SetActive(show);

        public void UpdateProgress(float progress, int progressPercent) {
            if (progressBar) 
                progressBar.value = progress;
            
            if (progressText) 
                progressText.SetText("DEPLOYING: {0}%", progressPercent);
        }

        public void ShowReadyUpPhase(bool requiresTwoPlayers) {
            if (progressBar) 
                progressBar.gameObject.SetActive(false);
            
            if (progressText) 
                progressText.SetText("PRESS START TO BEGIN...");
            
            if (readyUpPanel) 
                readyUpPanel.SetActive(true);

            if (p1Container) 
                p1Container.SetActive(true);
            
            UpdatePlayerStatus(1, false);

            if (p2Container) {
                p2Container.SetActive(requiresTwoPlayers);
                
                if (requiresTwoPlayers) 
                    UpdatePlayerStatus(2, false);
            }
        }

        public void UpdatePlayerStatus(int playerNum, bool isReady) {
            string status = isReady ? "<color=green>READY</color>" : "<color=red>NOT READY</color>";
            
            if (playerNum == 1 && p1StatusText) 
                p1StatusText.text = $"PLAYER 1: {status}";
            
            else if (playerNum == 2 && p2StatusText) 
                p2StatusText.text = $"PLAYER 2: {status}";
        }

        public void ShowLaunching() {
            if (progressText) 
                progressText.SetText("LAUNCHING...");
        }
    }
}