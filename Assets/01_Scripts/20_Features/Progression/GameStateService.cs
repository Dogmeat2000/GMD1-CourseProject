using System;
using UnityEngine;

namespace _01_Scripts._20_Features.Progression
{
    /// <summary>
    /// Centralized authority for game state and time manipulation.
    /// </summary>
    public class GameStateService : IGameStateProvider
    {
        public GameState CurrentState { get; set; }
        public event Action<GameState> OnStateChanged;

        public GameStateService() {
            TransitionTo(GameState.Playing);
        }
        
        public void PauseGame() {
            if (CurrentState == GameState.Playing)
                TransitionTo(GameState.Paused);
        }
        
        public void ResumeGame() {
            if (CurrentState == GameState.Paused)
                TransitionTo(GameState.Playing);
        }
        
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