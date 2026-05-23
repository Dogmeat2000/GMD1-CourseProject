using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts._20_Features.Targeting
{
    /// <summary>
    /// Tracks all active entities on the map, allowing other scripts access to updates information about Friendly, Enemy and Neutral game entities.
    /// </summary>
    public class BattlefieldRadar : MonoBehaviour, IActorTracker
    {
        private readonly Dictionary<Faction, List<ITargetable>> _radarBlips = new Dictionary<Faction, List<ITargetable>> {
            { Faction.Friendly, new List<ITargetable>(24) },
            { Faction.Hostile, new List<ITargetable>(232) },
            { Faction.Neutral, new List<ITargetable>(0) }
        };

        public List<ITargetable> GetRadarTargets(Faction faction) {
            return _radarBlips.TryGetValue(faction, out var blips) ? blips : new List<ITargetable>();
        }

        public void RegisterTarget(ITargetable target) {
            if (!_radarBlips[target.Faction].Contains(target))
                _radarBlips[target.Faction].Add(target);
        }

        public void UnregisterTarget(ITargetable target) {
            if (_radarBlips.TryGetValue(target.Faction, out var blip))
                blip.Remove(target);
        }

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