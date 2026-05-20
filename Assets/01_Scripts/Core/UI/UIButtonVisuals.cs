using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using _01_Scripts.Core.Managers;

namespace _01_Scripts.Core.UI
{
    // TODO Add description
    [RequireComponent(typeof(Selectable))]
    public class UIButtonVisuals : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, ISubmitHandler, IPointerClickHandler
    {
        [Header("Button Visuals Settings")]
        [Tooltip("Overrides the global scale multiplier. Set to 0 to use global default.")]
        [SerializeField] private float overrideTargetScale = 0f;
        
        [Tooltip("Overrides the global transition speed. Set to 0 to use global default.")]
        [SerializeField] private float overrideTransitionSpeed = 0f;
        
        
        [Header("Sounds (Leave empty to use Global Settings)")]
        [Tooltip("Optional: The AudioSource to play button sounds")]
        [SerializeField] private AudioSource uiAudioSource;
        
        [Tooltip("Optional: The sound to play when the button is highlighted")]
        [SerializeField] private AudioClip overrideHighlightSound;
        
        [Tooltip("Optional: The sound to play when the button is clicked/submitted")]
        [SerializeField] private AudioClip overrideSelectSound;

        private Vector3 _originalScale;
        private Coroutine _animationRoutine;
        
        private float ActiveTargetScale => overrideTargetScale > 0f ? overrideTargetScale : GlobalManager.Instance.GlobalSettings.DefaultButtonHighlightScale;
        private float ActiveTransitionSpeed => overrideTransitionSpeed > 0f ? overrideTransitionSpeed : GlobalManager.Instance.GlobalSettings.DefaultButtonTransitionSpeed;
        private AudioClip ActiveHighlightSound => overrideHighlightSound ? overrideHighlightSound : GlobalManager.Instance.GlobalSettings.DefaultButtonHighlightSound;
        private AudioClip ActiveSelectSound => overrideSelectSound ? overrideSelectSound : GlobalManager.Instance.GlobalSettings.DefaultButtonSelectSound;

        private void Awake() {
            _originalScale = transform.localScale;
        }

        // Triggered by Joystick / Keyboard Navigation
        // TODO Validate if this is still the proper approach, after the recent changes to input mapping!
        public void OnSelect(BaseEventData eventData) => EngageFocus();
        public void OnDeselect(BaseEventData eventData) => DisengageFocus();
        public void OnSubmit(BaseEventData eventData) => ExecuteClickFeedback();

        // Triggered by Mouse / Pointer Fallback
        // TODO Validate if this is still the proper approach, after the recent changes to input mapping!
        public void OnPointerEnter(PointerEventData eventData) => EngageFocus();
        public void OnPointerExit(PointerEventData eventData) => DisengageFocus();
        public void OnPointerClick(PointerEventData eventData) => ExecuteClickFeedback();

        private void EngageFocus() {
            if (ActiveHighlightSound) { 
                if (uiAudioSource)
                    uiAudioSource.PlayOneShot(ActiveHighlightSound);
                else if (GlobalManager.Instance)
                    GlobalManager.Instance.PlayPersistentUISound(ActiveHighlightSound);
            }
            
            if (_animationRoutine != null) 
                StopCoroutine(_animationRoutine);
            
            _animationRoutine = StartCoroutine(AnimateScale(_originalScale * ActiveTargetScale));
        }

        private void DisengageFocus() {
            if (_animationRoutine != null) 
                StopCoroutine(_animationRoutine);
            
            _animationRoutine = StartCoroutine(AnimateScale(_originalScale));
        }

        private IEnumerator AnimateScale(Vector3 target) {
            while (Vector3.Distance(transform.localScale, target) > 0.001f) {
                transform.localScale = Vector3.Lerp(transform.localScale, target, Time.unscaledDeltaTime * ActiveTransitionSpeed);
                yield return null;
            }
            transform.localScale = target;
        }
        
        private void ExecuteClickFeedback() {
            if (!ActiveSelectSound) 
                return;
            
            if (uiAudioSource)
                uiAudioSource.PlayOneShot(ActiveSelectSound);
            else if (GlobalManager.Instance)
                GlobalManager.Instance.PlayPersistentUISound(ActiveSelectSound);
        }
    }
}
