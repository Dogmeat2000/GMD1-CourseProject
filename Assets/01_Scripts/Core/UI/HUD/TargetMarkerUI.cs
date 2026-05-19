using UnityEngine;

namespace _01_Scripts.Core.UI.HUD
{
    /// <summary>
    /// UI component representing a hostile target on the HUD.
    /// </summary>
    [RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
    public class TargetMarkerUI : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;

        private void Awake() {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        /// <summary>
        /// Updates the visual position of the marker.
        /// </summary>
        public void UpdateMarker(Vector2 localAnchoredPosition,float scale, float alpha) {
            _rectTransform.anchoredPosition = localAnchoredPosition;
            _rectTransform.localScale = Vector3.one * scale;
            _canvasGroup.alpha = alpha;
        }
    }
}
