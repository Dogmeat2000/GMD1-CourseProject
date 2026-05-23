using System;
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
                Type concreteType = service.GetType();
                
                Type[] interfaces = concreteType.GetInterfaces();
                bool registeredAsInterface = false;
                
                foreach (Type iface in interfaces) {
                    if (typeof(IService).IsAssignableFrom(iface) && iface != typeof(IService)) {
                        ServiceLocator.Register(iface, service);
                        Debug.Log($"Registered Interface: {iface.Name} mapped to Implementation: {concreteType.Name}");
                        registeredAsInterface = true;
                    }
                }
                
                if (!registeredAsInterface) {
                    ServiceLocator.Register(concreteType, service);
                    Debug.LogWarning($"Service {concreteType.Name} registered by concrete type! Missing domain interface.");
                }
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