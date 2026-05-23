using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._10_Core.Persistence;

namespace _01_Scripts._20_Features.Progression
{
    /// <summary>
    /// Manages level specific configuration and setup.
    /// </summary>
    public interface ILevelManager : IService
    {
        /// <summary>
        /// Public getter for all other scripts to read from.
        /// </summary>
        public LevelSettings Settings { get; }
        
        /// <summary>
        /// Number of active players (e.g., 1 for Solo, 2 for Split-Screen).
        /// </summary>
        public int ActivePlayerCount { get; set; }
        
        /// <summary>
        /// The difficulty selected by the host/players when launching the level.
        /// </summary>
        public GameDifficulty CurrentDifficulty { get; set; }

        /// <summary>
        /// Returns a mathematical multiplier based on the chosen difficulty.
        /// </summary>
        public float GetDifficultyMultiplier();
    }
}
