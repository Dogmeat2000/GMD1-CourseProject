using UnityEngine;
using TMPro;

public class TurretHUD : MonoBehaviour
{
    [Header("Targeting")]
    public Transform lowerMuzzleExit;
    public Transform upperMuzzleExit;
    public RectTransform lowerReticleUI;
    public RectTransform upperReticleUI;
    public Camera turretCamera;

    [Header("Readout")]
    public TextMeshProUGUI statusText;

    private void LateUpdate()
    {
        if (!turretCamera) return;

        // Calculate the 3D target point 750m ahead of the lowerMuzzle
        if (lowerMuzzleExit && lowerReticleUI)
        {
            Ray targetRay = new Ray(lowerMuzzleExit.position, lowerMuzzleExit.forward);
            Vector3 worldImpactPoint = Physics.Raycast(targetRay, out RaycastHit hit, 750f) ? hit.point : targetRay.GetPoint(750f);

            // Project the 3D point onto the 2D screen
            Vector3 screenPoint = turretCamera.WorldToScreenPoint(worldImpactPoint);
            
            // Update UI position if target is in front of camera lens
            if (screenPoint.z > 0)
            {
                lowerReticleUI.gameObject.SetActive(true);
                lowerReticleUI.position = screenPoint;
            }
            else
            {
                lowerReticleUI.gameObject.SetActive(false);
            }
        }

        if (upperMuzzleExit&& upperReticleUI)
        {
            Ray targetRay = new Ray(upperMuzzleExit.position, upperMuzzleExit.forward);
            Vector3 worldImpactPoint = Physics.Raycast(targetRay, out RaycastHit hit, 750f) ? hit.point : targetRay.GetPoint(750f);

            // Project the 3D point onto the 2D screen
            Vector3 screenPoint = turretCamera.WorldToScreenPoint(worldImpactPoint);
            
            // Update UI position if target is in front of camera lens
            if (screenPoint.z > 0)
            {
                upperReticleUI.gameObject.SetActive(true);
                upperReticleUI.position = screenPoint;
            }
            else
            {
                upperReticleUI.gameObject.SetActive(false);
            }
        }

    }
    
    // Call this from TurretPlayerInput to update the status text
    // TODO
    public void SetStatus(string message)
    {
        if (statusText != null) statusText.text = message;
    }
}