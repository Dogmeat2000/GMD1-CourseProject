using UnityEngine;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Movement.Behaviors;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Settings;
using _01_Scripts.Core.Targeting;
using _01_Scripts.Core.Targeting.Strategies;

namespace _01_Scripts.Core.Movement
{
    // TODO: Add class descriptor
    [RequireComponent(typeof(Rigidbody))]
    public class KamikazeFlightBrain : MonoBehaviour, IEntityBrain
    {
        [Header("Flight Capabilities")]
        [Tooltip("The speed this entity breaches the water surface with")]
        [SerializeField] private float breachSpeed = 100f;
        
        [Tooltip("The speed this entity pursues player ships with")]
        [SerializeField] private float pursuitSpeed = 30f;
        
        [Tooltip("The speed this entity turns with")]
        [SerializeField] private float turnSpeed = 5f;
        
        [Tooltip("Which layer do allies of this entity belong to?")]
        [SerializeField] private LayerMask allyLayer;

        
        [Header("Behavior Tuning")]
        [Tooltip("How strongly the drone pushes toward the target")]
        [SerializeField] private float seekWeight = 1.0f;
        
        [Tooltip("How heavily the drone relies on zig-zag evasion")]
        [SerializeField] private float evadeWeight = 0.4f;
        
        [Tooltip("How strongly the drone repels away from allies")]
        [SerializeField] private float separateWeight = 0.6f;
        
        [Tooltip("The absolute minimum Y altitude before the drone pulls up.")]
        [SerializeField] private float hardDeckAltitude = 2.0f; 

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
        
        private enum FlightPhase { Asleep, Breaching, Pursuing } // TODO: Move to external Class (DRY VIOLATION)
        
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
            _separate = new SeparationBehavior(repelRadius, allyLayer);
            
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

        private void ExecuteBreachManeuver() { // TODO: Move to external class. Dry violation
            _rb.linearVelocity = Vector3.up * breachSpeed;
            
            Quaternion upwardRotation = Quaternion.LookRotation(Vector3.up, Vector3.back);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, upwardRotation, Time.fixedDeltaTime * turnSpeed);
            _rb.MoveRotation(targetRotation);

            if (transform.position.y >= _targetBreachAltitude)
                AcquireTarget();
        }

        private void AcquireTarget() { // TODO: Move to external class. Dry violation
            _assignedTarget = _battlefieldRadar.GetOptimalTarget(transform.position, Faction.Friendly, _targetingStrategy);

            if (_assignedTarget != null) {
                _currentPhase = FlightPhase.Pursuing;
            } else {
                _rb.linearVelocity = Vector3.zero;
            }
        }

        private void ExecuteSwarmPursuit() { // TODO: Potentially has some DRY violations
            if (_assignedTarget == null || !_assignedTarget.IsTargetable) {
                AcquireTarget();
                return;
            }

            Vector3 targetPos = _assignedTarget.TargetTransform.position;
            
            Vector3 seekForce = _seek.CalculateDirection(transform, targetPos);
            Vector3 evadeForce = _evade.CalculateDirection(transform, targetPos);
            Vector3 separateForce = _separate.CalculateDirection(transform, targetPos);
            Vector3 pullUpForceVector = Vector3.zero;
            
            if (transform.position.y < hardDeckAltitude) {
                float dangerSeverity = Mathf.Clamp01(1.0f - (transform.position.y / hardDeckAltitude));
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
                
                float singleStep = turnSpeed * Time.fixedDeltaTime;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, desiredDirection, singleStep, 0.0f);
                
                _rb.MoveRotation(Quaternion.LookRotation(newDirection, Vector3.up));
            }
            
            _rb.linearVelocity = transform.forward * pursuitSpeed;
            _rb.angularVelocity = Vector3.zero;
        }
    }
}