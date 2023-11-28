using UniRx;
using VContainer.Unity;
using Extreal.Core.Common.System;
using Cysharp.Threading.Tasks;

namespace Extreal.Integration.Chat.LiveKit.LiveKitTemp
{
    public class LiveKitTempPresenter : DisposableBase, IInitializable
    {
        private readonly LiveKitTempView liveKitTempView;
        private readonly LiveKitRoomClient liveKitRoomClient;
        private readonly LiveKitVoiceChatClient liveKitVoiceChatClient;

        private readonly CompositeDisposable disposables = new CompositeDisposable();

        public LiveKitTempPresenter(
            LiveKitTempView liveKitTempView,
            LiveKitRoomClient liveKitRoomClient,
            LiveKitVoiceChatClient liveKitVoiceChatClient
        )
        {
            this.liveKitTempView = liveKitTempView;
            this.liveKitRoomClient = liveKitRoomClient;
            this.liveKitVoiceChatClient = liveKitVoiceChatClient;
        }

        public void Initialize()
        {
            liveKitTempView.OnConnectButtonClicked
                .Subscribe(async playerName => await liveKitRoomClient.StartClientAsync("TestRoom", playerName))
                .AddTo(disposables);

            liveKitTempView.OnDisconnectButtonClicked
                .Subscribe(_ => liveKitRoomClient.StopClient())
                .AddTo(disposables);

            liveKitTempView.OnMuteUnmuteButtonClicked
                .Subscribe(_ => liveKitVoiceChatClient.ToggleMute().Forget())
                .AddTo(disposables);

            liveKitVoiceChatClient.OnMuted
                .Subscribe(liveKitTempView.SetMuteText)
                .AddTo(disposables);

            liveKitTempView.OnVolumeSliderChange
                .Subscribe(volume =>
                {
                    liveKitVoiceChatClient.SetVolume(volume);
                    liveKitTempView.SetVolumeText(volume);
                })
                .AddTo(disposables);
        }

        protected override void ReleaseManagedResources()
        {
            disposables.Dispose();
            base.ReleaseManagedResources();
        }
    }
}
