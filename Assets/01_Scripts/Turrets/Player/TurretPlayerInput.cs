using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _01_Scripts.Turrets.Player
{
    [RequireComponent(typeof(TurretMotor))]
    public class TurretPlayerInput : MonoBehaviour
    {
        [Header("Input Action Assets")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference fireMainWeaponAction;
        [SerializeField] private InputActionReference fireAuxWeaponAction;
        [SerializeField] private InputActionReference fireSpecialWeapon1Action;

        [Header("Mouse Sensitivity Settings")]
        [SerializeField] private float mouseSens = 10f;
        
        [Header("Arcade Sensitivity Settings")]
        [Tooltip("The initial, slow sensitivity when the stick is quickly tapped (for precision aiming).")]
        [SerializeField] private float precisionSens = 25f;
        
        [Tooltip("The maximum sensitivity when the stick is held down (for rapid turning).")]
        [SerializeField] private float maxSlewSens = 100f;
        
        [Tooltip("How many seconds the stick must be held to reach maximum turning speed.")]
        [SerializeField] private float timeToMaxSpeed = 0.4f;
        
        [Tooltip("How long [s] the turret keeps sliding after releasing the joystick")]
        [SerializeField] private float slideDuration = 0.15f;
        
        [Tooltip("How much the turret slows down when the reticle is over an enemy (e.g., 0.4 = 40% speed).")]
        [Range(0.1f, 1f)]
        [SerializeField] private float frictionMultiplier = 0.4f;

        private TurretMotor _motor;
        private bool _isFireMainRequested = false;
        private bool _isFireSpecialWeapon1Requested = false;
        private bool _isReticleOnTarget = false;
        private bool _wasLastInputMouse = false;
        private float _currentHoldTime = 0f;
        private Vector2 _currentMovement;
        private Vector2 _movementVelocity;

        void Awake() => _motor = GetComponent<TurretMotor>();

        void OnEnable() {
            moveAction.action.Enable();
            
            fireMainWeaponAction.action.Enable();
            fireMainWeaponAction.action.performed += ExecuteFireMainCommand;
            
            fireSpecialWeapon1Action.action.Enable();
            fireSpecialWeapon1Action.action.performed += ExecuteFireSpecial1Command;
            
            fireAuxWeaponAction.action.Enable();
        }
        
        void OnDisable() {
            fireMainWeaponAction.action.performed -= ExecuteFireMainCommand;
            fireSpecialWeapon1Action.action.performed -= ExecuteFireSpecial1Command;
        }

        private void ExecuteFireMainCommand(InputAction.CallbackContext context) {
            _isFireMainRequested = true;
        }

        private void ExecuteFireSpecial1Command(InputAction.CallbackContext context) {
            _isFireSpecialWeapon1Requested = true;
        }
        
        public void SetTargetFriction(bool onTarget) {
            _isReticleOnTarget = onTarget;
        }

        void Update() {
            Vector2 rawInput = moveAction.action.ReadValue<Vector2>();
            
            if (moveAction.action.activeControl != null) {
                _wasLastInputMouse = moveAction.action.activeControl.device is Pointer;
            }
            
            float activeSens;

            if (_wasLastInputMouse) {
                activeSens = mouseSens;
                _currentMovement = rawInput * activeSens; 
                _movementVelocity = Vector2.zero; 
            } else {
                if (rawInput.sqrMagnitude > 0.01f) {
                    _currentHoldTime += Time.deltaTime;
                    
                    float timeRatio = Mathf.Clamp01(_currentHoldTime / timeToMaxSpeed);
                    float baseArcadeSens = Mathf.Lerp(precisionSens, maxSlewSens, timeRatio);
                    
                    activeSens = _isReticleOnTarget ? (baseArcadeSens * frictionMultiplier) : baseArcadeSens;

                    _currentMovement = rawInput * activeSens;
                    _movementVelocity = Vector2.zero; 
                } else {
                    _currentHoldTime = 0f; 
                    _currentMovement = Vector2.SmoothDamp(_currentMovement, Vector2.zero, ref _movementVelocity, slideDuration);
                }
            }
            
            _motor.RotateJoints(_currentMovement.x * Time.deltaTime, _currentMovement.y * Time.deltaTime);
            
            // Main Fire Mode:
            if (_isFireMainRequested) {
                if (EventSystem.current && !EventSystem.current.IsPointerOverGameObject()) {
                    _motor.PullTrigger(TurretMotor.WeaponSlot.Main);
                }
                _isFireMainRequested = false;
            } else if (!fireMainWeaponAction.action.IsPressed()) {
                _motor.ReleaseTrigger(TurretMotor.WeaponSlot.Main);
            }
            
            // Auxiliary Fire Mode:
            if (fireAuxWeaponAction.action.IsPressed()) {
                if (EventSystem.current && !EventSystem.current.IsPointerOverGameObject()) {
                    _motor.PullTrigger(TurretMotor.WeaponSlot.Auxiliary);
                }
            } else {
                _motor.ReleaseTrigger(TurretMotor.WeaponSlot.Auxiliary);
            }
            
            // Special Weapons 1 Fire Mode:
            if (_isFireSpecialWeapon1Requested) {
                if (EventSystem.current && !EventSystem.current.IsPointerOverGameObject()) {
                    _motor.PullTrigger(TurretMotor.WeaponSlot.Special1);
                }
                _isFireSpecialWeapon1Requested = false;
            } else {
                _motor.ReleaseTrigger(TurretMotor.WeaponSlot.Special1);
            }
        }
    }
}