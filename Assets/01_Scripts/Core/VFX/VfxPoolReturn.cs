using System;
using System.Collections;
using UnityEngine;
using _01_Scripts.Core.Interfaces;

namespace _01_Scripts.Core.VFX
{
    public class VfxPoolReturn : MonoBehaviour, IPoolable
    {
        private Action<IPoolable> _returnToPoolCommand;
        private ParticleSystem[] _particleSystems; 
        private Coroutine _monitorCoroutine;

        private void Awake() {
            _particleSystems = GetComponentsInChildren<ParticleSystem>();
        }

        public void Initialize(Action<IPoolable> returnAction) {
            _returnToPoolCommand = returnAction;
        }

        public void OnSpawned() {
            foreach (var ps in _particleSystems) {
                if (ps) ps.Play(true);
            }
            
            if (_monitorCoroutine != null) StopCoroutine(_monitorCoroutine);
            _monitorCoroutine = StartCoroutine(MonitorVfxRoutine());
        }

        public void OnDespawned() {
            if (_monitorCoroutine != null) StopCoroutine(_monitorCoroutine);
            
            foreach (var ps in _particleSystems) {
                if (ps) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
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
                
                yield return new WaitForSeconds(0.2f); 
            }
            
            _returnToPoolCommand?.Invoke(this);
        }
    }
}