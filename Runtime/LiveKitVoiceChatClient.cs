using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using LiveKit;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace Extreal.Integration.Chat.LiveKit
{
    public class LiveKitVoiceChatClient : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(LiveKitVoiceChatClient));

        private AudioCaptureOptions audioCaptureOptions = new AudioCaptureOptions();

        private LiveKitRoomClient liveKitRoomClient;
        private bool mute;

        public IObservable<bool> OnMuted => onMuted;
        private Subject<bool> onMuted = new Subject<bool>();

        public LiveKitVoiceChatClient()
        {

        }

        public void InitializeTrackEvent(LiveKitRoomClient liveKitRoomClient)
        {
            this.liveKitRoomClient = liveKitRoomClient;
            this.liveKitRoomClient.Room.TrackSubscribed += (track, publication, participant) =>
            {
                if (track.Kind == TrackKind.Audio)
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug($"Subscribed {participant.Name} 's audio track");
                    }
                    track.Attach();
                }
            };

            this.liveKitRoomClient.Room.TrackUnsubscribed += (track, publication, participant) =>
            {
                if (track.Kind == TrackKind.Audio)
                {
                    if (Logger.IsDebug())
                    {
                        Logger.LogDebug($"Unsubscribed {participant.Name} 's audio track");
                    }
                    track.Detach();
                }
            };
        }

        /// <summary>
        /// Toggles mute or not.
        /// </summary>
        /// <returns></returns>
        public async UniTask ToggleMute()
        {
            mute = !mute;
            await liveKitRoomClient.Room.LocalParticipant.SetMicrophoneEnabled(mute, audioCaptureOptions);
        }

        public async UniTask SetVolume(float value)
        {
            foreach (var participant in liveKitRoomClient.Room.Participants.Values)
            {
                Debug.Log($"participant.GetVolume(): {participant.GetVolume()}");
                participant.SetVolume(value);
            }
        }

        /// <summary>
        /// Set audio input device with id
        /// </summary>
        /// <param name="deviceId">device id</param>
        /// <returns></returns>
        public async UniTask SetDevice(int deviceId)
            => audioCaptureOptions.DeviceId = deviceId.ToString();

        protected override void ReleaseManagedResources()
        {
            onMuted.Dispose();
            liveKitRoomClient.Dispose();

            base.ReleaseManagedResources();
        }
    }
}
