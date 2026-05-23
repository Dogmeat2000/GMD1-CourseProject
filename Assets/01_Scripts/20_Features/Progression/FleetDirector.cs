using System;
using System.Collections.Generic;
using _01_Scripts._20_Features.Vitals;
using UnityEngine;

namespace _01_Scripts._20_Features.Progression
{
    public class FleetDirector : MonoBehaviour, IFleetService
    { 
        [Header("Fleet Roster")]
        [Tooltip("Drag and drop the HealthManagers of all pre-placed allied ships into this list.")]
        [SerializeField] private List<HealthManager> alliedFleet = new();
       
        public event Action OnFleetDestroyed;

        private int _totalStartingShips;
        private int _currentAliveShips;

        private void Awake() {
            _totalStartingShips = alliedFleet.Count;
            _currentAliveShips = _totalStartingShips;
        }
        
        private void Start() {
            foreach (var ship in alliedFleet) {
                if (!ship) 
                    continue;
                
                ship.OnZeroHealth += HandleShipDestroyed;
            }
        }

        private void OnDestroy() {
            foreach (HealthManager ship in alliedFleet) {
                if (!ship) 
                    continue;
                
                ship.OnZeroHealth -= HandleShipDestroyed;
            }
        }

        private void HandleShipDestroyed(HealthManager ship, GameObject killer) {
            ship.OnZeroHealth -= HandleShipDestroyed;
            
            alliedFleet.Remove(ship);
            _currentAliveShips--;
            
            if (_currentAliveShips <= 0) {
                OnFleetDestroyed?.Invoke();
                Debug.LogWarning("GameOver: All allied ships lost. Defeat Triggered.");
            }
        }
    }
}
