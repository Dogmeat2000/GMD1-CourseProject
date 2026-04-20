using _01_Scripts.Core.Services;
using _01_Scripts.Core.Targeting;
using UnityEngine;

namespace _01_Scripts.Core.Managers
{
    /** <summary>
    * Initializes the Service Locator for the current level and registers all active directors/managers.
    * </summary>
    */
    [DefaultExecutionOrder(-100)] 
    public class LevelBootstrapper : MonoBehaviour
    {
        [Header("Level Directors/Managers")]
        [SerializeField] 
        private LevelManager levelManager;
        
        [SerializeField] 
        private GameDirector gameDirector;
        
        [SerializeField] 
        private WaveDirector waveDirector;
        
        [SerializeField] 
        private FleetDirector fleetDirector;
        
        [SerializeField] 
        private BattlefieldRadar battlefieldRadar;

        private void Awake() {
            ServiceLocator.Clear(); 
            
            if (levelManager) ServiceLocator.Register(levelManager);
            if (gameDirector) ServiceLocator.Register(gameDirector);
            if (waveDirector) ServiceLocator.Register(waveDirector);
            if (fleetDirector) ServiceLocator.Register(fleetDirector);
            if (battlefieldRadar) ServiceLocator.Register(battlefieldRadar);
            
            Debug.Log("Level Bootstrapper: All directors/managers online and registered.");
        }

        private void OnDestroy() {
            ServiceLocator.Clear();
        }
    }
}