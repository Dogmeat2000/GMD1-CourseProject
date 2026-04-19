using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts.Core.Enemies;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Settings;
using _01_Scripts.Core.Waves;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01_Scripts.Core.Managers
{
    public class WaveDirector : MonoBehaviour
    {
        [Header("Telemetry")]
        [Tooltip("The player ship to use as the center of the forward spawn cone")]
        [SerializeField] 
        private Transform playerShip;
        
        [Tooltip("Particle system for the water splash when a drone breaches sea level")]
        [SerializeField] 
        private GameObject breachVfxPrefab; 

        [Header("Campaign Sequence")]
        [Tooltip("Slot the master campaign file for this level here")]
        [SerializeField] 
        private LevelCampaign activeCampaign;
        
        [Tooltip("The time [s] to wait before starting the first wave")]
        [SerializeField] 
        private int firstWaveDelay;
        
        public static WaveDirector Instance { get; private set; }
        public event Action<int, int> OnWaveUpdated;
        public event Action<int> OnEnemyCountChanged;
        public event Action<string> OnStatusMessage;
        public event Action OnAllWavesCleared;
        
        private int _currentWaveIndex = 0;
        private int _activeHostiles = 0;
        private bool _isDeployingWave = false;
        
        private void Awake() {
            if (Instance && Instance != this) 
                Destroy(gameObject);
            else 
                Instance = this;
        }
        
        /** <summary>
         * Initiates the next wave in the sequence.
         * </summary>
         */
        public void BeginNextWave() {
            if (_currentWaveIndex >= activeCampaign.Waves.Count) {
                OnStatusMessage?.Invoke("ALL WAVES CLEARED!"); // TODO: This should become a serialized field!
                OnAllWavesCleared?.Invoke();
                return;
            }

            WaveData currentWaveData = activeCampaign.Waves[_currentWaveIndex];
            Debug.Log($"Starting Wave {_currentWaveIndex + 1} / {activeCampaign.Waves.Count}");
            OnWaveUpdated?.Invoke(_currentWaveIndex + 1, activeCampaign.Waves.Count);
            OnStatusMessage?.Invoke($"WAVE {_currentWaveIndex + 1} INCOMING");
            
            StartCoroutine(DeployWaveRoutine(currentWaveData));
        }
        
        private void Start() {
            StartCoroutine(RadarSweepFailSafe());
            
            OnWaveUpdated?.Invoke(_currentWaveIndex, activeCampaign.Waves.Count);
            OnEnemyCountChanged?.Invoke(_activeHostiles);
            
            OnStatusMessage?.Invoke("PROTECT THE FLEET."); // TODO: This should become a serialized field!
            
            Invoke(nameof(BeginNextWave), firstWaveDelay);
        }

        private IEnumerator DeployWaveRoutine(WaveData currentWave) {
            _isDeployingWave = true;
            
            List<EnemyProfile> assaultRoster = BuildSpawnRoster(currentWave);
            
            if (assaultRoster.Count == 0) 
                yield break;

            LevelSettings settings = LevelManager.Instance.Settings;
            float totalWaveSpawnDuration = Random.Range(settings.WaveSpawnDurationMin, settings.WaveSpawnDurationMax);
            float timeBetweenSpawns = totalWaveSpawnDuration / assaultRoster.Count;
            
            foreach (var enemy in assaultRoster) {
                DeploySingleUnit(enemy, settings);
                yield return new WaitForSeconds(timeBetweenSpawns);
            }
            
            _isDeployingWave = false;
            
            if (_activeHostiles <= 0) {
                AdvanceToNextWave();
            }
        }
        
        private void AdvanceToNextWave() {
            OnStatusMessage?.Invoke($"WAVE {_currentWaveIndex + 1} CLEARED");
            _currentWaveIndex++;
            BeginNextWave();
        }

        private List<EnemyProfile> BuildSpawnRoster(WaveData waveData) {
            List<EnemyProfile> roster = new List<EnemyProfile>();
            
            if (waveData.AllowedEnemies == null || waveData.AllowedEnemies.Count == 0) {
                Debug.LogError($"WaveDirector: WaveData at index {_currentWaveIndex} has no Allowed Enemies! Aborting roster build.");
                return roster;
            }
            
            int players = LevelManager.Instance.ActivePlayerCount;
            float difficultyMod = LevelManager.Instance.GetDifficultyMultiplier();
            
            int adjustedBudget = Mathf.RoundToInt(waveData.ThreatBudget * players * difficultyMod);
            
            Debug.Log($"Base Budget: {waveData.ThreatBudget} | Adjusted Budget: {adjustedBudget} (Players: {players}, Difficulty: {difficultyMod})");
            
            int remainingBudget = adjustedBudget;
            int cheapestCost = int.MaxValue;
            
            foreach(EnemyProfile e in waveData.AllowedEnemies) {
                if (e.ThreatCost < cheapestCost) {
                    cheapestCost = e.ThreatCost;
                }
            }

            int safetyBrake = 5000;
            while (remainingBudget >= cheapestCost && safetyBrake > 0) {
                EnemyProfile randomEnemy = waveData.AllowedEnemies[Random.Range(0, waveData.AllowedEnemies.Count)];
                
                if (remainingBudget >= randomEnemy.ThreatCost) {
                    roster.Add(randomEnemy);
                    remainingBudget -= randomEnemy.ThreatCost;
                }
                safetyBrake--;
            }
            return roster;
        }

        private void DeploySingleUnit(EnemyProfile profile, LevelSettings settings) {
            float randomAngle = Random.Range(-settings.SpawnAngleLimit, settings.SpawnAngleLimit);
            float randomDistance = Random.Range(settings.MinSpawnDistance, settings.MaxSpawnDistance);
            
            Vector3 flatForward = playerShip.forward;
            flatForward.y = 0;
            flatForward.Normalize();
            
            Vector3 spawnDirection = Quaternion.Euler(0, randomAngle, 0) * flatForward;
            
            Vector3 spawnPoint = playerShip.position + (spawnDirection * randomDistance);
            
            spawnPoint.y = settings.SpawnDepthY;

            var pooledObj = UniversalPoolService.Instance.Spawn(profile.Prefab, spawnPoint, Quaternion.LookRotation(spawnDirection));
            
            _activeHostiles++;
            OnEnemyCountChanged?.Invoke(_activeHostiles);
            
            if (pooledObj.gameObject.TryGetComponent<EnemyController>(out var enemyCtrl)) {
                enemyCtrl.OnRemovedFromBoard += HandleEnemyRemoved;
            }
            
            if (breachVfxPrefab) {
                Vector3 surfacePoint = spawnPoint;
                surfacePoint.y = settings.OceanSurfaceY;
                UniversalPoolService.Instance.Spawn(breachVfxPrefab, surfacePoint, Quaternion.identity);
            }
        }
        
        private void HandleEnemyRemoved(EnemyController source) {
            source.OnRemovedFromBoard -= HandleEnemyRemoved;
            
            _activeHostiles--;
            OnEnemyCountChanged?.Invoke(_activeHostiles);
            
            if (_activeHostiles <= 0) {
                AdvanceToNextWave();
            }
        }
        
        /** <summary>
         * Sweeps the battlefield every 10 seconds. If active hostiles dropped below 0 due to 
         * engine deletion (falling out of bounds) rather than combat, it forces the next wave.
         * </summary>
         */
        private IEnumerator RadarSweepFailSafe() {
            WaitForSeconds wait = new WaitForSeconds(10f);
            while (true) {
                yield return wait;
                
                if (_activeHostiles > 0 && !_isDeployingWave) {
                    int actualEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length; 
                    
                    if (actualEnemies == 0) {
                        Debug.LogWarning("WaveDirector: Failsafe sweep detected 0 physical enemies, but ActiveHostiles was > 0. Resolving soft-lock.");
                        _activeHostiles = 0;
                        _currentWaveIndex++;
                        BeginNextWave();
                    }
                }
            }
        }
    }
}