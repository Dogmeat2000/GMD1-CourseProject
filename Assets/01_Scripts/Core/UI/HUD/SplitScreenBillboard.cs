using UnityEngine;
using UnityEngine.Rendering;

namespace _01_Scripts.Core.UI.HUD
{
    // TODO Add description
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