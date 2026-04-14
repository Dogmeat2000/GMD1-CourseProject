using UnityEngine;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Movement.Behaviors;
using _01_Scripts.Core.Settings;
using _01_Scripts.Core.Targeting;
using _01_Scripts.Core.Targeting.Strategies;

namespace _01_Scripts.Core.Movement
{
    [RequireComponent(typeof(Rigidbody))]
    public class KamikazeFlightBrain : MonoBehaviour, IEntityBrain
    {
        private enum FlightPhase { Asleep, Breaching, Pursuing }

        [Header("Flight Capabilities")]
        [SerializeField] 
        private float breachSpeed = 80f;
        
        [SerializeField] 
        private float pursuitSpeed = 120f;
        
        [SerializeField] 
        private float turnSpeed = 5f;
        
        [SerializeField] 
        private LayerMask allyLayer;

        
        [Header("Behavior Tuning")]
        [Tooltip("How strongly the drone pushes toward the target")]
        [SerializeField] 
        private float seekWeight = 1.0f;
        
        [Tooltip("How heavily the drone relies on zig-zag evasion")]
        [SerializeField] 
        private float evadeWeight = 0.4f;
        
        [Tooltip("How strongly the drone repels away from allies")]
        [SerializeField] 
        private float separateWeight = 0.6f;
        
        
        [Header("Evasion Profile")]
        [Tooltip("The width of the zig-zag [m]")]
        [SerializeField] 
        private float evasionIntensity = 0.5f;
        
        [Tooltip("The frequency of the zig-zag")]
        [SerializeField] 
        private float evasionSpeed = 2.5f;
        
        
        [Header("Swarm Profile")]
        [Tooltip("How close [m] allies can get before repelling")]
        [SerializeField] 
        private float repelRadius = 15f;
        
        private FlightPhase _currentPhase = FlightPhase.Asleep;
        private Rigidbody _rb;
        private ITargetable _assignedTarget;
        
        private ITargetingStrategy _targetingStrategy;
        private SeekBehavior _seek;
        private EvasiveBehavior _evade;
        private SeparationBehavior _separate;

        private float _targetBreachAltitude;

        private void Awake() {
            _rb = GetComponent<Rigidbody>();
            _targetingStrategy = new WeightedRandomStrategy();
        }
        
        public void WakeUp() {
            LevelSettings settings = LevelManager.Instance.Settings;
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

        private void ExecuteBreachManeuver() {
            _rb.linearVelocity = Vector3.up * breachSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.up), Time.fixedDeltaTime * turnSpeed);

            if (transform.position.y >= _targetBreachAltitude) {
                AcquireTarget();
            }
        }

        private void AcquireTarget() {
            _assignedTarget = BattlefieldRadar.Instance.GetOptimalTarget(transform.position, Faction.Friendly, _targetingStrategy);

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
            
            Vector3 desiredDirection = (seekForce * seekWeight) + (evadeForce * evadeWeight) + (separateForce * separateWeight);
            desiredDirection.Normalize();
            
            Quaternion targetRotation = Quaternion.LookRotation(desiredDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed);
            _rb.linearVelocity = transform.forward * pursuitSpeed;
        }
    }
}