using _01_Scripts.Core.Targeting;
using UnityEngine;

namespace _01_Scripts.Core.Interfaces
{
    public interface ITargetable
    {
        Transform TargetTransform { get; }
        TargetPriority Priority { get; }
        Faction Faction { get; }
        bool IsTargetable { get; }
    }
}