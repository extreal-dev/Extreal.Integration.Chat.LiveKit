using System.Diagnostics.CodeAnalysis;

namespace Extreal.Integration.Chat.LiveKit
{
    public class LiveKitVoiceChatConfig
    {
        /// <summary>
        /// Initial status of mute.
        /// </summary>
        /// <value>True if initial muted, false otherwise.</value>
        public bool InitialMute { get; }

        public float InitialVolume { get; }

        /// <summary>
        /// Creates VoiceChatConfig with initialMute.
        /// </summary>
        /// <param name="initialMute">True if initial muted, false otherwise.</param>
        [SuppressMessage("Style", "CC0057")]
        public LiveKitVoiceChatConfig(bool initialMute = true, float initialVolume = 1.0f)
        {
            InitialMute = initialMute;
            InitialVolume = initialVolume;
        }
    }
}
