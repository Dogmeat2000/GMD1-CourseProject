using System;
using _01_Scripts.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Core.Scoring
{
    public class InputLeaderBoardRow : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI rankText;
        
        [SerializeField] 
        private ArcadeNameInput nameInputField;
        
        [SerializeField] 
        private TextMeshProUGUI scoreText;
        
        [SerializeField] 
        private Button saveButton;

        private Action<string> _onSaveCommand;

        public void Initialize(int rank, int score, Action<string> onSaveCallback) {
            rankText.text = $"{rank}.";
            scoreText.text = score.ToString("N0");
            _onSaveCommand = onSaveCallback;
            
            saveButton.onClick.RemoveAllListeners(); 
            saveButton.onClick.AddListener(ExecuteSave);
        }

        private void ExecuteSave() {
            string enteredName = nameInputField.text.Trim();
            
            if (string.IsNullOrWhiteSpace(enteredName)) 
                return; 
            
            _onSaveCommand?.Invoke(enteredName);
            
            nameInputField.interactable = false;
            saveButton.interactable = false;
        }
    }
}
