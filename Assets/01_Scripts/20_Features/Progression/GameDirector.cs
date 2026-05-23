using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._20_Features.Vitals;
using UnityEngine;

namespace _01_Scripts._20_Features.Progression
{
    public enum MatchResult { Victory, Defeat }
    
    /// <summary>
    /// The primary controller of the match lifecycle. Monitors critical win/loss conditions 
    /// across all players and directors, and commands the GameStateService to end the match.
    /// </summary>
    public class GameDirector : MonoBehaviour, IGameDirectorService
    { 
        private readonly List<HealthManager> _playerHealths = new();
        public event Action<MatchResult> OnMatchEnded;

        private int _alivePlayers = 0;
        private IWaveSpawnService _waveDirector;
        private IFleetService _fleetDirector;
        
        private IGameStateProvider _gameState;

        private void Awake() {
            _waveDirector = ServiceLocator.Get<IWaveSpawnService>();
            _fleetDirector = ServiceLocator.Get <IFleetService>();
            _gameState = ServiceLocator.Get<IGameStateProvider>();
        }
        
        private void Start() {
            if (_waveDirector != null)
                _waveDirector.OnAllWavesCleared += HandleVictory;
            
            if (_fleetDirector != null)
                _fleetDirector.OnFleetDestroyed += HandleDefeat;
        }
        
        public void RegisterPlayerTarget(HealthManager playerHealth) {
            if (!playerHealth || _playerHealths.Contains(playerHealth)) 
                return;
                
            _playerHealths.Add(playerHealth);
            _alivePlayers++;
            playerHealth.OnZeroHealth += HandlePlayerDeath;
        }

        private void OnDestroy() {
            if (_waveDirector != null) 
                _waveDirector.OnAllWavesCleared -= HandleVictory;
            
            if (_fleetDirector != null) 
                _fleetDirector.OnFleetDestroyed -= HandleDefeat;
            
            foreach (var player in _playerHealths) {
                if (player) 
                    player.OnZeroHealth -= HandlePlayerDeath;
            }
        }

        private void HandlePlayerDeath(HealthManager player, GameObject killer) {
            if (_gameState.CurrentState == GameState.GameOver) 
                return;

            player.OnZeroHealth -= HandlePlayerDeath;
            _alivePlayers--;
            
            if (_alivePlayers <= 0)
                HandleDefeat();
        }

        private void HandleVictory() {
            StartCoroutine(EndGameRoutine(MatchResult.Victory, 4f));
        }

        private void HandleDefeat() {
            StartCoroutine(EndGameRoutine(MatchResult.Defeat, 1.5f));
        }

        private IEnumerator EndGameRoutine(MatchResult result, float delay) {
            if (_gameState.CurrentState == GameState.GameOver) 
                yield break;
            
            yield return new WaitForSeconds(delay);
            
            _gameState.EndGame();
            OnMatchEnded?.Invoke(result);
        }
    }
}
