using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace _01_Scripts.Core.Managers
{
    public enum MatchResult { Victory, Defeat }
    
    public class GameDirector : MonoBehaviour
    { 
        public static GameDirector Instance { get; private set; }

        [Header("Players")]
        [Tooltip("Assign the HealthManagers for all players in the game level")]
        [SerializeField] private List<HealthManager> playerHealths = new();

        public event Action<MatchResult> OnMatchEnded;

        private int _alivePlayers;
        private bool _matchIsOver = false;

        private void Awake() {
            if (Instance && Instance != this) 
                Destroy(gameObject);
            else 
                Instance = this;
        }

        private void Start() {
            _alivePlayers = playerHealths.Count;

            if (WaveDirector.Instance) {
                WaveDirector.Instance.OnAllWavesCleared += HandleVictory;
            }
            
            if (FleetDirector.Instance) {
                FleetDirector.Instance.OnFleetDestroyed += HandleDefeat;
            }

            foreach (var player in playerHealths) {
                if (player) player.OnZeroHealth += HandlePlayerDeath;
            }
        }

        private void OnDestroy() {
            if (WaveDirector.Instance) 
                WaveDirector.Instance.OnAllWavesCleared -= HandleVictory;
            
            if (FleetDirector.Instance) 
                FleetDirector.Instance.OnFleetDestroyed -= HandleDefeat;
            
            foreach (var player in playerHealths) {
                if (player) 
                    player.OnZeroHealth -= HandlePlayerDeath;
            }
        }

        private void HandlePlayerDeath(HealthManager player, GameObject killer) {
            if (_matchIsOver) 
                return;

            player.OnZeroHealth -= HandlePlayerDeath;
            _alivePlayers--;
            
            if (_alivePlayers <= 0) {
                HandleDefeat();
            }
        }

        private void HandleVictory() {
            StartCoroutine(EndMatchRoutine(MatchResult.Victory, 4f));
        }

        private void HandleDefeat() {
            StartCoroutine(EndMatchRoutine(MatchResult.Defeat, 1.5f));
        }

        private IEnumerator EndMatchRoutine(MatchResult result, float delay) {
            if (_matchIsOver) 
                yield break;
            
            _matchIsOver = true;
            
            yield return new WaitForSeconds(delay);
            
            Time.timeScale = 0f; 
            OnMatchEnded?.Invoke(result);
        }
    }
}
