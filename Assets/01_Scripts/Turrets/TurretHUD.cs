using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using UnityEngine;

namespace _01_Scripts.Turrets
{
    public class TurretHUD : MonoBehaviour
    {
        [Header("Targeting")]
        [SerializeField] 
        private Transform lowerMuzzleExit;
        
        [SerializeField] 
        private Transform upperMuzzleExit;
        
        [SerializeField] 
        private RectTransform lowerReticleUI;
        
        [SerializeField] 
        private RectTransform upperReticleUI;
        
        [SerializeField] 
        private Camera turretCamera;
        
        [Tooltip("What can the turret aim at? (Select Default, Water, Enemy, etc.)")]
        [SerializeField] 
        private LayerMask targetingMask = ~0; // ~0 means 'Everything'

        private LevelManager _levelManager;

        private void Awake() {
            _levelManager = ServiceLocator.Get<LevelManager>();
        }

        private void LateUpdate() {
            if (!turretCamera) return;

            UpdateReticlePosition(lowerMuzzleExit, lowerReticleUI);
            UpdateReticlePosition(upperMuzzleExit, upperReticleUI);
        }
    
        private void UpdateReticlePosition(Transform muzzle, RectTransform reticle) {
            if (!muzzle || !reticle) return;

            Ray targetRay = new Ray(muzzle.position, muzzle.forward);
            
            Vector3 worldImpactPoint = Physics.Raycast(targetRay, out RaycastHit hit, _levelManager.Settings.MaxTargetingDistance, targetingMask) 
                ? hit.point 
                : targetRay.GetPoint(_levelManager.Settings.MaxTargetingDistance);

            Vector3 screenPoint = turretCamera.WorldToScreenPoint(worldImpactPoint);
            
            // z > 0 means the target is in front of the camera
            bool isTargetVisible = screenPoint.z > 0;
        
            reticle.gameObject.SetActive(isTargetVisible);
            if (isTargetVisible) {
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    (RectTransform) reticle.parent,
                    screenPoint,
                    turretCamera,
                    out Vector2 localPoint);
                
                reticle.localPosition = localPoint;
            }
        }
    }
}