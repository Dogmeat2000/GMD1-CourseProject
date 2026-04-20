using System.Collections.Generic;
using UnityEngine;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Targeting.Strategies;

namespace _01_Scripts.Core.Targeting
{
    public class BattlefieldRadar : MonoBehaviour
    {
        private readonly Dictionary<Faction, List<ITargetable>> _radarBlips = new Dictionary<Faction, List<ITargetable>> {
            { Faction.Friendly, new List<ITargetable>() },
            { Faction.Hostile, new List<ITargetable>() },
            { Faction.Neutral, new List<ITargetable>() }
        };

        public void RegisterTarget(ITargetable target) {
            if (!_radarBlips[target.Faction].Contains(target)) {
                _radarBlips[target.Faction].Add(target);
            }
        }

        public void UnregisterTarget(ITargetable target) {
            if (_radarBlips.TryGetValue(target.Faction, out var blip)) {
                blip.Remove(target);
            }
        }

        /** <summary>
         * Retrieves the optimal target of the requested faction using the provided tactical strategy.
         * </summary>
         */
        public ITargetable GetOptimalTarget(Vector3 requesterPosition, Faction targetFaction, ITargetingStrategy strategy) {
            if (!_radarBlips.ContainsKey(targetFaction) || _radarBlips[targetFaction].Count == 0) 
                return null;

            return strategy.SelectTarget(_radarBlips[targetFaction], requesterPosition);
        }
        
        public int GetActiveHostileCount() {
            if (!_radarBlips.TryGetValue(Faction.Hostile, out var blip)) 
                return 0;

            blip.RemoveAll(target => target == null || !target.IsTargetable);
            
            return _radarBlips[Faction.Hostile].Count;
        }
    }
}