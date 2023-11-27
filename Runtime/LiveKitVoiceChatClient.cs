using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using Extreal.Core.Logging;
using LiveKit;
using UniRx;

namespace Extreal.Integration.Chat.LiveKit
{
    public class LiveKitVoiceChatClient : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(LiveKitVoiceChatClient));

        private readonly LiveKitRoomClient liveKitRoomClient;
        private bool mute;
        private float volume;

        public IObservable<bool> OnMuted => onMuted;
        private readonly Subject<bool> onMuted = new Subject<bool>();

        public IObservable<float> OnVolumeChanged => onVolumeChanged;
        private readonly Subject<float> onVolumeChanged = new Subject<float>();

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public LiveKitVoiceChatClient(
            LiveKitRoomClient liveKitRoomClient,
            LiveKitVoiceChatConfig livekitVoiceChatConfig
        )
        {
            this.liveKitRoomClient = liveKitRoomClient;

            mute = livekitVoiceChatConfig.InitialMute;
            volume = livekitVoiceChatConfig.InitialVolume;

            this.liveKitRoomClient.OnTrackSubscribed
                .Subscribe(param => OnTrackSubscribed(param.track, param.participant))
                .AddTo(disposables);

            this.liveKitRoomClient.OnTrackUnsubscribed
                .Subscribe(param => OnTrackUnsubscribed(param.track, param.participant))
                .AddTo(disposables);

            this.liveKitRoomClient.OnActiveSpeakersChanged
                .Subscribe(OnActiveSpeakersChanged)
                .AddTo(disposables);

            this.liveKitRoomClient.OnParticipantConnected
                .Subscribe(OnParticipantConnected)
                .AddTo(disposables);
        }

        private void OnTrackSubscribed(RemoteTrack track, RemoteParticipant participant)
        {
            if (track.Kind == TrackKind.Audio)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Subscribed {participant.Identity} 's audio track");
                }
                track.Attach();
            }
        }

        private void OnTrackUnsubscribed(RemoteTrack track, RemoteParticipant participant)
        {
            if (track.Kind == TrackKind.Audio)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"Unsubscribed {participant.Identity} 's audio track");
                }
                track.Detach();
            }
        }

        private void OnActiveSpeakersChanged(JSArray<Participant> speakers)
        {
            foreach (var speaker in speakers)
            {
                if (Logger.IsDebug())
                {
                    Logger.LogDebug($"{speaker.Identity}: {speaker.AudioLevel}");
                }
            }
        }

        private void OnParticipantConnected(RemoteParticipant participant)
            => participant.SetVolume(volume);

        /// <summary>
        /// Toggles mute or not.
        /// </summary>
        /// <returns></returns>
        public async UniTask ToggleMute()
        {
            mute = !mute;
            await liveKitRoomClient.SetMicrophoneEnabled(!mute);
            onMuted.OnNext(mute);
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Mute: {mute}");
            }
        }

        public void SetVolume(float value)
        {
            foreach (var participant in liveKitRoomClient.Participants.Values)
            {
                volume = value;
                participant.SetVolume(volume);
            }
        }

        protected override void ReleaseManagedResources()
        {
            onMuted.Dispose();
            disposables.Dispose();

            base.ReleaseManagedResources();
        }
    }
}
