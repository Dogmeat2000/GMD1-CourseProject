using System.Collections.Generic;
using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts.Core.Targeting;
using UnityEngine;

namespace _01_Scripts._20_Features.Targeting
{
    /// <summary>
    /// Manages all active entities on the map, allowing other scripts access to updates information about Friendly, Enemy and Neutral game entities.
    /// </summary>
    public class BattlefieldRadar : MonoBehaviour, IService
    {
        private readonly Dictionary<Faction, List<ITargetable>> _radarBlips = new Dictionary<Faction, List<ITargetable>> {
            { Faction.Friendly, new List<ITargetable>(24) },
            { Faction.Hostile, new List<ITargetable>(232) },
            { Faction.Neutral, new List<ITargetable>(0) }
        };

        /// <summary>
        /// Returns all active targets that belong to the specified faction.
        /// </summary>
        public List<ITargetable> GetRadarTargets(Faction faction) {
            return _radarBlips.TryGetValue(faction, out var blips) ? blips : new List<ITargetable>();
        }

        /// <summary>
        /// Used by Entities to register with this script, to make their presence known on the map.
        /// </summary>
        /// <param name="target"></param>
        public void RegisterTarget(ITargetable target) {
            if (!_radarBlips[target.Faction].Contains(target))
                _radarBlips[target.Faction].Add(target);
        }

        /// <summary>
        /// Unregisters an entity from this script.
        /// </summary>
        /// <param name="target"></param>
        public void UnregisterTarget(ITargetable target) {
            if (_radarBlips.TryGetValue(target.Faction, out var blip))
                blip.Remove(target);
        }

        /// <summary>
        /// Retrieves the optimal target of the requested faction using the provided tactical strategy.
        /// </summary>
        public ITargetable GetOptimalTarget(Vector3 requesterPosition, Faction targetFaction, ITargetingStrategy strategy) {
            if (!_radarBlips.ContainsKey(targetFaction) || _radarBlips[targetFaction].Count == 0) 
                return null;

            return strategy.SelectTarget(_radarBlips[targetFaction], requesterPosition);
        }
        
        /// <summary>
        /// Returns the number of entities belonging til the Hostile faction, that are still alive on the map.
        /// </summary>
        /// <returns></returns>
        public int GetActiveHostileCount() {
            if (!_radarBlips.TryGetValue(Faction.Hostile, out var blip)) 
                return 0;

            blip.RemoveAll(target => target == null || !target.IsTargetable);
            
            return _radarBlips[Faction.Hostile].Count;
        }
    }
}