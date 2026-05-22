using System.Collections;
using UnityEngine;

namespace _01_Scripts._10_Core.Audio
{
    /// <summary>
    /// <p>Handles the audio that is played inside menus</p>
    /// </summary>
    public class MenuAudioController : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Audio Source Game Object responsible for playing the Menu Music")]
        [SerializeField] private AudioSource menuMusic;
        
        [Tooltip("How long [s] the fade effect should take, as the music starts and ends.")]
        [SerializeField] private float fadeDuration = 3.0f;

        private void Start() {
            menuMusic.volume = 0f;
            StartPlaying();
        }

        private IEnumerator FadeInMusic() {
            float currentTime = 0;
            while (currentTime < fadeDuration) {
                currentTime += Time.deltaTime;
                menuMusic.volume = Mathf.Lerp(0f, GlobalManager.Instance.GlobalSettings.GetMusicVolume(), currentTime / fadeDuration);
                yield return null;
            }
            
            menuMusic.volume = GlobalManager.Instance.GlobalSettings.GetMusicVolume();
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