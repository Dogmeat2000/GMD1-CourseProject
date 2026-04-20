using System.Collections;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Core.UI.HUD
{
    public class FleetStatusHUD : MonoBehaviour
    {
        [Header("Readout Displays")]
        [SerializeField] 
        private TextMeshProUGUI fleetCountText;
        
        [SerializeField] 
        private Slider fleetAverageHealthSlider;
        
        [SerializeField] 
        private Image sliderFillImage; 
        
        [SerializeField] 
        private Gradient healthGradient;

        private FleetDirector _fleetDirector;

        private void Awake() {
            _fleetDirector = ServiceLocator.Get<FleetDirector>();
        }
        
        private void OnEnable() {
            if (_fleetDirector) {
                SubscribeToTelemetry();
            } else {
                StartCoroutine(DelayedSubscription());
            }
        }

        private void OnDisable() {
            if (_fleetDirector) {
                _fleetDirector.OnFleetCountChanged -= UpdateFleetCount;
                _fleetDirector.OnFleetHealthAverageChanged -= UpdateFleetHealth;
            }
        }

        private IEnumerator DelayedSubscription() {
            yield return new WaitUntil(() => _fleetDirector);
            SubscribeToTelemetry();
        }

        private void SubscribeToTelemetry() {
            _fleetDirector.OnFleetCountChanged += UpdateFleetCount;
            _fleetDirector.OnFleetHealthAverageChanged += UpdateFleetHealth;
            _fleetDirector.RequestFleetStatusUpdate();
        }

        private void UpdateFleetCount(int currentAlive, int totalStarting) {
            if (fleetCountText) {
                fleetCountText.text = $"FLEET: {currentAlive} / {totalStarting}";
                fleetCountText.color = currentAlive <= (totalStarting / 2) ? Color.red : Color.white;
            }
        }

        private void UpdateFleetHealth(float averageHealthPercentage) {
            if (fleetAverageHealthSlider) {
                fleetAverageHealthSlider.value = averageHealthPercentage;
            }

            if (sliderFillImage && healthGradient != null) {
                sliderFillImage.color = healthGradient.Evaluate(averageHealthPercentage);
            }
        }
    }
}
