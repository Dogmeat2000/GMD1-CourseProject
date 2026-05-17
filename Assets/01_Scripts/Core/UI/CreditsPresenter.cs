using TMPro;
using UnityEngine;

namespace _01_Scripts.Core.UI
{
    public class CreditsPresenter : MonoBehaviour
    {
        [Header("Data Source")]
        [Tooltip("Reference to the credits file")]
        [SerializeField] 
        private TextAsset creditsFile;

        [Header("UI Routing")]
        [Tooltip("The TextMeshPro element where the credits will be rendered.")]
        [SerializeField] 
        private TextMeshProUGUI creditsTextDisplay;
        
        private void OnEnable() {
            InjectCredits();
        }

        private void InjectCredits() {
            creditsTextDisplay.text = creditsFile.text;
        }
    }
}
