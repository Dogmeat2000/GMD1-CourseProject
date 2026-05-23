using System;
using _01_Scripts._10_Core.DependencyInjection;

namespace _01_Scripts._20_Features.Progression
{
    public interface IFleetService : IService
    {
        /// <summary>
        /// Event that is fired when the fleet is fully destroyed.
        /// </summary>
        public event Action OnFleetDestroyed;
    }
}
