using _01_Scripts.Core.Targeting;
using UnityEngine;

namespace _01_Scripts._20_Features.Targeting
{
    public interface ITargetable
    {
        /// <summary>
        /// Property that holds the Transform (GameObject) this script is attached to.
        /// </summary>
        Transform TargetTransform { get; }
        
        TargetPriority Priority { get; }
        
        Faction Faction { get; }
        
        /// <summary>
        /// Property that indicates whether the entity this script is attached to, is alive and active.
        /// </summary>
        bool IsTargetable { get; }
    }
}