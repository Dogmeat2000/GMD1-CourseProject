using System.Collections;
using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._20_Features.Progression;
using TMPro;
using UnityEngine;

namespace _01_Scripts._40_UI.HUD
{
    /// <summary>
    /// Presenter class responsible for displaying information about the current Wave of enemies, to players in their HUD.
    /// </summary>
    public class WaveInformationHUD : MonoBehaviour
    {
        [Header("Readout Displays")]
        [Tooltip("The TextMeshPro GameObject to display information about current wave. Ex: [WAVE 1 / 10]")]
        [SerializeField] private TextMeshProUGUI waveText;
        
        [Tooltip("The TextMeshPro GameObject to display information about number of hostiles. Ex: HOSTILES: 7")]
        [SerializeField] private TextMeshProUGUI enemyCountText;
        
        [Tooltip("The TextMeshPro GameObject to display brief status information in. Ex.: INCOMING ENEMIES")]
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("HUD Settings")]
        [Tooltip("How long central status messages stay on screen before fading")]
        [SerializeField] private float statusDisplayDuration = 3f;

        private WaitForSeconds _delay;
        private Coroutine _statusClearCoroutine;
        private IWaveSpawnService _waveDirector;

        private void Awake() {
            _delay = new WaitForSeconds(statusDisplayDuration);
            _waveDirector = ServiceLocator.Get<IWaveSpawnService>();
        }
        
        private void OnEnable() {
            if (_waveDirector != null)
                SubscribeToWaveUpdates();
            else
                StartCoroutine(DelayedSubscription());
        }

        private void OnDisable() {
            if (_waveDirector != null) {
                _waveDirector.OnWaveUpdated -= UpdateWaveDisplay;
                _waveDirector.OnEnemyCountChanged -= UpdateEnemyCount;
                _waveDirector.OnStatusMessage -= DisplayStatus;
            }
        }

        private IEnumerator DelayedSubscription() {
            yield return new WaitUntil(() => _waveDirector != null);
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

            if (_statusClearCoroutine != null)
                StopCoroutine(_statusClearCoroutine);
            
            _statusClearCoroutine = StartCoroutine(ClearStatusRoutine());
        }

        private IEnumerator ClearStatusRoutine() {
            yield return _delay;
            
            if (!statusText) 
                yield break;
            
            statusText.gameObject.SetActive(false);
            statusText.text = string.Empty;
        }
    }
}
