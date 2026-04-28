using System;
using System.Collections;
using UnityEngine;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Managers;

namespace _01_Scripts.Core.VFX
{
    public class VfxPoolReturn : MonoBehaviour, IPoolable
    {
        [Header("Acoustics (Optional)")]
        [Tooltip("The speaker attached to this VFX prefab.")]
        [SerializeField] 
        private AudioSource vfxAudioSource;
        
        [Tooltip("The sound to play when this VFX spawns.")]
        [SerializeField] 
        private AudioClip vfxSound;
        
        private Action<IPoolable> _returnToPoolCommand;
        private ParticleSystem[] _particleSystems; 
        private Coroutine _monitorCoroutine;

        private void Awake() {
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
            
            if (vfxAudioSource && GlobalManager.Instance) {
                vfxAudioSource.outputAudioMixerGroup = GlobalManager.Instance.GlobalSettings.SfxMixerGroup;
            }
        }

        public void Initialize(Action<IPoolable> returnAction) {
            _returnToPoolCommand = returnAction;
        }

        public void OnSpawned() {
            foreach (var ps in _particleSystems) {
                if (ps) ps.Play(true);
            }
            
            if (vfxAudioSource && vfxSound) {
                vfxAudioSource.PlayOneShot(vfxSound);
            }
            
            if (_monitorCoroutine != null) StopCoroutine(_monitorCoroutine);
            _monitorCoroutine = StartCoroutine(MonitorVfxRoutine());
        }

        public void OnDespawned() {
            if (_monitorCoroutine != null) StopCoroutine(_monitorCoroutine);
            
            foreach (var ps in _particleSystems) {
                if (ps) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
            
            if (vfxAudioSource) {
                vfxAudioSource.Stop();
            }
        }

        private IEnumerator MonitorVfxRoutine() {
            yield return null; 
            
            bool isAlive = true;
            
            while (isAlive) {
                isAlive = false;
                foreach (var ps in _particleSystems) {
                    if (ps && ps.IsAlive(true)) {
                        isAlive = true;
                        break;
                    }
                }
                
                if (vfxAudioSource && vfxAudioSource.isPlaying) {
                    isAlive = true;
                }
                
                yield return new WaitForSeconds(0.2f); 
            }
            
            _returnToPoolCommand?.Invoke(this);
        }
    }
}