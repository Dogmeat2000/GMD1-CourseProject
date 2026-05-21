using System.Collections;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using UnityEngine;

namespace _01_Scripts.Core.Audio
{
    /// <summary>
    /// Manages the sequential playback of deployment voiceover and randomized combat music.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class CombatAudioController : MonoBehaviour
    {
        [Header("Deployment Voiceover")] 
        [Tooltip("The spoken word intro to play at the start of the mission.")] 
        [SerializeField] private AudioClip introVoiceover;

        [Tooltip("Time [seconds] to wait before playing the intro.")] 
        [SerializeField] private float initialDelay = 2f;

        [Header("Combat Tracks")] 
        [Tooltip("A collection of background music tracks. One will be picked at random.")] 
        [SerializeField] private AudioClip[] combatTracks;

        [Tooltip("Time [s] to wait between the intro and music, or between consecutive music tracks.")] 
        [SerializeField] private float delayBetweenTracks = 1.5f;
        
        [Tooltip("How long [s] it takes to fade the music out upon Game Over.")]
        [SerializeField] private float fadeOutDuration = 2.0f;

        private AudioSource _audioSource;
        private GameStateService _gameState;
        private Coroutine _sequenceRoutine;
        
        private WaitForSeconds _initialDelay;
        private WaitForSeconds _introVoiceOverDelay;
        private WaitForSeconds _delayBetweenTracks;

        private void Awake() {
            _audioSource = GetComponent<AudioSource>();
            _audioSource.playOnAwake = false;
            _audioSource.loop = false;
            
            if (GlobalManager.Instance && GlobalManager.Instance.GlobalSettings.MusicMixerGroup) {
                _audioSource.outputAudioMixerGroup = GlobalManager.Instance.GlobalSettings.MusicMixerGroup;
            } else {
                Debug.LogWarning("GlobalSettings is missing the MusicMixerGroup! Ducking will fail.");
            }
            
            _gameState = ServiceLocator.Get<GameStateService>();
        }

        private void Start() {
            _initialDelay = new WaitForSeconds(initialDelay);
            _introVoiceOverDelay = new WaitForSeconds(introVoiceover.length);
            _delayBetweenTracks = new WaitForSeconds(delayBetweenTracks);
            
            _sequenceRoutine = StartCoroutine(AudioSequenceRoutine());
        }
        
        private void OnEnable() {
            if (_gameState != null)
                _gameState.OnStateChanged += HandleStateChanged;
        }

        private void OnDisable() {
            if (_gameState != null)
                _gameState.OnStateChanged -= HandleStateChanged;
        }
        
        private void HandleStateChanged(GameState state) {
            if (state != GameState.GameOver) 
                return;
            
            if (_sequenceRoutine != null)
                StopCoroutine(_sequenceRoutine);
            
            StartCoroutine(FadeOutRoutine());
        }
        
        private IEnumerator FadeOutRoutine() {
            float startVolume = _audioSource.volume;
            float timeElapsed = 0f;
            
            while (timeElapsed < fadeOutDuration) {
                timeElapsed += Time.unscaledDeltaTime;
                _audioSource.volume = Mathf.Lerp(startVolume, 0f, timeElapsed / fadeOutDuration);
                yield return null;
            }

            _audioSource.volume = 0f;
            _audioSource.Stop();
        }

        private IEnumerator AudioSequenceRoutine() {
            if (initialDelay > 0)
                yield return _initialDelay;
            
            if (introVoiceover) {
                _audioSource.clip = introVoiceover;
                _audioSource.Play();
                yield return _introVoiceOverDelay;
            }
            
            if (combatTracks == null || combatTracks.Length == 0) {
                Debug.LogWarning("No combat tracks assigned. Ending audio sequence.");
                yield break;
            }

            while (true) {
                if (delayBetweenTracks > 0)
                    yield return _delayBetweenTracks;
                
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
