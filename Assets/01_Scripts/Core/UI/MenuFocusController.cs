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
        private GameObject firstSelectedButton;

        private void OnEnable() {
            if (firstSelectedButton) {
                StartCoroutine(SetFocusNextFrame());
            }
        }

        private IEnumerator SetFocusNextFrame() {
            yield return new WaitForEndOfFrame(); 

            if (EventSystem.current) {
                EventSystem.current.SetSelectedGameObject(null);
                
                if (firstSelectedButton.TryGetComponent<Selectable>(out var selectable)) {
                    selectable.Select();
                } else {
                    EventSystem.current.SetSelectedGameObject(firstSelectedButton);
                }
            } else {
                Debug.LogWarning("MenuFocusController could not find an active EventSystem.");
            }
        }
    }
}
