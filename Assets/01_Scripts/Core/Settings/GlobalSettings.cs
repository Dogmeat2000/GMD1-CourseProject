using UnityEngine;

namespace _01_Scripts.Core.Settings
{
    [CreateAssetMenu(fileName = "NewGlobalSettings", menuName = "Game/Settings/Global Settings", order = 0)]
    public class GlobalSettings : ScriptableObject
    {
        [Header("Global UI Acoustics")]
        [Tooltip("The universal sound played when a joystick navigates to a button")]
        [field: SerializeField] 
        public AudioClip DefaultButtonHighlightSound { get; private set; }
        
        [Tooltip("The universal sound played when a button is clicked/submitted")]
        [field: SerializeField] 
        public AudioClip DefaultButtonSelectSound { get; private set; }

        [Header("Global UI Visuals")]
        [Tooltip("The universal scale multiplier for highlighted buttons")]
        [field: SerializeField] 
        public float DefaultButtonHighlightScale { get; private set; } = 1.1f;
        
        [Tooltip("The universal speed at which buttons animate")]
        [field: SerializeField] 
        public float DefaultButtonTransitionSpeed { get; private set; } = 15f;
    }
}
