using UnityEngine;
using UnityEngine.Video;

namespace _01_Scripts.Core.UI
{
    /// <summary>
    /// <p>This script handles compatibility between video playing in webgl builds (through browser) and playing video through windows builds -
    /// where the direction of the '/' or '\' differs from platform to platform.</p>
    /// </summary>
    [RequireComponent(typeof(VideoPlayer))]
    public class WebGLVideoLoader : MonoBehaviour
    {
        [Header("WebGL Video Routing")]
        [Tooltip("The exact file name inside the StreamingAssets folder, including the .mp4 extension")]
        [SerializeField] private string videoFileName = "";

        private VideoPlayer _videoPlayer;

        private void Awake() {
            _videoPlayer = GetComponent<VideoPlayer>();
            string rawPath = System.IO.Path.Combine(Application.streamingAssetsPath, videoFileName);
            _videoPlayer.url = rawPath.Replace("\\", "/");
            for (ushort i = 0; i < _videoPlayer.audioTrackCount; i++) {
                _videoPlayer.SetDirectAudioMute(i, true);
            }
            _videoPlayer.Prepare();
            _videoPlayer.prepareCompleted += OnVideoPrepared;
        }

        private void OnVideoPrepared(VideoPlayer source) {
            source.Play();
        }
    }
}