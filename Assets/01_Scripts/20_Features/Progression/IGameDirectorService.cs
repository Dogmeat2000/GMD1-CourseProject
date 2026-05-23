using System;
using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._20_Features.Vitals;

namespace _01_Scripts._20_Features.Progression
{
    /// <summary>
    /// The primary controller of the match lifecycle. Monitors critical win/loss conditions 
    /// across all players and directors, and commands the GameStateService to end the match.
    /// </summary>
    public interface IGameDirectorService : IService
    {
        /// <summary>
        /// Exposes a registration endpoint so the FleetDeploymentManager 
        /// can inject the correct players based on the chosen GameMode.
        /// </summary>
        public void RegisterPlayerTarget(HealthManager playerHealth);
        
        /// <summary>
        /// Broadcast when the current game is over.
        /// </summary>
        public event Action<MatchResult> OnMatchEnded;
    }
}
