using System.Collections.Generic;
using _01_Scripts._10_Core.DependencyInjection;
using UnityEngine;

namespace _01_Scripts._20_Features.Targeting
{
    /// <summary>
    /// Tracks all active entities on the map, allowing other scripts access to updates information about Friendly, Enemy and Neutral game entities.
    /// </summary>
    public interface IActorTracker : IService
    {
        /// <summary>
        /// Returns all active targets that belong to the specified faction.
        /// </summary>
        public List<ITargetable> GetRadarTargets(Faction faction);

        /// <summary>
        /// Used by Entities to register with this script, to make their presence known on the map.
        /// </summary>
        /// <param name="target"></param>
        public void RegisterTarget(ITargetable target);

        /// <summary>
        /// Unregisters an entity from this script.
        /// </summary>
        /// <param name="target"></param>
        public void UnregisterTarget(ITargetable target);

        /// <summary>
        /// Retrieves the optimal target of the requested faction using the provided tactical strategy.
        /// </summary>
        public ITargetable GetOptimalTarget(Vector3 requesterPosition, Faction targetFaction, ITargetingStrategy strategy);

        /// <summary>
        /// Returns the number of entities belonging til the Hostile faction, that are still alive on the map.
        /// </summary>
        /// <returns></returns>
        public int GetActiveHostileCount();
    }
}
