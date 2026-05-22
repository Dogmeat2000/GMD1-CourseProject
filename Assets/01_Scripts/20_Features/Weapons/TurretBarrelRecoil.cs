using System.Collections;
using UnityEngine;

namespace _01_Scripts._20_Features.Weapons
{
    /// <summary>
    /// Provides access to Recoil movement, for the GameObject this script is attached to.
    /// Used for applying recoil upon firing the ranges turret weapons.
    /// </summary>
    public class TurretBarrelRecoil : MonoBehaviour
    {
        [Header("Recoil Settings")]
        [Tooltip("How far back the barrel slides on the local Z axis.")]
        [SerializeField] private float recoilDistance = 0.5f;
    
        [Tooltip("How fast the barrel snaps backward.")]
        [SerializeField] private float snapBackSpeed = 50f;
    
        [Tooltip("How smoothly the barrel returns to its resting position.")]
        [SerializeField] private float recoverySpeed = 10f;

        private Vector3 _originalLocalPosition;
        private Coroutine _recoilRoutine;

        private void Awake() {
            _originalLocalPosition = transform.localPosition;
        }
        
        /// <summary>
        /// Triggers the recoil effect.
        /// </summary>
        public void TriggerRecoil() {
            if (_recoilRoutine != null)
                StopCoroutine(_recoilRoutine);
                
            _recoilRoutine = StartCoroutine(RecoilRoutine());
        }

        private IEnumerator RecoilRoutine() {
            Vector3 localBackward = transform.localRotation * Vector3.back;
            Vector3 targetRecoilPosition = _originalLocalPosition + (localBackward * recoilDistance);

            while (Vector3.Distance(transform.localPosition, targetRecoilPosition) > 0.01f) {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetRecoilPosition, snapBackSpeed * Time.deltaTime);
                yield return null;
            }

            while (Vector3.Distance(transform.localPosition, _originalLocalPosition) > 0.001f) {
                transform.localPosition = Vector3.Lerp(transform.localPosition, _originalLocalPosition, recoverySpeed * Time.deltaTime);
                yield return null;
            }
            
            transform.localPosition = _originalLocalPosition;
        }
    }
}
