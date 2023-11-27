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

        private LiveKitRoomClient liveKitRoomClient;
        private bool mute;
        private float volume;

        public IObservable<bool> OnMuted => onMuted;
        private Subject<bool> onMuted = new Subject<bool>();

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public LiveKitVoiceChatClient(LiveKitRoomClient liveKitRoomClient)
        {

        }

        public async void InitializeTrackEvent(LiveKitRoomClient liveKitRoomClient)
        {
            this.liveKitRoomClient = liveKitRoomClient;
            this.liveKitRoomClient.OnTrackSubscribed
                .Subscribe(param => OnTrackSubscribed(param.track, param.publication, param.participant))
                .AddTo(disposables);

            this.liveKitRoomClient.OnTrackUnsubscribed
                .Subscribe(param => OnTrackUnsubscribed(param.track, param.publication, param.participant))
                .AddTo(disposables);

            this.liveKitRoomClient.OnActiveSpeakersChanged
                .Subscribe(OnActiveSpeakersChanged)
                .AddTo(disposables);

            this.liveKitRoomClient.OnParticipantConnected
                .Subscribe(OnParticipantConnected)
                .AddTo(disposables);

            if (Logger.IsDebug())
            {
                var localDevicesPromise = Room.GetLocalDevices(MediaDeviceKind.AudioInput, true);
                await localDevicesPromise;
                var localDevices = localDevicesPromise.ResolveValue;
                foreach (var localDevice in localDevices)
                {
                    Logger.LogDebug($"DeviceId: {localDevice.DeviceId}, GroupId: {localDevice.GroupId}, Label: {localDevice.Label}");
                }
            }
        }

        private void OnTrackSubscribed(RemoteTrack track, RemoteTrackPublication publication, RemoteParticipant participant)
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

        private void OnTrackUnsubscribed(RemoteTrack track, RemoteTrackPublication publication, RemoteParticipant participant)
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
