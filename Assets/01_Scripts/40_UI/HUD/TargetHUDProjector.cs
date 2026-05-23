using System.Collections.Generic;
using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._20_Features.Targeting;
using UnityEngine;

namespace _01_Scripts._40_UI.HUD
{
    /// <summary>
    /// Projects 3D world coordinates of hostile targets onto the players 2D Canvas.
    /// </summary>
    public class TargetHUDProjector : MonoBehaviour
    { 
        [Header("Setup")]
        [Tooltip("The root Canvas that houses this UI.")]
        [SerializeField] private Canvas parentCanvas;
        
        [Tooltip("The main player camera used for projection.")]
        [SerializeField] private Camera mainCamera;
        
        [Tooltip("The UI Prefab featuring the Sprite to project.")]
        [SerializeField] private TargetMarkerUI markerPrefab;
        
        [Tooltip("An empty RectTransform high in the Canvas hierarchy to keep markers behind other UI.")]
        [SerializeField] private RectTransform markerContainer;

        [Header("Configuration")]
        [Tooltip("Distance [m] where the marker appears at its maximum size.")]
        [SerializeField] private float closeDistance = 20f;
        
        [Tooltip("Distance [m] where the marker shrinks to its minimum size.")]
        [SerializeField] private float farDistance = 450f;
        
        [Tooltip("Maximum scale the HUD marker has near closeDistance.")]
        [SerializeField] private float maxScale = 1.3f;
        
        [Tooltip("Minimum scale the HUD marker has near farDistance.")]
        [SerializeField] private float minScale = 0.4f;
        
        [Tooltip("Alpha value when the target is at the closeDistance (0 to 1)")]
        [SerializeField, Range(0f, 1f)] private float maxAlpha = 1.0f;
        
        [Tooltip("Alpha value when the target is at the farDistance (0 to 1)")]
        [SerializeField, Range(0f, 1f)] private float minAlpha = 0.65f;

        private IActorTracker _radar;
        private readonly List<TargetMarkerUI> _markerPool = new List<TargetMarkerUI>();

        private void Start() {
            _radar = ServiceLocator.Get<IActorTracker>();
            if (!mainCamera) 
                mainCamera = Camera.main;
            
            if (!parentCanvas) 
                parentCanvas = GetComponentInParent<Canvas>();
        }

        private void LateUpdate() {
            if (_radar == null || !mainCamera|| !parentCanvas) 
                return;
            
            List<ITargetable> hostiles = _radar.GetRadarTargets(Faction.Hostile);
            int activeCount = 0;

            foreach (var target in hostiles) {
                if (target == null || !target.IsTargetable) 
                    continue;
                
                Vector3 screenPoint = mainCamera.WorldToScreenPoint(target.TargetTransform.position);
                
                if (screenPoint.z > 0) {
                    Camera uiCamera = parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : mainCamera;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        markerContainer, 
                        screenPoint, 
                        uiCamera, 
                        out Vector2 localUIPoint
                    );
                    
                    TargetMarkerUI marker = GetOrCreateMarker(activeCount);
                    marker.gameObject.SetActive(true);
                    
                    float depthRatio = Mathf.InverseLerp(closeDistance, farDistance, screenPoint.z);
                    float currentScale = Mathf.Lerp(maxScale, minScale, depthRatio);
                    float currentAlpha = Mathf.Lerp(maxAlpha, minAlpha, depthRatio);
                    
                    marker.UpdateMarker(localUIPoint, currentScale, currentAlpha);
                    activeCount++;
                }
            }
            
            for (int i = activeCount; i < _markerPool.Count; i++) {
                _markerPool[i].gameObject.SetActive(false);
            }
        }
        
        private TargetMarkerUI GetOrCreateMarker(int index) {
            if (index < _markerPool.Count)
                return _markerPool[index];

            TargetMarkerUI newMarker = Instantiate(markerPrefab, markerContainer);
            _markerPool.Add(newMarker);
            return newMarker;
        }
    }
}
