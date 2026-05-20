using UnityEngine;

namespace _01_Scripts.Core.Audio
{
    public class ThunderAudio : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Thunder rumble audio files")]
        [SerializeField] public AudioClip[] thunderRumbles;

        [Tooltip("The audio source")] 
        [SerializeField] public AudioSource audioSource;
        
        [Tooltip("The Thunder Storm particle system")] 
        [SerializeField] public ParticleSystem thunderStormPS;
    
        private int _lastStrikeCount = 0;

        void Update() {
            if (!thunderStormPS)
                return;
            
            int currentStrikes = thunderStormPS.particleCount;
            
            if (currentStrikes > _lastStrikeCount) {
                CalculateAndPlayThunder();
            }
        
            _lastStrikeCount = currentStrikes;
        }

        private void CalculateAndPlayThunder() {
            if (!audioSource)
                return;
            
            if (thunderRumbles.Length == 0) 
                return;
            
            AudioClip thunder = thunderRumbles[Random.Range(0, thunderRumbles.Length)];
            audioSource.clip = thunder;
            audioSource.pitch = Random.Range(0.85f, 1.15f);
            audioSource.PlayDelayed(1);
        }
    }
}
