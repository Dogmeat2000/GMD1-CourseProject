using System;
using _01_Scripts.Core.Interfaces;
using UnityEngine;

namespace _01_Scripts.Core.Services
{
    /// <summary>
    /// Centralized authority for game state and time manipulation.
    /// </summary>
    public class GameStateService : IService
    {
        /// <summary>
        /// Property that reflects the current state of the game (Deploying, Playing, Paused, GameOver)
        /// </summary>
        public GameState CurrentState { get; private set; }
        
        /// <summary>
        /// Event that is broadcast whenever the state of the game changes.
        /// </summary>
        public event Action<GameState> OnStateChanged;

        public GameStateService() {
            TransitionTo(GameState.Playing);
        }

        /// <summary>
        /// Pauses game. Ignored if the game has ended.
        /// </summary>
        public void PauseGame() {
            if (CurrentState == GameState.Playing)
                TransitionTo(GameState.Paused);
        }

        /// <summary>
        /// Resumes the game.
        /// </summary>
        public void ResumeGame() {
            if (CurrentState == GameState.Paused)
                TransitionTo(GameState.Playing);
        }

        /// <summary>
        /// Irreversibly locks the state to Game Over and halts time.
        /// </summary>
        public void EndGame() {
            if (CurrentState != GameState.GameOver)
                TransitionTo(GameState.GameOver);
        }

        private void TransitionTo(GameState newState) {
            CurrentState = newState;
            
            switch (newState) {
                case GameState.Playing:
                    Time.timeScale = 1f;
                    break;
                
                case GameState.Deploying:
                case GameState.Paused:
                case GameState.GameOver:
                    Time.timeScale = 0f;
                    break;
            }

            OnStateChanged?.Invoke(CurrentState);
        }
    }
}