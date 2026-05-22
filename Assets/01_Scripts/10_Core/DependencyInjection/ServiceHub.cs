using _01_Scripts._20_Features.Progression;
using UnityEngine;

namespace _01_Scripts._10_Core.DependencyInjection
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
            
            IGameStateProvider stateService = new GameStateService();
            ServiceLocator.Register(typeof(IGameStateProvider), stateService);
            
            Debug.Log("All directors/managers online and registered.");
        }

        private void OnDestroy() {
            ServiceLocator.Clear();
        }
    }
}