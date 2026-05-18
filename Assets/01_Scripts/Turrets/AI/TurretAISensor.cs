using System.Collections.Generic;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Targeting;
using UnityEngine;

namespace _01_Scripts.Turrets.AI
{
    /// <summary>
    /// Scans the BattlefieldRadar at set intervals and locks onto the closest valid threat.
    /// </summary>
    public class TurretAISensor : MonoBehaviour
    {
        [Header("Capabilities")]
        [Tooltip("Maximum range the AI can acquire targets.")]
        [SerializeField] private float maxTargetingDistance = 500f; // TODO: Change to settings option instead
        
        [Tooltip("How often [s] the sensor pings the radar. Higher = better performance.")]
        [SerializeField] private float scanInterval = 0.1f; // TODO: Change to settings option instead

        public ITargetable CurrentTarget { get; private set; }

        private BattlefieldRadar _radar;
        private float _nextScanTime;

        private void Start() {
            _radar = ServiceLocator.Get<BattlefieldRadar>();
        }

        private void Update() {
            if (Time.time >= _nextScanTime) {
                ScanForTargets();
                _nextScanTime = Time.time + scanInterval;
            }
        }

        private void ScanForTargets() {
            if (!_radar) {
                CurrentTarget = null;
                return;
            }
            
            float closestDistanceSqr = maxTargetingDistance * maxTargetingDistance;
            ITargetable bestTarget = null;
            Vector3 currentPosition = transform.position;
            List<ITargetable> targetableEnemies = _radar.GetRadarTargets(Faction.Hostile);

            if (targetableEnemies.Count <= 0) {
                CurrentTarget = null;
                return;
            }
            
            foreach (ITargetable enemy in targetableEnemies) {
                if (enemy == null || !enemy.IsTargetable) 
                    continue;

                float distanceSqr = (enemy.TargetTransform.position - currentPosition).sqrMagnitude;
                if (distanceSqr < closestDistanceSqr) {
                    closestDistanceSqr = distanceSqr;
                    bestTarget = enemy;
                }
            }

            CurrentTarget = bestTarget;
        }
    }
}
