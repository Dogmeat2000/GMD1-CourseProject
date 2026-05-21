using UnityEngine;
using UnityEngine.Rendering;

namespace _01_Scripts.Core.UI.HUD
{
    /// <summary>
    /// Attach to billboards that must always face the player, even during split screen multiplayer gameplay.
    /// Normally billboards auto-rotate to face a player - but with split screen you have 2 players, so which player does the billboards (such as FloatingHealthBar) face?
    /// This script ensure these billboards face both, by adjusting their orientation during rendering to each camera in the scene.
    /// </summary>
    public class SplitScreenBillboard : MonoBehaviour
    {
        private void OnEnable() {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
        }

        private void OnDisable() {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam) {
            if (cam.cameraType != CameraType.Game) 
                return;
            
            Vector3 directionAwayFromCamera = transform.position - cam.transform.position;
                
            if (directionAwayFromCamera.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(directionAwayFromCamera, cam.transform.up);
        }
    }
}