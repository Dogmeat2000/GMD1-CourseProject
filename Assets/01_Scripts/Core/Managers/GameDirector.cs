using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Services;

namespace _01_Scripts.Core.Managers
{
    public enum MatchResult { Victory, Defeat }
    
    /// <summary>
    /// The primary controller of the match lifecycle. Monitors critical win/loss conditions 
    /// across all players and directors, and commands the GameStateService to end the match.
    /// </summary>
    public class GameDirector : MonoBehaviour, IService
    { 
        private readonly List<HealthManager> _playerHealths = new();
        public event Action<MatchResult> OnMatchEnded;

        private int _alivePlayers = 0;
        private WaveDirector _waveDirector;
        private FleetDirector _fleetDirector;
        
        private GameStateService _gameState;

        private void Awake() {
            _waveDirector = ServiceLocator.Get<WaveDirector>();
            _fleetDirector = ServiceLocator.Get < FleetDirector>();
            _gameState = ServiceLocator.Get<GameStateService>();
        }
        
        private void Start() {
            if (_waveDirector) {
                _waveDirector.OnAllWavesCleared += HandleVictory;
            }
            
            if (_fleetDirector) {
                _fleetDirector.OnFleetDestroyed += HandleDefeat;
            }
        }
        
        /// <summary>
        /// Exposes a registration endpoint so the FleetDeploymentManager 
        /// can inject the correct players based on the chosen GameMode.
        /// </summary>
        public void RegisterPlayerTarget(HealthManager playerHealth) {
            if (!playerHealth || _playerHealths.Contains(playerHealth)) 
                return;
                
            _playerHealths.Add(playerHealth);
            _alivePlayers++;
            playerHealth.OnZeroHealth += HandlePlayerDeath;
        }

        private void OnDestroy() {
            if (_waveDirector) 
                _waveDirector.OnAllWavesCleared -= HandleVictory;
            
            if (_fleetDirector) 
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
            
            if (_alivePlayers <= 0) {
                HandleDefeat();
            }
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
