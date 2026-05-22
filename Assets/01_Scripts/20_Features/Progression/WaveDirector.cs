using System;
using System.Collections;
using System.Collections.Generic;
using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._10_Core.Persistence;
using _01_Scripts._10_Core.Pooling;
using _01_Scripts._20_Features.Targeting;
using _01_Scripts._30_Actors.Enemies;
using _01_Scripts.Core.Targeting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _01_Scripts._20_Features.Progression
{
    public class WaveDirector : MonoBehaviour, IService
    {
        [Header("Setup")]
        [Tooltip("The player ship to use as the center of the forward spawn cone")]
        [SerializeField] private Transform playerShip;
        
        [Tooltip("Particle system for the water splash when a drone breaches sea level")]
        [SerializeField] private GameObject breachVfxPrefab; 

        [Header("Campaign Sequence")]
        [Tooltip("Slot the master campaign file for this level here")]
        [SerializeField] private LevelCampaign activeCampaign;
        
        [Tooltip("The time [s] to wait before starting the first wave")]
        [SerializeField] private int firstWaveDelay;
        
        [Tooltip("The time [s] to wait between each wave")]
        [SerializeField] private int otherWaveDelay;
        
        [Header("Narrative Broadcasts")]
        [SerializeField] private string startMessage = "PROTECT THE FLEET.";
        
        [SerializeField] private string clearMessage = "ALL WAVES CLEARED!";
        
        public event Action<int, int> OnWaveUpdated;
        public event Action<int> OnEnemyCountChanged;
        public event Action<string> OnStatusMessage;
        public event Action OnAllWavesCleared;
        
        private int _currentWaveIndex;
        private int _activeHostiles;
        private bool _isDeployingWave;
        private bool _isTransitioning;
        private LevelManager _levelManager;
        private BattlefieldRadar _battlefieldRadar;
        
        private WaitForSeconds _waveDelay;
        private readonly WaitForSeconds _scanDelay = new (10f);
        
        private void Awake() {
            _waveDelay = new WaitForSeconds(otherWaveDelay);
            _levelManager = ServiceLocator.Get<LevelManager>();
            _battlefieldRadar = ServiceLocator.Get<BattlefieldRadar>();
        }
        
        private void Start() {
            StartCoroutine(RadarSweepFailSafe());
            
            OnWaveUpdated?.Invoke(_currentWaveIndex, activeCampaign.Waves.Count);
            OnEnemyCountChanged?.Invoke(_activeHostiles);
            OnStatusMessage?.Invoke(startMessage);
            
            Invoke(nameof(BeginNextWave), firstWaveDelay);
        }
       
        /// <summary>
        /// Initiates the next wave in the sequence.
        /// </summary>
        public void BeginNextWave() {
            _isTransitioning = false;
            
            if (_currentWaveIndex >= activeCampaign.Waves.Count) {
                OnStatusMessage?.Invoke(clearMessage);
                OnAllWavesCleared?.Invoke();
                return;
            }

            WaveData currentWaveData = activeCampaign.Waves[_currentWaveIndex];
            Debug.Log($"Starting Wave {_currentWaveIndex + 1} / {activeCampaign.Waves.Count}");
            
            OnWaveUpdated?.Invoke(_currentWaveIndex + 1, activeCampaign.Waves.Count);
            OnStatusMessage?.Invoke($"WAVE {_currentWaveIndex + 1} INCOMING");
            
            StartCoroutine(DeployWaveRoutine(currentWaveData));
        }

        private IEnumerator DeployWaveRoutine(WaveData currentWave) {
            _isDeployingWave = true;
            
            List<EnemyProfile> assaultRoster = BuildSpawnRoster(currentWave);

            if (assaultRoster.Count == 0) {
                _isDeployingWave = false;
                AdvanceToNextWave();
                yield break;
            }

            LevelSettings settings = _levelManager.Settings;
            float totalWaveSpawnDuration = Random.Range(settings.WaveSpawnDurationMin, settings.WaveSpawnDurationMax);
            WaitForSeconds timeBetweenSpawns = new WaitForSeconds(totalWaveSpawnDuration / assaultRoster.Count);
            
            foreach (var enemy in assaultRoster) {
                DeploySingleUnit(enemy, settings);
                yield return timeBetweenSpawns;
            }
            
            _isDeployingWave = false;
            
            if (_activeHostiles <= 0)
                AdvanceToNextWave();
        }
        
        private void AdvanceToNextWave() {
            if (_isTransitioning) 
                return;
            
            _isTransitioning = true;
            
            OnStatusMessage?.Invoke($"WAVE {_currentWaveIndex + 1} CLEARED");
            _currentWaveIndex++;
            StartCoroutine(TransitionToNextWaveRoutine());
        }
        
        private IEnumerator TransitionToNextWaveRoutine() {
            if (otherWaveDelay > 0)
                yield return _waveDelay;
            
            BeginNextWave();
        }

        private List<EnemyProfile> BuildSpawnRoster(WaveData waveData) {
            List<EnemyProfile> roster = new List<EnemyProfile>();
            
            if (waveData.AllowedEnemies == null || waveData.AllowedEnemies.Count == 0) {
                Debug.LogError($"WaveData at index {_currentWaveIndex} has no Allowed Enemies! Aborting roster build.");
                return roster;
            }
            
            int players = _levelManager.ActivePlayerCount;
            float difficultyMod = _levelManager.GetDifficultyMultiplier();
            
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
            Vector3 spawnPoint = CalculateSpawnPosition(settings);
            Vector3 spawnDirection = (spawnPoint - playerShip.position).normalized;
            int poolSize = _levelManager.Settings.DefaultObjectPoolSize;
            int maxPoolSize = _levelManager.Settings.MaxDefaultObjectPoolSize;
            spawnDirection.y = 0;

            var pooledObj = UniversalPoolService.Instance.Spawn(profile.Prefab, spawnPoint, Quaternion.LookRotation(spawnDirection), poolSize, maxPoolSize);
            
            _activeHostiles++;
            OnEnemyCountChanged?.Invoke(_activeHostiles);
            
            if (pooledObj.gameObject.TryGetComponent<EnemyController>(out var enemyCtrl))
                enemyCtrl.OnRemovedFromBoard += HandleEnemyRemoved;
            
            if (breachVfxPrefab) {
                Vector3 surfacePoint = spawnPoint;
                surfacePoint.y = settings.OceanSurfaceY;
                UniversalPoolService.Instance.Spawn(breachVfxPrefab, surfacePoint, Quaternion.identity,poolSize , maxPoolSize);
            }
        }
        
        private Vector3 CalculateSpawnPosition(LevelSettings settings) {
            float randomAngle = Random.Range(-settings.SpawnAngleLimit, settings.SpawnAngleLimit);
            float randomDistance = Random.Range(settings.MinSpawnDistance, settings.MaxSpawnDistance);
            
            Vector3 flatForward = playerShip.forward;
            flatForward.y = 0;
            flatForward.Normalize();
            
            Vector3 spawnDirection = Quaternion.Euler(0, randomAngle, 0) * flatForward;
            Vector3 spawnPoint = playerShip.position + (spawnDirection * randomDistance);
            spawnPoint.y = settings.SpawnDepthY;

            return spawnPoint;
        }
        
        private void HandleEnemyRemoved(EnemyController source) {
            source.OnRemovedFromBoard -= HandleEnemyRemoved;
            
            _activeHostiles--;
            OnEnemyCountChanged?.Invoke(_activeHostiles);
            
            if (_activeHostiles <= 0)
                AdvanceToNextWave();
        }
        
        /// <summary>
        /// Sweeps the battlefield every 10 seconds. If active hostiles dropped below 0 due to 
        /// engine deletion (falling out of bounds) rather than combat, it forces the next wave.
        /// </summary>
        private IEnumerator RadarSweepFailSafe() {
            while (true) {
                yield return _scanDelay;
                
                if (_activeHostiles > 0 && !_isDeployingWave) {
                    int actualEnemies = _battlefieldRadar.GetActiveHostileCount();
                    
                    if (actualEnemies == 0) {
                        Debug.LogWarning("Failsafe sweep detected 0 physical enemies, but ActiveHostiles was > 0. Resolving soft-lock.");
                        _activeHostiles = 0;
                        _currentWaveIndex++;
                        BeginNextWave();
                    }
                }
            }
        }
    }
}