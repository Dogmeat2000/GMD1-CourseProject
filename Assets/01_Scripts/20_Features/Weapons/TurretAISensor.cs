using System.Collections.Generic;
using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._10_Core.Persistence;
using _01_Scripts._20_Features.Progression;
using _01_Scripts._20_Features.Targeting;
using UnityEngine;

namespace _01_Scripts._20_Features.Weapons
{
    /// <summary>
    /// Scans the BattlefieldRadar at set intervals and locks onto the closest valid threat.
    /// </summary>
    public class TurretAISensor : MonoBehaviour
    {
        /// <summary>
        /// Property containing the Current Target that this Turret is locked on to.
        /// </summary>
        public ITargetable CurrentTarget { get; private set; }
        
        private BattlefieldRadar _radar;
        private float _nextScanTime;
        private LevelSettings _settings;

        private void Start() {
            _radar = ServiceLocator.Get<BattlefieldRadar>();
            _settings = ServiceLocator.Get<LevelManager>().Settings;
        }

        private void Update() {
            if (Time.time >= _nextScanTime) {
                ScanForTargets();
                _nextScanTime = Time.time + _settings.AiScanInterval;
            }
        }

        private void ScanForTargets() {
            if (!_radar) {
                CurrentTarget = null;
                return;
            }
            
            float closestDistanceSqr = _settings.MaxAiTargetingDistance * _settings.MaxAiTargetingDistance;
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
