using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using UnityEngine.SceneManagement;

namespace _01_Scripts.Core.Utilities
{
    /// <summary>
    /// Utility class for helper methods relating to navigating between scenes.
    /// </summary>
    public class SceneNavigationUtilities
    {
        public static void LoadMainMenu() {
            ServiceLocator.Get<GameStateService>()?.ResumeGame();
            SceneManager.LoadSceneAsync(GlobalManager.Instance.GlobalSettings.MainMenuSceneName);
        }
    }
}
