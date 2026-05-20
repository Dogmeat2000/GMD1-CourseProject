using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Movement.Behaviors;
using _01_Scripts.Core.Services;
using _01_Scripts.Core.Targeting;
using _01_Scripts.Core.Targeting.Strategies;
using UnityEngine;

namespace _01_Scripts.Core.Movement
{
    // TODO: Add class descriptor
    [RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(HealthManager))]
    public class RangedFlightBrain : MonoBehaviour, IEntityBrain
    { 
        [Header("Flight Capabilities")]
        [Tooltip("The speed this entity breaches the water surface with")]
        [SerializeField] private float breachSpeed = 150;
        
        [Tooltip("The speed this entity pursues player ships with")]
        [SerializeField] private float pursuitSpeed = 30;
        
        [Tooltip("The speed this entity turns with")]
        [SerializeField] private float turnSpeed = 10f;
        
        [Tooltip("Which layer do allies of this entity belong to?")]
        [SerializeField] private LayerMask allyLayer;

        [Header("Combat Settings")]
        [Tooltip("Distance [m] at which the entity stops moving and begins idling/firing.")]
        [SerializeField] private float engagementRange = 150f;
        
        [Tooltip("Time [seconds] between each shot.")]
        [SerializeField] private float fireCooldown = 5f;

        [Header("Behavior Tuning")]
        [Tooltip("How strongly the drone pushes toward the target")]
        [SerializeField] private float seekWeight = 1.0f;
        
        [Tooltip("How heavily the drone relies on zig-zag evasion")]
        [SerializeField] private float evadeWeight = 0.1f;
        
        [Tooltip("How strongly the drone repels away from allies")]
        [SerializeField] private float separateWeight = 0.6f;
        
        [Tooltip("The absolute minimum Y altitude before the drone pulls up.")]
        [SerializeField] private float hardDeckAltitude = 10.0f; 
        
        [Tooltip("How violently the drone pitches up to avoid the water.")]
        [SerializeField] private float pullUpForce = 15.0f;
        
        [Header("Evasion Profile")]
        [Tooltip("The width of the zig-zag [m]")]
        [SerializeField] private float evasionIntensity = 2f;
        
        [Tooltip("The frequency of the zig-zag")]
        [SerializeField] private float evasionSpeed = 2.5f;
        
        [Tooltip("How close [m] allies can get before repelling")]
        [SerializeField] private float repelRadius = 15f;

        [Header("Animation Configuration")]
        [Tooltip("Matches the Boolean parameter in the Animator Controller.")]
        [SerializeField] private string isMovingBool = "IsMoving";
        
        [Tooltip("Matches the Trigger parameter in the Animator Controller.")]
        [SerializeField] private string hitTrigger = "Hit";
        
        [Tooltip("Matches the Trigger parameter in the Animator Controller.")]
        [SerializeField] private string shootTrigger = "Shoot";

        private enum FlightPhase { Asleep, Breaching, Moving, Idling } // TODO: Move to external class. DRY violation.
        
        private FlightPhase _currentState = FlightPhase.Asleep;
        private Rigidbody _rb;
        private Animator _animator;
        private HealthManager _healthManager;
        
        private ITargetable _assignedTarget;
        private BattlefieldRadar _battlefieldRadar;
        private ITargetingStrategy _targetingStrategy;

        private SeekBehavior _seek;
        private EvasiveBehavior _evade;
        private SeparationBehavior _separate;
        
        private LevelManager _levelManager;
        private float _targetBreachAltitude;
        
        private IRangedWeapon _weapon;
        private float _nextFireTime;
        
        private int _isMovingHash;
        private int _hitHash;
        private int _shootHash;

        private void Awake() {
            _levelManager = ServiceLocator.Get<LevelManager>();
            _battlefieldRadar = ServiceLocator.Get<BattlefieldRadar>();
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _healthManager = GetComponent<HealthManager>();
            _weapon = GetComponentInChildren<IRangedWeapon>();
            
            _targetingStrategy = new WeightedRandomStrategy(); 

            _isMovingHash = Animator.StringToHash(isMovingBool);
            _hitHash = Animator.StringToHash(hitTrigger);
            _shootHash = Animator.StringToHash(shootTrigger);
        }

        private void OnEnable() {
            _healthManager.OnHealthChanged += HandleHit;
        }

        private void OnDisable() {
            _healthManager.OnHealthChanged -= HandleHit;
        }

        public void WakeUp() {
            _targetBreachAltitude = Random.Range(_levelManager.Settings.BioSwarmerBreachHeightMin, _levelManager.Settings.BioSwarmerBreachHeightMax);
            
            _seek = new SeekBehavior();
            _evade = new EvasiveBehavior(evasionIntensity, evasionSpeed);
            _separate = new SeparationBehavior(repelRadius, allyLayer);
            
            _assignedTarget = null;
            _currentState = FlightPhase.Breaching;
            _nextFireTime = Time.time + fireCooldown;
        }

        public void ShutDown() {
            _currentState = FlightPhase.Asleep;
            _assignedTarget = null;
        }

        private void FixedUpdate() {
            if (_currentState == FlightPhase.Asleep) 
                return;
            
            if (_currentState == FlightPhase.Breaching) {
                ExecuteBreachManeuver();
                
            } else {
                if (_assignedTarget == null || !_assignedTarget.IsTargetable) {
                    AcquireTarget();
                    
                } else {
                    float sqrDistance = (_assignedTarget.TargetTransform.position - transform.position).sqrMagnitude;
                    float sqrEngagement = engagementRange * engagementRange;

                    if (sqrDistance > sqrEngagement) {
                        _currentState = FlightPhase.Moving;
                        ExecuteSwarmPursuit();
                    } else {
                        _currentState = FlightPhase.Idling;
                        ExecuteIdling();
                    }
                }
            }
            
            bool isMoving = _currentState == FlightPhase.Moving || _currentState == FlightPhase.Breaching;
            _animator.SetBool(_isMovingHash, isMoving);
        }

        private void AcquireTarget() { // TODO: Move to external class. Dry violation
            _assignedTarget = _battlefieldRadar.GetOptimalTarget(transform.position, Faction.Friendly, _targetingStrategy);

            if (_assignedTarget != null) {
                _currentState = FlightPhase.Moving;
            } else {
                _currentState = FlightPhase.Idling;
                ExecuteIdling(); 
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

        private void ExecuteSwarmPursuit() { // TODO: Potentially has some DRY violations
            Vector3 targetPos = _assignedTarget.TargetTransform.position;
            
            Vector3 seekForce = _seek.CalculateDirection(transform, targetPos);
            Vector3 evadeForce = _evade.CalculateDirection(transform, targetPos);
            Vector3 separateForce = _separate.CalculateDirection(transform, targetPos);
            Vector3 pullUpForceVector = Vector3.zero;
            
            if (transform.position.y < hardDeckAltitude) {
                float dangerSeverity = Mathf.Clamp01(1.0f - (transform.position.y / hardDeckAltitude));
                pullUpForceVector = Vector3.up * (pullUpForce * dangerSeverity);
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

        private void ExecuteIdling() {
            if (_assignedTarget != null && _assignedTarget.IsTargetable) {
                Vector3 directionToTarget = (_assignedTarget.TargetTransform.position - transform.position).normalized;
                
                float singleStep = turnSpeed * Time.fixedDeltaTime;
                Vector3 newDirection = Vector3.RotateTowards(transform.forward, directionToTarget, singleStep, 0.0f);
                
                _rb.MoveRotation(Quaternion.LookRotation(newDirection, Vector3.up));
                
                if (Time.time >= _nextFireTime) {
                    _animator.SetTrigger(_shootHash);
                    _nextFireTime = Time.time + fireCooldown;
                }
            }
            
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }
        
        /// <summary>
        /// Triggered by an Animation Event on the "Shoot" Animation.
        /// </summary>
        public void ExecuteAnimationFireEvent() {
            if (_weapon != null && _assignedTarget != null && _assignedTarget.IsTargetable) {
                _weapon.Fire();
            }
        }

        private void HandleHit(int currentHealth, int maxHealth, GameObject shooter) {
            if (currentHealth > 0 && currentHealth < maxHealth) {
                _animator.SetTrigger(_hitHash);
            }
        }
    }
}
