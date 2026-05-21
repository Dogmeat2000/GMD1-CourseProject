using System.Collections.Generic;
using _01_Scripts.Core.Managers;
using _01_Scripts.Core.Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace _01_Scripts.Turrets.Player
{
    /// <summary>
    /// Handles Player Input and translates these into actions that are executed on the referenced TurretMotor.
    /// Dynamically configures the input based on which player (Player1 or Player2) this script is applied to,
    /// allowing for COOP using multiple GamePads/Controllers.
    /// </summary>
    [RequireComponent(typeof(TurretMotor))]
    public class TurretPlayerInput : MonoBehaviour
    {
        [Header("Player Configuration")]
        [Tooltip("0 for Gamepad 1 (Or mouse/keyboard), 1 for Gamepad 2")]
        [SerializeField] private int playerIndex = 0;
        
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

        private void Awake() {
            _playerInput = GetComponent<PlayerInput>();
            _motor = GetComponent<TurretMotor>();
            
            _moveAction = _playerInput.actions[GlobalManager.Instance.GlobalSettings.MoveInputAction];
            _fireMainAction = _playerInput.actions[GlobalManager.Instance.GlobalSettings.FireMainCannonInputAction];
            _fireAuxAction = _playerInput.actions[GlobalManager.Instance.GlobalSettings.FireAuxCannonInputAction];
            _fireSpecial1Action = _playerInput.actions[GlobalManager.Instance.GlobalSettings.FireSpecialAmmo1InputAction];
            _pauseAction = _playerInput.actions[GlobalManager.Instance.GlobalSettings.PauseGameInputAction];
        }

        private void Start() {
            string schemeName = (playerIndex == 0) ? GlobalManager.Instance.GlobalSettings.P1InputControlSchemeName : GlobalManager.Instance.GlobalSettings.P2InputControlSchemeName;
            List<InputDevice> assignedDevices = new List<InputDevice>();
            
            if (playerIndex == 0) {
                if (Gamepad.all.Count > 0) 
                    assignedDevices.Add(Gamepad.all[0]);
                
                if (Keyboard.current != null) 
                    assignedDevices.Add(Keyboard.current);
                
                if (Mouse.current != null) 
                    assignedDevices.Add(Mouse.current);

                _playerInput.SwitchCurrentControlScheme(schemeName, assignedDevices.ToArray());
            }  else {
                if (Gamepad.all.Count > 1) {
                    assignedDevices.Add(Gamepad.all[1]);
                    _playerInput.SwitchCurrentControlScheme(schemeName, assignedDevices.ToArray());
                } else {
                    Debug.LogWarning($"Player 2 offline: Gamepad 2 not detected.");
                    _playerInput.DeactivateInput();
                }
            }
        }

        private void OnEnable() {
            if (_fireMainAction != null)
                _fireMainAction.performed += ExecuteFireMainCommand;
            
            if(_fireSpecial1Action != null)
                _fireSpecial1Action.performed += ExecuteFireSpecial1Command;
            
            if (_pauseAction != null) 
                _pauseAction.performed += ExecutePauseCommand;
        }
        
        private void OnDisable() {
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
        
        private void ExecutePauseCommand(InputAction.CallbackContext context) {
            var gameState = ServiceLocator.Get<GameStateService>();
            
            if (gameState == null || gameState.CurrentState == GameState.GameOver) 
                return;

            if (gameState.CurrentState == GameState.Playing)
                gameState.PauseGame();
            else if (gameState.CurrentState == GameState.Paused)
                gameState.ResumeGame();
        }
        
        /// <summary>
        /// Applies a reduction in mouse/stick movement, when the player has a target in its sights.
        /// This makes it easier to track and shoot distant targets.
        /// </summary>
        /// <param name="onTarget"></param>
        public void SetTargetFriction(bool onTarget) {
            _isReticleOnTarget = onTarget;
        }

        private void Update() {
            HandleMoveTurret();
            
            // Semi-Automatic weapons:
            HandleFireWeapon(ref _isFireMainRequested, WeaponSlot.Main);
            HandleFireWeapon(ref _isFireSpecialWeapon1Requested, WeaponSlot.Special1);
            
            // Full-Automatic weapons:
            bool isFireAuxiliaryPressed = _fireAuxAction.IsPressed();
            HandleFireWeapon(ref isFireAuxiliaryPressed, WeaponSlot.Auxiliary);
        }

        private void HandleMoveTurret() {
            if (_moveAction.activeControl != null)
                _wasLastInputMouse = _moveAction.activeControl.device is Pointer;
            
            Vector2 rawInput = _moveAction.ReadValue<Vector2>();
            float activeSens;
            
            if (_wasLastInputMouse) {
                activeSens = GlobalManager.Instance.GlobalSettings.MouseSens;
                _currentMovement = rawInput * activeSens; 
                _movementVelocity = Vector2.zero; 
            } else {
                if (rawInput.sqrMagnitude > 0.01f) {
                    _currentHoldTime += Time.deltaTime;
                    
                    float timeRatio = Mathf.Clamp01(_currentHoldTime / GlobalManager.Instance.GlobalSettings.TimeToMaxSpeed);
                    float baseArcadeSens = Mathf.Lerp(GlobalManager.Instance.GlobalSettings.PrecisionSens, GlobalManager.Instance.GlobalSettings.MaxSlewSens, timeRatio);
                    
                    activeSens = _isReticleOnTarget ? (baseArcadeSens * GlobalManager.Instance.GlobalSettings.FrictionMultiplier) : baseArcadeSens;

                    _currentMovement = rawInput * activeSens;
                    _movementVelocity = Vector2.zero; 
                } else {
                    _currentHoldTime = 0f; 
                    _currentMovement = Vector2.SmoothDamp(_currentMovement, Vector2.zero, ref _movementVelocity, GlobalManager.Instance.GlobalSettings.SlideDuration);
                }
            }
            
            _motor.RotateJoints(_currentMovement.x * Time.deltaTime, _currentMovement.y * Time.deltaTime);
        }

        private void HandleFireWeapon(ref bool condition, WeaponSlot weaponSlot) {
            if (condition) {
                if (EventSystem.current && !EventSystem.current.IsPointerOverGameObject())
                    _motor.PullTrigger(weaponSlot);
                
                condition = false;
            } else {
                _motor.ReleaseTrigger(weaponSlot);
            }
        } 
    }
}