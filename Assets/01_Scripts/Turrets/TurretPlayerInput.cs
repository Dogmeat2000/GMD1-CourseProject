using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

namespace _01_Scripts.Turrets
{
    [RequireComponent(typeof(TurretMotor))]
    public class TurretPlayerInput : MonoBehaviour
    {
        [Header("Input Action Assets")]
        [SerializeField] private InputActionReference moveAction;
        [SerializeField] private InputActionReference fireAction;

        [Header("Hardware Sensitivity Settings")]
        [SerializeField] private float mouseSens = 10f;
        [SerializeField] private float arcadeSens = 100f;

        private TurretMotor _motor;
        private bool _isFireRequested = false;

        void Awake() => _motor = GetComponent<TurretMotor>();

        void OnEnable() 
        {
            moveAction.action.Enable();
            fireAction.action.Enable();
            fireAction.action.performed += ExecuteFireCommand;
        }
        
        void OnDisable() 
        {
            moveAction.action.Disable();
            fireAction.action.Disable();
            fireAction.action.performed -= ExecuteFireCommand;
        }

        private void ExecuteFireCommand(InputAction.CallbackContext context)
        {
            _isFireRequested = true;
        }

        void Update() 
        {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            
            bool isMouse = moveAction.action.activeControl?.device is Pointer;
            float sens = isMouse ? mouseSens : arcadeSens;
            
            _motor.RotateJoints(input.x * sens * Time.deltaTime, input.y * sens * Time.deltaTime);
            
            if (_isFireRequested)
            {
                if (EventSystem.current && !EventSystem.current.IsPointerOverGameObject())
                {
                    _motor.PullTrigger();
                }
                _isFireRequested = false;
            }
        }
    }
}