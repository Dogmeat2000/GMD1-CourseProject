using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts._40_UI.Menus
{
    /// <summary>
    /// Used for Credits. Ensures loaded credits file auto-scrolls when viewed.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class AutoScrollController : MonoBehaviour
    { 
        [Header("Scroll Settings")]
        [Tooltip("Scroll speed in pixels per second. Keeps speed consistent regardless of text length.")]
        [SerializeField] private float scrollSpeed = 45f;
        
        [Tooltip("Time [s] to wait before the credits begin rolling.")]
        [SerializeField] private float startDelay = 2.0f;
        
        private ScrollRect _scrollRect;
        private RectTransform _contentRect;
        private Coroutine _scrollRoutine;
        private WaitForSeconds _startDelay;

        private void Awake() {
            _scrollRect = GetComponent<ScrollRect>();
            _contentRect = _scrollRect.content;
            
            _scrollRect.horizontal = false;
            _scrollRect.vertical = false;
            _scrollRect.scrollSensitivity = 0f;

            _startDelay = new WaitForSeconds(startDelay);
        }

        private void OnEnable() {
            if (_scrollRoutine != null)
                StopCoroutine(_scrollRoutine);
            _scrollRoutine = StartCoroutine(ScrollSequenceRoutine());
        }
        
        private void OnDisable() {
            if (_scrollRoutine != null)
                StopCoroutine(_scrollRoutine);
        }

        private IEnumerator ScrollSequenceRoutine() {
            yield return null;
            _scrollRect.verticalNormalizedPosition = 1f;
            
            if (startDelay > 0)
                yield return _startDelay;
            
            while (_scrollRect.verticalNormalizedPosition > 0.001f) {
                if (_contentRect)
                    _contentRect.anchoredPosition += Vector2.up * (scrollSpeed * Time.deltaTime);
                yield return null;
            }
        }
    }
}
