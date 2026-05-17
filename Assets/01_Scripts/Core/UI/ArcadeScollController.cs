using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace _01_Scripts.Core.UI
{
    /// <summary>
    /// Allows arcade joysticks to scroll UI text seamlessly alongside PC mice.
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class ArcadeScollController : MonoBehaviour
    {
        [Header("Input Setup")]
        [Tooltip("The Input Action for moving (e.g., Joystick Up/Down)")]
        [SerializeField] 
        private InputActionReference scrollAction;

        [Header("Scroll Settings")]
        [Tooltip("How fast the joystick scrolls the text.")]
        [SerializeField] 
        private float scrollSpeed = 1.5f;

        private ScrollRect _scrollRect;

        private void Awake() {
            _scrollRect = GetComponent<ScrollRect>();
        }

        private void OnEnable() {
            if (scrollAction) {
                scrollAction.action.Enable();
            }
        }

        private void Update() {
            if (!scrollAction) 
                return;
            
            float scrollInput = scrollAction.action.ReadValue<Vector2>().y;
            
            if (Mathf.Abs(scrollInput) > 0.05f) {
                _scrollRect.verticalNormalizedPosition += scrollInput * scrollSpeed * Time.deltaTime;
                _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(_scrollRect.verticalNormalizedPosition);
            }
        }
    }
}
