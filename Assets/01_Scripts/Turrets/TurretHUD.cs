using TMPro;
using UnityEngine;

namespace _01_Scripts.Turrets
{
    public class TurretHUD : MonoBehaviour
    {
        [Header("Targeting")]
        [SerializeField] private Transform lowerMuzzleExit;
        [SerializeField] private Transform upperMuzzleExit;
        [SerializeField] private RectTransform lowerReticleUI;
        [SerializeField] private RectTransform upperReticleUI;
        [SerializeField] private Camera turretCamera;

        [Header("Readout")]
        [SerializeField] private TextMeshProUGUI statusText;

        private void LateUpdate()
        {
            if (!turretCamera) return;

            // Calculate the 3D target point 750m ahead of the muzzles
            UpdateReticlePosition(lowerMuzzleExit, lowerReticleUI);
            UpdateReticlePosition(upperMuzzleExit, upperReticleUI);
        }
    
        private void UpdateReticlePosition(Transform muzzle, RectTransform reticle)
        {
            if (!muzzle || !reticle) return;

            Ray targetRay = new Ray(muzzle.position, muzzle.forward);
            Vector3 worldImpactPoint = Physics.Raycast(targetRay, out RaycastHit hit, 750f) ? hit.point : targetRay.GetPoint(750f);

            Vector3 screenPoint = turretCamera.WorldToScreenPoint(worldImpactPoint);
            
            // z > 0 means the target is in front of the camera
            bool isTargetVisible = screenPoint.z > 0;
        
            reticle.gameObject.SetActive(isTargetVisible);
            if (isTargetVisible)
            {
                reticle.position = screenPoint;
            }
        }
    
    
        // Call this from TurretPlayerInput to update the status text
        // TODO
        public void SetStatus(string message)
        {
            if (statusText) statusText.text = message;
        }
    }
}