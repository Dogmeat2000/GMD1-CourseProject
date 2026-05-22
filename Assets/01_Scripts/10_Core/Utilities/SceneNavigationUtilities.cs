using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._20_Features.Progression;
using UnityEngine.SceneManagement;

namespace _01_Scripts._10_Core.Utilities
{
    /// <summary>
    /// Utility class for helper methods relating to navigating between scenes.
    /// </summary>
    public static class SceneNavigationUtilities
    {
        public static void LoadMainMenu() {
            ServiceLocator.Get<IGameStateProvider>()?.ResumeGame();
            SceneManager.LoadSceneAsync(GlobalManager.Instance.GlobalSettings.MainMenuSceneName);
        }
    }
}
