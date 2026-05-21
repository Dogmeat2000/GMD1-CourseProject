using UnityEngine;

namespace _01_Scripts.Turrets.AI
{
    /// <summary>
    /// AI Controller that operates the TurretMotor.
    /// </summary>
    [RequireComponent(typeof(TurretAISensor))]
    [RequireComponent(typeof(TurretMotor))] 
    public class TurretAIBrain : MonoBehaviour
    {
        [Header("Mechanical References")]
        [Tooltip("The part that rotates left/right.")]
        [SerializeField] private Transform turretBase;
        
        [Tooltip("The part that rotates up/down.")]
        [SerializeField] private Transform barrelBase;
        
        [Tooltip("The primary barrel that is fired from. Used to evaluate when turret has target in sights.")]
        [SerializeField] private Transform muzzleReference;

        [Header("Configuration")]
        [Tooltip("How fast the AI moves to track the target.")]
        [SerializeField] private float aimSpeed = 150f;
        
        [Tooltip("How close to the target [degrees] is acceptable to begin shooting.")]
        [SerializeField] private float fireToleranceAngle = 1f;
        
        [Tooltip("How often the AI may fire the main weapon")]
        [SerializeField] private float aiMainFireInterval = 10;
        
        [Tooltip("How often the AI may fire the auxiliary weapon")]
        [SerializeField] private float aiAuxFireInterval = 0.1f;

        private TurretAISensor _sensor;
        private TurretMotor _motor;
        
        private float _nextMainFireTime;
        private float _nextAuxFireTime;
        
        private Vector3 _homeLocalAimDirection;

        private void Awake() {
            _sensor = GetComponent<TurretAISensor>();
            _motor = GetComponent<TurretMotor>();
        }
        
        private void Start() {
            if (muzzleReference)
                _homeLocalAimDirection = transform.InverseTransformDirection(muzzleReference.forward);
        }

        private void Update() {
            if (!turretBase || !barrelBase || !muzzleReference) 
                return;

            bool hasTarget = _sensor.CurrentTarget != null && _sensor.CurrentTarget.IsTargetable;
            
            if (hasTarget)
                ExecuteCombatProtocol();
            else
                ExecuteRestingProtocol();
        }
        
        private void ExecuteCombatProtocol() {
            Vector3 targetPos = _sensor.CurrentTarget.TargetTransform.position;
            Vector3 currentAim = muzzleReference.forward;
            Vector3 dirToTarget = (targetPos - muzzleReference.position).normalized;
            Vector3 errorRotationAxis = Vector3.Cross(currentAim, dirToTarget);
            
            float yawError = Vector3.Dot(errorRotationAxis, turretBase.forward);
            float pitchError = Vector3.Dot(errorRotationAxis, barrelBase.up);
            
            float yawInput = yawError * aimSpeed * Time.deltaTime;
            float pitchInput = pitchError * aimSpeed * Time.deltaTime;

            _motor.RotateJoints(yawInput, pitchInput);
            
            float totalAngleError = Vector3.Angle(currentAim, dirToTarget);
            
            if (totalAngleError <= fireToleranceAngle) 
                EngageTarget();
            else 
                _motor.ReleaseTrigger(WeaponSlot.Both);
        }
        
        private void ExecuteRestingProtocol() {
            _motor.ReleaseTrigger(WeaponSlot.Both);
            
            Vector3 worldHomeAim = transform.TransformDirection(_homeLocalAimDirection);
            Vector3 targetPos = muzzleReference.position + (worldHomeAim * 200);

            Vector3 currentAim = muzzleReference.forward;
            Vector3 dirToTarget = (targetPos - muzzleReference.position).normalized;
            
            if (Vector3.Angle(currentAim, dirToTarget) < 0.1f) {
                _motor.RotateJoints(0f, 0f);
                return;
            }
            
            Vector3 errorRotationAxis = Vector3.Cross(currentAim, dirToTarget);
            
            float yawError = Vector3.Dot(errorRotationAxis, turretBase.forward);
            float pitchError = Vector3.Dot(errorRotationAxis, barrelBase.up);
            
            float restingSpeed = aimSpeed * 0.5f;
            float yawInput = yawError * restingSpeed * Time.deltaTime;
            float pitchInput = pitchError * restingSpeed * Time.deltaTime;

            _motor.RotateJoints(yawInput, pitchInput);
        }

        private void EngageTarget() {
            if (Time.time >= _nextMainFireTime) {
                _motor.PullTrigger(WeaponSlot.Main);
                _motor.ReleaseTrigger(WeaponSlot.Main);
                _nextMainFireTime = Time.time + aiMainFireInterval;
            }

            if (Time.time >= _nextAuxFireTime) {
                _motor.PullTrigger(WeaponSlot.Auxiliary);
                _motor.ReleaseTrigger(WeaponSlot.Auxiliary);
                _nextAuxFireTime = Time.time + aiAuxFireInterval;
            }
        }
    }
}
