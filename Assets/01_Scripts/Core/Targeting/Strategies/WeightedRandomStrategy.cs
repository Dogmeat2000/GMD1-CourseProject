using System.Collections.Generic;
using UnityEngine;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Settings;

namespace _01_Scripts.Core.Targeting.Strategies
{
    // TODO Add description
    public class WeightedRandomStrategy : ITargetingStrategy
    {
        public ITargetable SelectTarget(List<ITargetable> availableTargets, Vector3 requesterPosition) {
            int totalWeight = 0;
            List<ITargetable> validTargets = new List<ITargetable>();

            LevelManager levelManager = ServiceLocator.Get<LevelManager>();
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