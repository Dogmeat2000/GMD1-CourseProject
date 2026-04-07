using UnityEngine;

namespace _01_Scripts.Environment
{
    public class OceanFollower : MonoBehaviour
    {
        [Tooltip("The player vessel or camera this ocean should follow.")]
        [SerializeField] private Transform target;
        
        [Tooltip("The absolute sea level (Y-axis) the ocean should stay at.")]
        [SerializeField] private float seaLevelY = 0f;

        private void LateUpdate() {
            if (!target) return;
            Vector3 targetPosition = target.position;
            transform.position = new Vector3(targetPosition.x, seaLevelY, targetPosition.z);
        }
    }
}
