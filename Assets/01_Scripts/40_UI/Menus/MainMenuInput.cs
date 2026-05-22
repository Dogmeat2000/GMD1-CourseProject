using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace _01_Scripts._40_UI.Menus
{
    /// <summary>
    /// Handles raw hardware input routing.
    /// </summary>
    public class MainMenuInput : MonoBehaviour
    {
        [Tooltip("Binding to Start Button")]
        [SerializeField] private InputActionReference readyActionRef;
        
        private InputAction _readyAction;
        
        public bool IsP1Ready { get; private set; }
        public bool IsP2Ready { get; private set; }
        public bool IsAwaitingPlayers { get; set; }
        public bool RequiresTwoPlayers { get; set; }
        
        public event Action<int, bool> OnPlayerReadyStatusChanged;
        
        private void OnEnable() {
            if (readyActionRef) {
                _readyAction = readyActionRef.action;
                _readyAction.Enable();
                _readyAction.performed += HandleReadyInput;
            }
        }

        private void OnDisable() {
            if (_readyAction != null)
                _readyAction.performed -= HandleReadyInput;
        }

        private void HandleReadyInput(InputAction.CallbackContext context) {
            if (!IsAwaitingPlayers) 
                return;

            var device = context.control.device;

            if (Gamepad.all.Count >= 2 && device == Gamepad.all[1]) {
                if (RequiresTwoPlayers) {
                    IsP2Ready = !IsP2Ready;
                    OnPlayerReadyStatusChanged?.Invoke(2, IsP2Ready);
                }
            } else if (device is Keyboard || device is Pointer || (Gamepad.all.Count >= 1 && device == Gamepad.all[0])) {
                IsP1Ready = !IsP1Ready;
                OnPlayerReadyStatusChanged?.Invoke(1, IsP1Ready);
            }
        }

        public void ResetInput() {
            IsP1Ready = false;
            IsP2Ready = false;
        }
    }
}
