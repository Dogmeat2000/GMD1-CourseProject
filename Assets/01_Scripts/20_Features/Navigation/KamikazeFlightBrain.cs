using _01_Scripts._10_Core.DependencyInjection;
using _01_Scripts._10_Core.Persistence;
using _01_Scripts._20_Features.Progression;
using _01_Scripts._20_Features.Targeting;
using _01_Scripts._30_Actors.Enemies;
using _01_Scripts.Core.Targeting;
using UnityEngine;

namespace _01_Scripts._20_Features.Navigation
{
    /// <summary>
    /// Primary AI brain that makes decisions for the Kamikaze type of attackers.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class KamikazeFlightBrain : MonoBehaviour, IEntityBrain
    {
        [Tooltip("The EnemyProfile that defines this entities behavior")] 
        [SerializeField] private EnemyProfile profile;

        
        [Header("Behavior Tuning")]
        [Tooltip("How strongly the drone pushes toward the target")]
        [SerializeField] private float seekWeight = 1.0f;
        
        [Tooltip("How heavily the drone relies on zig-zag evasion")]
        [SerializeField] private float evadeWeight = 0.4f;
        
        [Tooltip("How strongly the drone repels away from allies")]
        [SerializeField] private float separateWeight = 0.6f;

        [Tooltip("How violently the drone pitches up to avoid the water.")]
        [SerializeField] private float pullUpForce = 15.0f;
        
        
        [Header("Evasion Profile")]
        [Tooltip("The width of the zig-zag [m]")]
        [SerializeField] private float evasionIntensity = 0.5f;
        
        [Tooltip("The frequency of the zig-zag")]
        [SerializeField] private float evasionSpeed = 2.5f;
        
        
        [Header("Swarm Profile")]
        [Tooltip("How close [m] allies can get before repelling")]
        [SerializeField] private float repelRadius = 15f;
        
        private FlightPhase _currentPhase = FlightPhase.Asleep;
        private Rigidbody _rb;
        private ITargetable _assignedTarget;
        private LevelManager _levelManager;
        private BattlefieldRadar _battlefieldRadar;
        
        private ITargetingStrategy _targetingStrategy;
        private SeekBehavior _seek;
        private EvasiveBehavior _evade;
        private SeparationBehavior _separate;

        private float _targetBreachAltitude;
        
        private void Awake() {
            _levelManager = ServiceLocator.Get<LevelManager>();
            _battlefieldRadar = ServiceLocator.Get<BattlefieldRadar>();
            _rb = GetComponent<Rigidbody>();
            _targetingStrategy = new WeightedRandomStrategy();
        }
        
        public void WakeUp() {
            LevelSettings settings = _levelManager.Settings;
            _targetBreachAltitude = Random.Range(settings.KamikazeBreachHeightMin, settings.KamikazeBreachHeightMax);
            
            _seek = new SeekBehavior();
            _evade = new EvasiveBehavior(evasionIntensity, evasionSpeed);
            _separate = new SeparationBehavior(repelRadius, profile.allyLayer);
            
            _assignedTarget = null;
            _currentPhase = FlightPhase.Breaching;
        }

        public void ShutDown() {
            _currentPhase = FlightPhase.Asleep;
            _assignedTarget = null;
        }

        private void FixedUpdate() {
            if (_currentPhase == FlightPhase.Asleep) 
                return;

            if (_currentPhase == FlightPhase.Breaching) {
                ExecuteBreachManeuver();
            } else if (_currentPhase == FlightPhase.Pursuing) {
                ExecuteSwarmPursuit();
            }
        }

        private void ExecuteBreachManeuver() {
            _rb.linearVelocity = Vector3.up * profile.breachSpeed;
            
            Quaternion upwardRotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, upwardRotation, Time.fixedDeltaTime * profile.turnSpeed);
            _rb.MoveRotation(targetRotation);

            if (transform.position.y >= _targetBreachAltitude)
                AcquireTarget();
        }

        private void AcquireTarget() {
            _assignedTarget = _battlefieldRadar.GetOptimalTarget(transform.position, Faction.Friendly, _targetingStrategy);

            if (_assignedTarget != null) {
                _currentPhase = FlightPhase.Pursuing;
            } else {
                _rb.linearVelocity = Vector3.zero;
            }
        }

        private void ExecuteSwarmPursuit() {
            if (_assignedTarget == null || !_assignedTarget.IsTargetable) {
                AcquireTarget();
                return;
            }

            Vector3 targetPos = _assignedTarget.TargetTransform.position;
            
            Vector3 seekForce = _seek.CalculateDirection(transform, targetPos);
            Vector3 evadeForce = _evade.CalculateDirection(transform, targetPos);
            Vector3 separateForce = _separate.CalculateDirection(transform, targetPos);
            Vector3 pullUpForceVector = Vector3.zero;
            
            if (transform.position.y < profile.hardDeckAltitude) {
                float dangerSeverity = Mathf.Clamp01(1.0f - (transform.position.y / profile.hardDeckAltitude));
                pullUpForceVector = Vector3.up * (pullUpForce * dangerSeverity);
                
                if (_rb.linearVelocity.y < 0) {
                    Vector3 flattenedVelocity = _rb.linearVelocity;
                    flattenedVelocity.y *= 0.8f; 
                    _rb.linearVelocity = flattenedVelocity;
                }
            }
            
            Vector3 desiredDirection = (seekForce * seekWeight) + (evadeForce * evadeWeight) + (separateForce * separateWeight) + pullUpForceVector;
            
            if (desiredDirection != Vector3.zero) {
                desiredDirection.Normalize();
                
                float singleStep = profile.turnSpeed * Time.fixedDeltaTime;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, desiredDirection, singleStep, 0.0f);
                
                _rb.MoveRotation(Quaternion.LookRotation(newDirection, Vector3.up));
            }
            
            _rb.linearVelocity = transform.forward * profile.pursuitSpeed;
            _rb.angularVelocity = Vector3.zero;
        }
    }
}