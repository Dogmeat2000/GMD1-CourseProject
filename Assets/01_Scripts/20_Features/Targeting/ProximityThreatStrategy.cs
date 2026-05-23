using System.Collections.Generic;
using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._10_Core.Persistence;
using _01_Scripts._20_Features.Progression;
using UnityEngine;

namespace _01_Scripts._20_Features.Targeting
{
    /// <summary>
    /// A concrete TargetingStrategy that prioritizes which target an entity (allied turret or hostile entity)
    /// should engage based on a mix of the importance (priority) of the targets on the field as well as their distance.
    /// Will generally prioritize targets in closer proximity.
    /// </summary>
    public class ProximityThreatStrategy : ITargetingStrategy
    {
        public ITargetable SelectTarget(List<ITargetable> availableTargets, Vector3 requesterPosition) {
            ITargetable mostDangerousTarget = null;
            float highestThreatScore = -1f;

            ILevelManager levelManager = ServiceLocator.Get<ILevelManager>();
            LevelSettings settings = levelManager.Settings;

            foreach (ITargetable target in availableTargets) {
                if (target == null || !target.IsTargetable) 
                    continue;
                
                float sqrDistance = (target.TargetTransform.position - requesterPosition).sqrMagnitude;
                
                if (sqrDistance <= 0.1f) 
                    sqrDistance = 0.1f; 
                
                int weight = settings.GetPriorityWeight(target.Priority);
                float threatScore = weight / sqrDistance;

                if (threatScore > highestThreatScore) {
                    highestThreatScore = threatScore;
                    mostDangerousTarget = target;
                }
            }

            return mostDangerousTarget;
        }
    }
}