using _01_Scripts.Core.Managers;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace _01_Scripts.Core.Settings
{
    /// <summary>
    /// Exposes methods to retrieve and modify settings that are exposed to the player.
    /// </summary>
    public class SettingsController : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("The Toggle GameObject that toggles all audio on/off")]
        [SerializeField] private Toggle toggleMasterAudio;
        
        [Tooltip("The Slider GameObject that adjusts the Master Audio Level")]
        [SerializeField] private Slider masterAudioSlider;
        
        [Tooltip("The Toggle GameObject that toggles music audio on/off")]
        [SerializeField] private Toggle toggleMusicAudio;
        
        [Tooltip("The Slider GameObject that adjusts the Music Audio Level")]
        [SerializeField] private Slider musicAudioSlider;
        
        [Tooltip("The Toggle GameObject that toggles SFX audio on/off")]
        [SerializeField] private Toggle toggleSfxAudio;
        
        [Tooltip("The Slider GameObject that adjusts the SFX Audio Level")]
        [SerializeField] private Slider sfxAudioSlider;
        
        [Tooltip("The Toggle GameObject that toggles UI audio on/off")]
        [SerializeField] private Toggle toggleUiAudio;
        
        [Tooltip("The Slider GameObject that adjusts the UI Audio Level")]
        [SerializeField] private Slider uiAudioSlider;
        
        [Tooltip("The Slider GameObject that adjusts the Mouse Sensitivity")]
        [SerializeField] private Slider mouseSensSlider;
        
        [Tooltip("The Slider GameObject that adjusts the GamePad Precision Sensitivity")]
        [SerializeField] private Slider gamePadPrecisionSensSlider;
        
        [Tooltip("The Slider GameObject that adjusts the GamePad Maximum Sensitivity")]
        [SerializeField] private Slider gamePadMaxSensSlider;

        private const float MinAudioVolume = -80f;
        private const float MaxAudioVolume = 0f;
        
        private void Start() {
            // Base audio settings:
            SetMusicVolume(-10f);
            SetMasterVolume(MaxAudioVolume);
            SetSfxAudioVolume(-5);
            SetUIAudioVolume(-30);
            
            // Sliders
            masterAudioSlider.value = GlobalManager.Instance.GlobalSettings.GetMasterVolume();
            musicAudioSlider.value = GlobalManager.Instance.GlobalSettings.GetMusicVolume();
            sfxAudioSlider.value = GlobalManager.Instance.GlobalSettings.GetSfxVolume();
            uiAudioSlider.value = GlobalManager.Instance.GlobalSettings.GetUIAudioVolume();
            mouseSensSlider.value = GetMouseSensitivity();
            gamePadPrecisionSensSlider.value = GetGamePadPrecisionSensitivity();
            gamePadMaxSensSlider.value = GetGamePadMaximumSensitivity();
            
            // Toggles
            toggleMasterAudio.SetIsOnWithoutNotify(IsMasterAudioOn());
            toggleMusicAudio.SetIsOnWithoutNotify(IsMusicOn());
            toggleSfxAudio.SetIsOnWithoutNotify(IsSfxAudioOn());
            toggleUiAudio.SetIsOnWithoutNotify(IsUIAudioOn());
            
            // Subscribe to changes:
            toggleMasterAudio.onValueChanged.AddListener(ToggleMasterAudio);
            toggleMusicAudio.onValueChanged.AddListener(ToggleMusic);
            toggleSfxAudio.onValueChanged.AddListener(ToggleSfxAudio);
            toggleUiAudio.onValueChanged.AddListener(ToggleUIAudio);
            masterAudioSlider.onValueChanged.AddListener(SetMasterVolume);
            musicAudioSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxAudioSlider.onValueChanged.AddListener(SetSfxAudioVolume);
            uiAudioSlider.onValueChanged.AddListener(SetUIAudioVolume);
            mouseSensSlider.onValueChanged.AddListener(SetMouseSensitivity);
            gamePadPrecisionSensSlider.onValueChanged.AddListener(SetGamePadStickPrecisionSens);
            gamePadMaxSensSlider.onValueChanged.AddListener(SetGamePadStickMaximumSens);
        }
        
        /// <summary>
        /// Changes the active game mode. Expected indices: 0 = SinglePlayer, 1 = CoopTwoShips, 2 = CoopOneShip.
        /// </summary>
        public void SetGameMode(int modeIndex) {
            GlobalManager.Instance.GlobalSettings.ActiveGameMode = (GameMode) modeIndex;
            Debug.Log($"Settings changed: Game Mode set to {GlobalManager.Instance.GlobalSettings.ActiveGameMode}");
        }

        /// <summary>
        /// Toggles music audio on or off.
        /// </summary>
        public void ToggleMusic(bool enable) {
            float newVolume = ToggleAudio(GlobalManager.Instance.GlobalSettings.MusicMixerGroup, GlobalManager.Instance.GlobalSettings.MusicVolumeParam, enable);
            musicAudioSlider.value = newVolume;
        }
        
        /// <summary>
        /// Returns the toggle state (on/off) of the Music Audio.
        /// </summary>
        /// <returns>True if on, False if off.</returns>
        public bool IsMusicOn() {
            return GlobalManager.Instance.GlobalSettings.GetMusicVolume() > MinAudioVolume;
        }

        /// <summary>
        /// Sets the Music Audio volume to the specified value, but limited to between 0 and 1.
        /// </summary>
        public void SetMusicVolume(float volume) {
            SetAudioVolume(GlobalManager.Instance.GlobalSettings.MusicMixerGroup, GlobalManager.Instance.GlobalSettings.MusicVolumeParam, volume);
        }
        
        /// <summary>
        /// Toggles all audio on or off.
        /// </summary>
        public void ToggleMasterAudio(bool enable) {
            float newVolume = ToggleAudio(GlobalManager.Instance.GlobalSettings.MasterMixerGroup, GlobalManager.Instance.GlobalSettings.MasterVolumeParam, enable);
            masterAudioSlider.value = newVolume;
        }

        /// <summary>
        /// Returns the toggle state (on/off) of the Master Audio.
        /// </summary>
        /// <returns>True if on, False if off.</returns>
        public bool IsMasterAudioOn() {
            return GlobalManager.Instance.GlobalSettings.GetMasterVolume() > MinAudioVolume;
        }

        /// <summary>
        /// Sets the Master Audio volume to the specified value, but limited to between 0 and 1.
        /// </summary>
        public void SetMasterVolume(float volume) {
            SetAudioVolume(GlobalManager.Instance.GlobalSettings.MasterMixerGroup, GlobalManager.Instance.GlobalSettings.MasterVolumeParam, volume);
        }
        
        /// <summary>
        /// Toggles SFX audio on or off.
        /// </summary>
        public void ToggleSfxAudio(bool enable) {
            float newVolume = ToggleAudio(GlobalManager.Instance.GlobalSettings.SfxMixerGroup, GlobalManager.Instance.GlobalSettings.SfxVolumeParam, enable);
            sfxAudioSlider.value = newVolume;
        }
        
        /// <summary>
        /// Returns the toggle state (on/off) of the SFX Audio.
        /// </summary>
        /// <returns>True if on, False if off.</returns>
        public bool IsSfxAudioOn() {
            return GlobalManager.Instance.GlobalSettings.GetSfxVolume() > MinAudioVolume;
        }
        
        /// <summary>
        /// Sets the SFX Audio volume to the specified value, but limited to between 0 and 1.
        /// </summary>
        public void SetSfxAudioVolume(float volume) {
            SetAudioVolume(GlobalManager.Instance.GlobalSettings.SfxMixerGroup, GlobalManager.Instance.GlobalSettings.SfxVolumeParam, volume);
        }
        
        /// <summary>
        /// Toggles UI audio on or off.
        /// </summary>
        public void ToggleUIAudio(bool enable) {
            float newVolume = ToggleAudio(GlobalManager.Instance.GlobalSettings.UiMixerGroup, GlobalManager.Instance.GlobalSettings.UIVolumeParam, enable);
            uiAudioSlider.value = newVolume;
        }
        
        /// <summary>
        /// Returns the toggle state (on/off) of the UI Audio.
        /// </summary>
        /// <returns>True if on, False if off.</returns>
        public bool IsUIAudioOn() {
            return GlobalManager.Instance.GlobalSettings.GetUIAudioVolume() > MinAudioVolume;
        }

        /// <summary>
        /// Sets the UI Audio volume to the specified value, but limited to between 0 and 1.
        /// </summary>
        public void SetUIAudioVolume(float volume) {
            SetAudioVolume(GlobalManager.Instance.GlobalSettings.UiMixerGroup, GlobalManager.Instance.GlobalSettings.UIVolumeParam, volume);
        }

        /// <summary>
        /// Change the mouse sensitivity. Only applies to mouse input (not gamepad sticks)
        /// </summary>
        /// <param name="value"></param>
        public void SetMouseSensitivity(float value) {
            GlobalManager.Instance.GlobalSettings.MouseSens = value;
            Debug.Log($"Settings changed: MouseSensitivity set to {value}");
        }
        
        /// <summary>
        /// Gets the mouse sensitivity.
        /// </summary>
        public float GetMouseSensitivity() {
            return GlobalManager.Instance.GlobalSettings.MouseSens;
        }

        /// <summary>
        /// Change the GamePad's base sensitivity. Only applies to gamepad stick input (not mouse).
        /// This is the amount the turret moves when you begin moving the gamepad stick.
        /// The other exposed sensitivity options are applied based on how long the gamepad stick has been held down.
        /// </summary>
        /// <param name="value"></param>
        public void SetGamePadStickPrecisionSens(float value) {
            GlobalManager.Instance.GlobalSettings.PrecisionSens = value;
            Debug.Log($"Settings changed: GamePadStickPrecisionSens set to {value}");
        }
        
        /// <summary>
        /// Gets the GamePad's base sensitivity.
        /// </summary>
        public float GetGamePadPrecisionSensitivity() {
            return GlobalManager.Instance.GlobalSettings.PrecisionSens;
        }

        /// <summary>
        /// Change the GamePad's maximum sensitivity. Only applies to gamepad stick input (not mouse).
        /// This is the amount maximum speed which the turret can move after holding down the stick for some time.
        /// </summary>
        /// <param name="value"></param>
        public void SetGamePadStickMaximumSens(float value) {
            GlobalManager.Instance.GlobalSettings.MaxSlewSens = value;
            Debug.Log($"Settings changed: GamePadStickMaximumSens set to {value}");
        }
        
        /// <summary>
        /// Gets the GamePad's maximum sensitivity.
        /// </summary>
        public float GetGamePadMaximumSensitivity() {
            return GlobalManager.Instance.GlobalSettings.MaxSlewSens;
        }
        
        // Helper methods:
        private float ToggleAudio(AudioMixerGroup mixerGroup, string volumeParam, bool enable) {
            AudioMixer mixer = mixerGroup.audioMixer;
            float newVolume = enable ? MaxAudioVolume : MinAudioVolume;
            mixer.SetFloat(volumeParam, newVolume);
            Debug.Log($"Settings changed: {volumeParam} toggled to {newVolume} DB");
            return newVolume;
        }

        private void SetAudioVolume(AudioMixerGroup mixerGroup, string volumeParam, float volume) {
            mixerGroup.audioMixer.SetFloat(volumeParam, volume);
            Debug.Log($"Settings changed: {volumeParam} set to {volume} DB");
        }
    }
}
