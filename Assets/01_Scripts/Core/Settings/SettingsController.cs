using _01_Scripts.Core.Managers;
using UnityEngine;

namespace _01_Scripts.Core.Settings
{
    // TODO Add description
    public class SettingsController : MonoBehaviour
    {
        /// <summary>
        /// Changes the active game mode. Expected indices: 0 = SinglePlayer, 1 = CoopTwoShips, 2 = CoopOneShip.
        /// </summary>
        public void SetGameMode(int modeIndex) {
            if (!GlobalManager.Instance || !GlobalManager.Instance.GlobalSettings) 
                return;
            
            GlobalManager.Instance.GlobalSettings.ActiveGameMode = (GameMode) modeIndex;
            Debug.Log($"Game Mode set to: {GlobalManager.Instance.GlobalSettings.ActiveGameMode}");
        }
    }
}
