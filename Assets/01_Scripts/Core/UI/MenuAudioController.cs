using System.Collections;
using UnityEngine;

namespace _01_Scripts.Core.UI
{
    /// <summary>
    /// <p>Handles the audio that is played inside menus</p>
    /// </summary>
    public class MenuAudioController : MonoBehaviour
    {
        // TODO Add description
        [SerializeField] private AudioSource menuMusic;
        
        // TODO Add description
        [SerializeField] private float fadeDuration = 3.0f;
        
        // TODO Add description
        [SerializeField] private float targetVolume = 1.0f;

        private void Start() {
            menuMusic.volume = 0f;
            StartPlaying();
        }

        private IEnumerator FadeInMusic() {
            float currentTime = 0;
            while (currentTime < fadeDuration) {
                currentTime += Time.deltaTime;
                menuMusic.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeDuration);
                yield return null;
            }
            
            menuMusic.volume = targetVolume;
        }

        // TODO Add description
        // Consider refactoring, or moving this out into the SettingsController to read from the GlobalSettings!
        public void SetTargetVolume(float volume) {
            targetVolume = Mathf.Clamp01(volume);
            menuMusic.volume = targetVolume;
        }

        private void OnDisable() {
            menuMusic.Stop();
        }

        private void OnEnable() {
            StartPlaying();
        }

        private void StartPlaying() {
            menuMusic.Play();
            StartCoroutine(FadeInMusic());
        }
    }
}