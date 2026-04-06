using System.Collections;
using UnityEngine;

namespace _01_Scripts.Core
{
    /**
     * <p>Handles the audio that is played inside menus</p>
     */
    public class MenuAudioController : MonoBehaviour
    {
        [SerializeField] private AudioSource menuMusic;
        [SerializeField] private float fadeDuration = 3.0f;
        [SerializeField] private float targetVolume = 1.0f;

        private void Start()
        {
            menuMusic.volume = 0f;
            StartPlaying();
        }

        private IEnumerator FadeInMusic()
        {
            float currentTime = 0;
            while (currentTime < fadeDuration)
            {
                currentTime += Time.deltaTime;
                menuMusic.volume = Mathf.Lerp(0f, targetVolume, currentTime / fadeDuration);
                yield return null;
            }
            
            menuMusic.volume = targetVolume;
        }

        public void SetTargetVolume(float volume)
        {
            targetVolume = Mathf.Clamp01(volume);
            menuMusic.volume = targetVolume;
        }

        private void OnDisable()
        {
            menuMusic.Stop();
        }

        private void OnEnable()
        {
            StartPlaying();
        }

        private void StartPlaying()
        {
            menuMusic.Play();
            StartCoroutine(FadeInMusic());
        }
    }
}