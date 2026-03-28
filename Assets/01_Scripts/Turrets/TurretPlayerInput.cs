using UnityEngine;
using UnityEngine.InputSystem;

namespace _01_Scripts.Turrets
{
    [RequireComponent(typeof(TurretMotor))]
    public class TurretPlayerInput : MonoBehaviour
    {
        [Header("Input Action Assets")]
        public InputActionReference moveAction;
        public InputActionReference fireAction;

        [Header("Hardware Sensitivity Settings")]
        public float mouseSens = 10f;
        public float arcadeSens = 100f;

        private TurretMotor _motor;

        void Awake() => _motor = GetComponent<TurretMotor>();

        void OnEnable() {
            moveAction.action.Enable();
            fireAction.action.Enable();
            fireAction.action.performed += _ => _motor.PullTrigger();
        }

        void Update() {
            Vector2 input = moveAction.action.ReadValue<Vector2>();
            
            bool isMouse = moveAction.action.activeControl?.device is Pointer;
            float sens = isMouse ? mouseSens : arcadeSens;
            
            _motor.RotateJoints(input.x * sens * Time.deltaTime, input.y * sens * Time.deltaTime);
        }
    }
}