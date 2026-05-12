using UnityEngine;

namespace _01_Scripts.Core.Audio
{
    /// <summary>
    /// Maintains a single AudioListener at the geometric center of all active local players,
    /// ensuring balanced spatial audio during split-screen co-op.
    /// </summary>
    [RequireComponent(typeof(AudioListener))]
    public class SplitScreenAudioDirector : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Add all player controlled turrets here. The script will only track those currently enabled.")]
        [SerializeField] private Transform[] allPlayerTurrets;
        
        [Tooltip("Approximate time [s] it takes to reach the centroid.")]
        [SerializeField] private float positionSmoothTime = 0.3f;
        
        [Tooltip("How aggressively the audio listener rotates to match the players' average forward vector.")]
        [SerializeField] private float rotationSpeed = 10f;

        private Vector3 _currentVelocity;
        
        private void Update() {
            UpdateAcousticCentroid();
        }

        private void UpdateAcousticCentroid() {
            if (allPlayerTurrets == null || allPlayerTurrets.Length == 0) 
                return;

            Vector3 centerPoint = Vector3.zero;
            Vector3 averageForward = Vector3.zero;
            
            int activePilots = 0;
            
            foreach (Transform turret in allPlayerTurrets) {
                if (turret && turret.gameObject.activeInHierarchy) {
                    centerPoint += turret.position;
                    averageForward += turret.forward;
                    activePilots++;
                }
            }

            if (activePilots <= 0) 
                return;
            
            Vector3 targetPosition = centerPoint / activePilots;
            Vector3 targetForward = (averageForward / activePilots).normalized;
            
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, positionSmoothTime);
            
            if (targetForward != Vector3.zero) {
                Quaternion targetRotation = Quaternion.LookRotation(targetForward);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }
}