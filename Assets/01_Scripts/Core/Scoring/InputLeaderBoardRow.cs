using System;
using _01_Scripts.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _01_Scripts.Core.Scoring
{
    public class InputLeaderBoardRow : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The TextMeshPro object where rank should be displayed.")]
        [SerializeField] private TextMeshProUGUI rankText;
        
        [Tooltip("The ArcadeNameInput prefab to use for input fields, when players make it to the leaderboard.")]
        [SerializeField] private ArcadeNameInput nameInputField;
        
        [Tooltip("The TextMeshPro object where the player score should be displayed.")]
        [SerializeField] private TextMeshProUGUI scoreText;
        
        [Tooltip("The Button object to use, to save players name to the leaderboard.")]
        [SerializeField] private Button saveButton;

        private Action<string> _onSaveCommand;

        public void Initialize(int rank, int score, string defaultName, Action<string> onSaveCallback) {
            rankText.text = $"{rank}.";
            scoreText.text = score.ToString("N0");
            _onSaveCommand = onSaveCallback;
            
            if (nameInputField)
                nameInputField.SetStartingName(defaultName);
            
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
