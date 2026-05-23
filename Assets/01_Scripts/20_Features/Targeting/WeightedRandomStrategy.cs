using System.Collections.Generic;
using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._10_Core.Persistence;
using _01_Scripts._20_Features.Progression;
using UnityEngine;

namespace _01_Scripts._20_Features.Targeting
{
    /// <summary>
    /// A concrete TargetingStrategy that prioritizes which target an entity (allied turret or hostile entity)
    /// should engage based primarily the priority/importance of available targets. Will prioritize selecting the more important targets,
    /// while still sometimes selecting less important targets - to provide a random element to this strategy.
    /// </summary>
    public class WeightedRandomStrategy : ITargetingStrategy
    {
        public ITargetable SelectTarget(List<ITargetable> availableTargets, Vector3 requesterPosition) {
            int totalWeight = 0;
            List<ITargetable> validTargets = new List<ITargetable>();

            ILevelManager levelManager = ServiceLocator.Get<ILevelManager>();
            LevelSettings settings = levelManager.Settings;
            
            foreach (ITargetable target in availableTargets) {
                if (target != null && target.IsTargetable) {
                    totalWeight += settings.GetPriorityWeight(target.Priority);
                    validTargets.Add(target);
                }
            }

            if (validTargets.Count == 0) 
                return null;

            int randomDraw = Random.Range(0, totalWeight);
            int currentWeight = 0;

            foreach (ITargetable target in validTargets) {
                currentWeight += settings.GetPriorityWeight(target.Priority);
                if (currentWeight > randomDraw)
                    return target;
            }

            return validTargets[0]; 
        }
    }
}