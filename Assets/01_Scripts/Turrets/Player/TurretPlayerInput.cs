using _01_Scripts.Core.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _01_Scripts.Turrets.Player
{
    [RequireComponent(typeof(TurretMotor))]
    public class TurretPlayerInput : MonoBehaviour
    {
        [Header("Input Action Assets")]
        [Tooltip("0 for Gamepad 1, 1 for Gamepad 2")]
        [SerializeField] private int playerIndex = 0;

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
        private PlayerInput _playerInput;
        
        private InputAction _moveAction;
        private InputAction _fireMainAction;
        private InputAction _fireAuxAction;
        private InputAction _fireSpecial1Action;
        private InputAction _pauseAction;
        
        private bool _isFireMainRequested = false;
        private bool _isFireSpecialWeapon1Requested = false;
        private bool _isReticleOnTarget = false;
        private bool _wasLastInputMouse = false;
        private float _currentHoldTime = 0f;
        private Vector2 _currentMovement;
        private Vector2 _movementVelocity;

        void Awake() {
            _playerInput = GetComponent<PlayerInput>();
            _motor = GetComponent<TurretMotor>();
            
            _moveAction = _playerInput.actions["Movement"];
            _fireMainAction = _playerInput.actions["B - Main Cannon"];
            _fireAuxAction = _playerInput.actions["X - Auxiliary Cannon"];
            _fireSpecial1Action = _playerInput.actions["Y - Special Ammo"];
            _pauseAction = _playerInput.actions["RightTrigger - Pause"];
        }

        void Start() {
            if (Gamepad.all.Count > playerIndex) {
                _playerInput.SwitchCurrentControlScheme(Gamepad.all[playerIndex]);
            } else {
                Debug.LogError($"Gamepad {playerIndex} not found!");
            }
        }

        void OnEnable() {
            if (_fireMainAction != null)
                _fireMainAction.performed += ExecuteFireMainCommand;
            
            if(_fireSpecial1Action != null)
                _fireSpecial1Action.performed += ExecuteFireSpecial1Command;
            
            if (_pauseAction != null) 
                _pauseAction.performed += ExecutePauseCommand;
        }
        
        void OnDisable() {
            if (_fireMainAction != null)
                _fireMainAction.performed -= ExecuteFireMainCommand;
            
            if(_fireSpecial1Action != null)
                _fireSpecial1Action.performed -= ExecuteFireSpecial1Command;
            
            if (_pauseAction != null) 
                _pauseAction.performed -= ExecutePauseCommand;
        }

        private void ExecuteFireMainCommand(InputAction.CallbackContext context) {
            _isFireMainRequested = true;
        }

        private void ExecuteFireSpecial1Command(InputAction.CallbackContext context) {
            _isFireSpecialWeapon1Requested = true;
        }
        
        private void ExecutePauseCommand(InputAction.CallbackContext context) 
        {
            var gameState = ServiceLocator.Get<GameStateService>();
            if (gameState == null || gameState.CurrentState == GameState.GameOver) return;

            if (gameState.CurrentState == GameState.Playing) {
                gameState.PauseGame();
            } 
            else if (gameState.CurrentState == GameState.Paused) {
                gameState.ResumeGame();
            }
        }
        
        public void SetTargetFriction(bool onTarget) {
            _isReticleOnTarget = onTarget;
        }

        void Update() {
            
            Vector2 rawInput = _moveAction.ReadValue<Vector2>();
            
            if (_moveAction.activeControl != null) {
                _wasLastInputMouse = _moveAction.activeControl.device is Pointer;
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
            } else if (!_fireMainAction.IsPressed()) {
                _motor.ReleaseTrigger(TurretMotor.WeaponSlot.Main);
            }
            
            // Auxiliary Fire Mode:
            if (_fireAuxAction.IsPressed()) {
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