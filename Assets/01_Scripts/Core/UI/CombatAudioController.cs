using System.Collections;
using _01_Scripts.Core.Managers;
using UnityEngine;
using UnityEngine.Audio;

namespace _01_Scripts.Core.UI
{
    /// <summary>
    /// Manages the sequential playback of deployment voiceovers and randomized combat music.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class CombatAudioController : MonoBehaviour
    {
        [Header("Deployment Voiceover")] 
        [Tooltip("The spoken word intro to play at the start of the mission.")] 
        [SerializeField]
        private AudioClip introVoiceover;

        [Tooltip("Time [seconds] to wait before playing the intro.")] 
        [SerializeField]
        private float initialDelay = 2f;

        [Header("Combat Tracks")] 
        [Tooltip("A collection of background music tracks. One will be picked at random.")] 
        [SerializeField]
        private AudioClip[] combatTracks;

        [Tooltip("Time [s] to wait between the intro and music, or between consecutive music tracks.")] 
        [SerializeField]
        private float delayBetweenTracks = 1.5f;

        private AudioSource _audioSource;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            
            if (GlobalManager.Instance && GlobalManager.Instance.GlobalSettings.MusicMixerGroup) {
                _audioSource.outputAudioMixerGroup = GlobalManager.Instance.GlobalSettings.MusicMixerGroup;
            } else {
                Debug.LogWarning("GlobalSettings is missing the MusicMixerGroup! Ducking will fail.");
            }
        }

        private void Start() {
            StartCoroutine(AudioSequenceRoutine());
        }

        private IEnumerator AudioSequenceRoutine() {
            if (initialDelay > 0) {
                yield return new WaitForSeconds(initialDelay);
            }
            
            if (introVoiceover) {
                _audioSource.clip = introVoiceover;
                _audioSource.Play();
                yield return new WaitForSeconds(introVoiceover.length);
            }
            
            if (combatTracks == null || combatTracks.Length == 0) {
                Debug.LogWarning("No combat tracks assigned. Ending audio sequence.");
                yield break;
            }

            while (true) {
                if (delayBetweenTracks > 0) {
                    yield return new WaitForSeconds(delayBetweenTracks);
                }
                
                AudioClip nextTrack = combatTracks[Random.Range(0, combatTracks.Length)];

                if (nextTrack) {
                    _audioSource.clip = nextTrack;
                    _audioSource.Play();
                    yield return new WaitForSeconds(nextTrack.length);
                } else {
                    yield return null;
                }
            }
        }
    }
}
