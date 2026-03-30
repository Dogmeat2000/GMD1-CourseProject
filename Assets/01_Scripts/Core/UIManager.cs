using UnityEngine;

namespace _01_Scripts.Core
{
    /**
     * Handles swapping static screens in menu scene. Does NOT handle any level loading.
     */
    public class UIManager : MonoBehaviour
    {
        [Header("Menu Screens")]
        public GameObject mainMenuPanel;
        
        private GameObject _activePanel;

        private void Start()
        {
            // Ensure all panels are hidden initially to prevent overlap
            mainMenuPanel.SetActive(false);

            // Deploy the primary menu screen
            OpenPanel(mainMenuPanel);
        }

        public void OpenPanel(GameObject panelToOpen)
        {
            if (_activePanel)
            {
                _activePanel.SetActive(false);
            }
            
            _activePanel = panelToOpen;
            _activePanel.SetActive(true);
        }

        // --- UI Button Execution Commands ---
        // Link UI Buttons to these specific methods in the Inspector
        public void ShowMainMenu() => OpenPanel(mainMenuPanel);
    }
}