using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _01_Scripts.Core.UI
{
    public class MenuFocusController : MonoBehaviour
    {
        [Header("Navigation Configuration")]
        [Tooltip("The button that should be highlighted first when this menu opens.")]
        [SerializeField]
        private GameObject defaultSelectedButton;
        
        [Tooltip("How long to ignore input after the screen appears (in seconds).")]
        public float inputDelay = 1.0f;
        
        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        private void OnEnable() {
            if (defaultSelectedButton) {
                StartCoroutine(SetFocusNextFrame());
            }
        }

        private IEnumerator SetFocusNextFrame() {
            _canvasGroup.interactable = false;
            
            if(inputDelay > 0)
                yield return new WaitForEndOfFrame(); 
            else 
                yield return new WaitForSeconds(inputDelay);

            _canvasGroup.interactable = true;
            
            if (EventSystem.current) {
                EventSystem.current.SetSelectedGameObject(null);
                
                if (defaultSelectedButton.TryGetComponent<Selectable>(out var selectable)) {
                    selectable.Select();
                } else {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(defaultSelectedButton);
                }
            } else {
                Debug.LogWarning("MenuFocusController could not find an active EventSystem.");
            }
        }
    }
}
