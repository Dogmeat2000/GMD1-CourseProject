using System;
using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Core.Managers
{
    public class FleetDirector : MonoBehaviour
    { 
        public static FleetDirector Instance { get; private set; }

        [Header("Fleet Roster")]
        [Tooltip("Drag and drop the HealthManagers of all pre-placed allied ships into this list.")]
        [SerializeField] private List<HealthManager> alliedFleet = new();
        
        public event Action<float> OnFleetHealthAverageChanged; // Broadcasts: 0.0f to 1.0f (Percentage)
        public event Action<int, int> OnFleetCountChanged;      // Broadcasts: Current Alive, Total Starting
        public event Action OnFleetDestroyed;                   // TODO Triggers the Defeat Sequence

        private int _totalStartingShips;
        private int _currentAliveShips;

        private void Awake() {
            if (Instance && Instance != this) {
                Destroy(gameObject);
            } else {
                Instance = this;
            }
        }

        private void Start() {
            _totalStartingShips = alliedFleet.Count;
            _currentAliveShips = _totalStartingShips;
            
            foreach (var ship in alliedFleet) {
                if (!ship) 
                    continue;
                
                ship.OnHealthChanged += HandleShipHealthChanged;
                ship.OnZeroHealth += HandleShipDestroyed;
            }
            BroadcastFleetStatus();
        }

        private void OnDestroy() {
            foreach (var ship in alliedFleet) {
                if (!ship) 
                    continue;
                
                ship.OnHealthChanged -= HandleShipHealthChanged;
                ship.OnZeroHealth -= HandleShipDestroyed;
            }
        }

        private void HandleShipHealthChanged(int current, int max, GameObject instigator) {
            BroadcastFleetStatus();
        }

        private void HandleShipDestroyed(HealthManager ship, GameObject killer) {
            ship.OnHealthChanged -= HandleShipHealthChanged;
            ship.OnZeroHealth -= HandleShipDestroyed;
            
            alliedFleet.Remove(ship);
            _currentAliveShips--;

            BroadcastFleetStatus();
            
            if (_currentAliveShips <= 0) {
                OnFleetDestroyed?.Invoke();
                Debug.LogWarning("CRITICAL FAILURE: All allied ships lost. Triggering Defeat.");
                // TODO: Wire this into Game Over UI
            }
        }
        
        /** <summary>
         * Allows late-joining UI elements to request an immediate update
         * without waiting for a ship to take damage.
         * </summary>
         */
        public void RequestFleetStatusUpdate() {
            BroadcastFleetStatus();
        }

        private void BroadcastFleetStatus() {
            if (_totalStartingShips == 0) return;

            OnFleetCountChanged?.Invoke(_currentAliveShips, _totalStartingShips);

            if (_currentAliveShips == 0) {
                OnFleetHealthAverageChanged?.Invoke(0f);
                return;
            }

            float totalCurrentHealth = 0;
            float totalMaxHealth = 0;

            foreach (var ship in alliedFleet) {
                totalCurrentHealth += ship.CurrentHealth;
                totalMaxHealth += ship.MaxHealth;
            }
            
            float averageHealth = totalCurrentHealth / totalMaxHealth;
            OnFleetHealthAverageChanged?.Invoke(averageHealth);
        }
    }
}
