using UnityEngine;
using UnityEngine.Audio;

namespace _01_Scripts.Core.Settings
{
    // TODO Add description
    [CreateAssetMenu(fileName = "NewGlobalSettings", menuName = "Game/Settings/Global Settings", order = 0)]
    public class GlobalSettings : ScriptableObject
    {
        [field: Header("Global Audio Settings")]
        [field: Tooltip("The master channel for all background tracks.")]
        [field: SerializeField] public AudioMixerGroup MusicMixerGroup { get; private set; }
        
        [field: Tooltip("The master channel for all weapons, explosions, and gameplay SFX.")]
        [field: SerializeField] public AudioMixerGroup SfxMixerGroup { get; private set; }
        
        [field: Tooltip("The master channel for UI interactions.")]
        [field: SerializeField] public AudioMixerGroup UiMixerGroup { get; private set; }
        
        [field: Tooltip("The universal sound played when a joystick navigates to a button")]
        [field: SerializeField] public AudioClip DefaultButtonHighlightSound { get; private set; }
        
        [field: Tooltip("The universal sound played when a button is clicked/submitted")]
        [field: SerializeField] public AudioClip DefaultButtonSelectSound { get; private set; }

        
        [field: Header("Global Visuals")]
        [field: Tooltip("The universal scale multiplier for highlighted buttons")]
        [field: SerializeField] public float DefaultButtonHighlightScale { get; private set; } = 1.1f;
        
        [field: Tooltip("The universal speed at which buttons animate")]
        [field: SerializeField] public float DefaultButtonTransitionSpeed { get; private set; } = 15f;


        [field: Header("Player Settings")]
        [field: Tooltip("Name of the 'Move' player input action")]
        [field: SerializeField] public string MoveInputAction { get; private set; } = "Movement";

        [field: Tooltip("Name of the 'Fire Main Cannon' player input action")]
        [field: SerializeField] public string FireMainCannonInputAction { get; private set; } = "B - Main Cannon";
        
        [field: Tooltip("Name of the 'Fire Auxiliary Cannon' player input action")]
        [field: SerializeField] public string FireAuxCannonInputAction { get; private set; } = "X - Auxiliary Cannon";
        
        [field: Tooltip("Name of the 'Fire Special Ammo 1' player input action")]
        [field: SerializeField] public string FireSpecialAmmo1InputAction { get; private set; } = "Y - Special Ammo";
        
        [field: Tooltip("Name of the 'Pause Game' player input action")]
        [field: SerializeField] public string PauseGameInputAction { get; private set; } = "RightTrigger - Pause";
        
        [field: Header("Mouse Sensitivity Settings")]
        [field: SerializeField] public float MouseSens { get; set; } = 10f;
        
        [field: Header("Player 1 Input Control Scheme Name")]
        [field: SerializeField] public string P1InputControlSchemeName { get; set; } = "Player1_Scheme";
        
        [field: Header("Player 2 Input Control Scheme Name")]
        [field: SerializeField] public string P2InputControlSchemeName { get; set; } = "Player2_Scheme";
        
        [field: Header("Arcade Sensitivity Settings")]
        [field: Tooltip("The initial, slow sensitivity when the stick is quickly tapped (for precision aiming).")]
        [field: SerializeField] public float PrecisionSens { get; set; } = 25f;
        
        [field: Tooltip("The maximum sensitivity when the stick is held down (for rapid turning).")]
        [field: SerializeField] public float MaxSlewSens { get; set; } = 100f;
        
        [field: Tooltip("How many seconds the stick must be held to reach maximum turning speed.")]
        [field: SerializeField] public float TimeToMaxSpeed { get; set; } = 0.4f;
        
        [field: Tooltip("How long [s] the turret keeps sliding after releasing the joystick")]
        [field: SerializeField] public float SlideDuration { get; set; } = 0.15f;
        
        [field: Tooltip("How much the turret slows down when the reticle is over an enemy (e.g., 0.4 = 40% speed).")]
        [field: Range(0.1f, 1f)]
        [field: SerializeField] public float FrictionMultiplier { get; set; } = 0.4f;
        
        [field: Tooltip("Active Game Mode")]
        [field: SerializeField] public GameMode ActiveGameMode { get; set; } = GameMode.SinglePlayer;
    }
}
