using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._10_Core.Utilities;
using _01_Scripts._20_Features.Progression;
using _01_Scripts._20_Features.Weapons;
using UnityEngine;

namespace _01_Scripts._40_UI.HUD
{
    /// <summary>
    /// Presentor class responsible for displaying the Player Crosshairs on a Player Turret.
    /// The basic principle is to shoot a Raycast from a provided Muzzle a maximum distance out, see where it impacts, and project that position onto the screen, displaying a crosshair.
    /// This allows for an accurate estimation of where the provided muzzle or barrel would impact.
    /// </summary>
    public class TurretHUD : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The Transform belonging to the primary firing point (muzzle) of the primary weapon")]
        [SerializeField] private Transform lowerMuzzleExit;
        
        [Tooltip("The Crosshair to project onto the camera.")]
        [SerializeField] private RectTransform lowerReticleUI;
        
        [Tooltip("The Crosshair to project onto the camera.")]
        [SerializeField] private Camera turretCamera;
        
        [Tooltip("What can the turret aim at? (Select Default, Water, Enemy, etc.)")]
        [SerializeField] private LayerMask targetingMask = ~0; // ~0 means 'Everything'

        [Header("Aim Assist Settings")]
        [Tooltip("Link to the concrete player input handler")]
        [SerializeField] private TurretPlayerInput playerInputHandler;
        
        [Tooltip("Which layers contain the enemies that trigger friction?")]
        [SerializeField] private LayerMask enemyLayerMask;
        
        private LevelManager _levelManager;

        private void Awake() {
            _levelManager = ServiceLocator.Get<LevelManager>();
        }

        private void LateUpdate() {
            if (!turretCamera) 
                return;

            bool isPaintingTarget = false;
            
            isPaintingTarget |= UpdateReticlePosition(lowerMuzzleExit, lowerReticleUI);
            
            if (playerInputHandler)
                playerInputHandler.SetTargetFriction(isPaintingTarget);
        }
    
        /// <summary>
        /// Updates the UI reticle position and returns true if the targeting ray struck an enemy layer.
        /// </summary>
        private bool UpdateReticlePosition(Transform muzzle, RectTransform reticle) {
            if (!muzzle || !reticle) 
                return false;

            Ray targetRay = new Ray(muzzle.position, muzzle.forward);
            
            bool hitSomething = Physics.Raycast(targetRay, out RaycastHit hit, _levelManager.Settings.MaxTargetingDistance, targetingMask);
            Vector3 worldImpactPoint = hitSomething 
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
            
            if (hitSomething)
                return enemyLayerMask.Contains(hit.collider.gameObject.layer);
            
            return false;
        }
    }
}