using System.Collections;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using TMPro;
using UnityEngine;

namespace _01_Scripts.Core.UI.HUD
{
    public class WaveInformationHUD : MonoBehaviour
    {
        [Header("Readout Displays")]
        [SerializeField] 
        private TextMeshProUGUI waveText;
        
        [SerializeField] 
        private TextMeshProUGUI enemyCountText;
        
        [SerializeField] 
        private TextMeshProUGUI statusText;

        [Header("HUD Settings")]
        [Tooltip("How long central status messages stay on screen before fading")]
        [SerializeField] 
        private float statusDisplayDuration = 3f;

        private Coroutine _statusClearCoroutine;
        private WaveDirector _waveDirector;

        private void Awake() {
            _waveDirector = ServiceLocator.Get<WaveDirector>();
        }
        
        private void OnEnable() {
            if (_waveDirector) {
                SubscribeToWaveUpdates();
            } else {
                StartCoroutine(DelayedSubscription());
            }
        }

        private void OnDisable() {
            if (_waveDirector) {
                _waveDirector.OnWaveUpdated -= UpdateWaveDisplay;
                _waveDirector.OnEnemyCountChanged -= UpdateEnemyCount;
                _waveDirector.OnStatusMessage -= DisplayStatus;
            }
        }

        private IEnumerator DelayedSubscription() {
            yield return new WaitUntil(() => _waveDirector);
            SubscribeToWaveUpdates();
        }

        private void SubscribeToWaveUpdates() {
            _waveDirector.OnWaveUpdated += UpdateWaveDisplay;
            _waveDirector.OnEnemyCountChanged += UpdateEnemyCount;
            _waveDirector.OnStatusMessage += DisplayStatus;
            
            if (statusText) 
                statusText.gameObject.SetActive(false);
        }

        private void UpdateWaveDisplay(int currentWave, int totalWaves) {
            if (waveText) 
                waveText.text = $"WAVE {currentWave} / {totalWaves}";
        }

        private void UpdateEnemyCount(int activeEnemies) {
            if (!enemyCountText) 
                return;
            
            enemyCountText.text = $"HOSTILES: {activeEnemies}";
            enemyCountText.color = activeEnemies > 0 ? Color.red : Color.green;
        }

        private void DisplayStatus(string message) {
            if (!statusText) 
                return;

            statusText.text = message;
            statusText.gameObject.SetActive(true);

            if (_statusClearCoroutine != null) {
                StopCoroutine(_statusClearCoroutine);
            }
            _statusClearCoroutine = StartCoroutine(ClearStatusRoutine());
        }

        private IEnumerator ClearStatusRoutine() {
            yield return new WaitForSeconds(statusDisplayDuration);
            
            if (!statusText) 
                yield break;
            
            statusText.gameObject.SetActive(false);
            statusText.text = string.Empty;
        }
    }
}
