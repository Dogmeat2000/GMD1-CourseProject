using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Utilities;
using _01_Scripts.Turrets.Player;
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

        [Header("Aim Assist Settings")]
        [Tooltip("Link to the concrete player input handler")]
        [SerializeField] 
        private TurretPlayerInput playerInputHandler;
        
        [Tooltip("Which layers contain the enemies that trigger friction?")]
        [SerializeField] 
        private LayerMask enemyLayerMask;
        
        private LevelManager _levelManager;

        private void Awake() {
            _levelManager = ServiceLocator.Get<LevelManager>();
        }

        /// <summary>
        /// Programmatically initializes the HUD with references. Useful, when dynamically spawning the player turret prefab.
        /// </summary>
        public void Initialize() {
            
        }

        private void LateUpdate() {
            if (!turretCamera) 
                return;

            bool isPaintingTarget = false;
            
            isPaintingTarget |= UpdateReticlePosition(lowerMuzzleExit, lowerReticleUI);
            isPaintingTarget |= UpdateReticlePosition(upperMuzzleExit, upperReticleUI);
            
            if (playerInputHandler) {
                playerInputHandler.SetTargetFriction(isPaintingTarget);
            }
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
            
            if (hitSomething) {
                return enemyLayerMask.Contains(hit.collider.gameObject.layer);
            }
            
            return false;
        }
    }
}