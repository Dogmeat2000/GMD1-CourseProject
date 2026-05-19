using System.Collections.Generic;
using UnityEngine;

namespace _01_Scripts.Core.Interfaces
{
    public interface ITargetingStrategy
    {
        /// <summary>
        /// Evaluates a list of targets and returns the optimal choice based on the implemented algorithm.
        /// </summary>
        ITargetable SelectTarget(List<ITargetable> availableTargets, Vector3 requesterPosition);
    }
}