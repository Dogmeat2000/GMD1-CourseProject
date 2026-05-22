using System;
using _01_Scripts._10_Core.DependencyInjection;

namespace _01_Scripts._20_Features.Progression
{
    /// <summary>
    /// Primary interface that GameState service providers must implement.
    /// </summary>
    public interface IGameStateProvider : IService
    {
        /// <summary>
        /// Property that reflects the current state of the game (Deploying, Playing, Paused, GameOver)
        /// </summary>
        public GameState CurrentState { get; set; }
        
        /// <summary>
        /// Event that is broadcast whenever the state of the game changes.
        /// </summary>
        public event Action<GameState> OnStateChanged;

        /// <summary>
        /// Pauses game. Ignored if the game has ended.
        /// </summary>
        public void PauseGame();

        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void ResumeGame();

        /// <summary>
        /// Irreversibly locks the state to Game Over and halts time.
        /// </summary>
        public void EndGame();
    }
}
