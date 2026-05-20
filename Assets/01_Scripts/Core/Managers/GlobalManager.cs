using _01_Scripts.Core.Settings;
using UnityEngine;

namespace _01_Scripts.Core.Managers
{
    [RequireComponent(typeof(AudioSource))]
    public class GlobalManager : MonoBehaviour
    {
        public static GlobalManager Instance { get; private set; }

        [Header("Global Configuration")]
        [Tooltip("Slot the master GameSettings ScriptableObject here")]
        [SerializeField] private GlobalSettings globalSettings;
        
        [Tooltip("The persistent speaker for UI sounds that must survive scene loads")]
        [SerializeField] private AudioSource persistentUIAudioSource;
        
        public GlobalSettings GlobalSettings => globalSettings;

        private void Awake() {
            if (Instance && Instance != this) {
                Destroy(gameObject);
                return;
            }
            
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (!persistentUIAudioSource) 
                persistentUIAudioSource = GetComponent<AudioSource>();
        }
        
        public void PlayPersistentUISound(AudioClip clip) {
            if (persistentUIAudioSource && clip)
                persistentUIAudioSource.PlayOneShot(clip);
        }
    }
}
