using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace _01_Scripts.Core.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class ArcadeNameInput : Selectable
    { 
        [Header("Arcade Input Specifications")]
        [Tooltip("The maximum number of letters allowed")]
        [SerializeField] 
        private int maxCharacters = 10;
        
        [Tooltip("The characters available to scroll through")]
        [SerializeField] 
        private string characterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.- ";
        
        [SerializeField] 
        private Color activeCharColor = Color.yellow;
        
        [SerializeField] 
        private Color normalCharColor = Color.white;

        private char[] _nameChars;
        private int _currentSlotIndex = 0;
        private TextMeshProUGUI _textDisplay;
        private bool _isEditing = false;

        public string text => new string(_nameChars).Trim();

        protected override void Awake() {
            base.Awake();
            
            _textDisplay = GetComponent<TextMeshProUGUI>();
            _nameChars = new char[maxCharacters];
            
            for(int i = 0; i < maxCharacters; i++) {
                _nameChars[i] = characterSet[0];
            }
            UpdateDisplay();
        }
        
        /// <summary>
        /// Pre-fills the input array with a starting string, converting to uppercase 
        /// and filtering out any characters not present in the allowed character set.
        /// </summary>
        public void SetStartingName(string defaultName) {
            if (string.IsNullOrEmpty(defaultName)) 
                return;
            
            string upperName = defaultName.ToUpper();
            char padChar = characterSet.IndexOf('-') >= 0 ? ' ' : characterSet[0];

            for (int i = 0; i < maxCharacters; i++) {
                if (i < upperName.Length) {
                    char c = upperName[i];
                    _nameChars[i] = characterSet.IndexOf(c) >= 0 ? c : padChar;
                } else {
                    _nameChars[i] = padChar;
                }
            }
            UpdateDisplay();
        }

        public override void OnSelect(BaseEventData eventData) {
            base.OnSelect(eventData);
            _isEditing = true;
            UpdateDisplay();
        }

        public override void OnDeselect(BaseEventData eventData) {
            base.OnDeselect(eventData);
            _isEditing = false;
            UpdateDisplay();
        }

        public override void OnMove(AxisEventData eventData) {
            if (!_isEditing || !interactable) {
                base.OnMove(eventData);
                return;
            }

            switch (eventData.moveDir) {
                case MoveDirection.Up:
                    ChangeCharacter(-1);
                    eventData.Use();
                    break;
                
                case MoveDirection.Down:
                    ChangeCharacter(1);
                    eventData.Use();
                    break;
                
                case MoveDirection.Left:
                    if (_currentSlotIndex > 0) {
                        _currentSlotIndex--;
                        UpdateDisplay();
                        eventData.Use();
                    } else {
                        base.OnMove(eventData);
                    }
                    break;
                
                case MoveDirection.Right:
                    if (_currentSlotIndex < maxCharacters - 1) {
                        _currentSlotIndex++;
                        UpdateDisplay();
                        eventData.Use();
                    } else {
                        base.OnMove(eventData); 
                    }
                    break;
            }
        }

        private void ChangeCharacter(int direction) {
            char currentChar = _nameChars[_currentSlotIndex];
            int charIndex = characterSet.IndexOf(currentChar);
            charIndex += direction;
            
            if (charIndex >= characterSet.Length) charIndex = 0;
            if (charIndex < 0) charIndex = characterSet.Length - 1;

            _nameChars[_currentSlotIndex] = characterSet[charIndex];
            UpdateDisplay();
        }

        private void UpdateDisplay() {
            if (!_textDisplay) 
                return;

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < maxCharacters; i++) {
                if (_isEditing && i == _currentSlotIndex && interactable) {
                    string colorHex = ColorUtility.ToHtmlStringRGB(activeCharColor);
                    sb.Append($"<color=#{colorHex}><u>{_nameChars[i]}</u></color>");
                } else {
                    string colorHex = ColorUtility.ToHtmlStringRGB(normalCharColor);
                    sb.Append($"<color=#{colorHex}>{_nameChars[i]}</color>");
                }
            }
            _textDisplay.text = sb.ToString();
        }
    }
}
