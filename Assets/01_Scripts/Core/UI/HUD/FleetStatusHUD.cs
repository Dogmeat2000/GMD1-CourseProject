using System.Collections;
using _01_Scripts.Core.Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Core.UI.HUD
{
    public class FleetStatusHUD : MonoBehaviour
    {
        [Header("Readout Displays")]
        [SerializeField] private TextMeshProUGUI fleetCountText;
        [SerializeField] private Slider fleetAverageHealthSlider;
        [SerializeField] private Image sliderFillImage; 
        [SerializeField] private Gradient healthGradient;

        private void OnEnable() {
            if (FleetDirector.Instance) {
                SubscribeToTelemetry();
            } else {
                StartCoroutine(DelayedSubscription());
            }
        }

        private void OnDisable() {
            if (FleetDirector.Instance) {
                FleetDirector.Instance.OnFleetCountChanged -= UpdateFleetCount;
                FleetDirector.Instance.OnFleetHealthAverageChanged -= UpdateFleetHealth;
            }
        }

        private IEnumerator DelayedSubscription() {
            yield return new WaitUntil(() => FleetDirector.Instance);
            SubscribeToTelemetry();
        }

        private void SubscribeToTelemetry() {
            FleetDirector.Instance.OnFleetCountChanged += UpdateFleetCount;
            FleetDirector.Instance.OnFleetHealthAverageChanged += UpdateFleetHealth;
            FleetDirector.Instance.RequestFleetStatusUpdate();
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
