using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Services;
using UnityEngine;

namespace _01_Scripts.Core.Managers
{
    /** <summary>
    * Initializes the Service Locator for the current level and registers all active directors/managers.
    * </summary>
    */
    [DefaultExecutionOrder(-100)] 
    public class ServiceHub : MonoBehaviour
    {
        private void Awake() {
            ServiceLocator.Initialize();
            IService[] allServices = GetComponentsInChildren<IService>();
            
            foreach (IService service in allServices) {
                ServiceLocator.Register(service.GetType(), service);
            }
            
            GameStateService stateService = new GameStateService();
            ServiceLocator.Register(typeof(GameStateService), stateService);
            
            Debug.Log("All directors/managers online and registered.");
        }

        private void OnDestroy() {
            ServiceLocator.Clear();
        }
    }
}