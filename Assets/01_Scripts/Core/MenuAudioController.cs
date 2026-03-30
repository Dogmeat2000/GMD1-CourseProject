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
            menuMusic.Play();
            StartCoroutine(FadeInMusic());
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
    }
}