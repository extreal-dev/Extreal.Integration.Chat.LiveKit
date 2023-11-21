using System;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.Integration.Chat.LiveKit
{
    public class LiveKitVoiceChatClient : DisposableBase
    {
        public IObservable<bool> OnMuted => onMuted;
        private Subject<bool> onMuted = new Subject<bool>();

        public LiveKitVoiceChatClient()
        {

        }

        public async UniTask StartClient(string accessTokenEndpoint, string livekitEndpoint, string roomName, string playerName)
        {

        }

        public async UniTask StopClient()
        {

        }

        public async UniTask ToggleMute()
        {

        }

        public async UniTask SetVolume(float value)
        {

        }

        public async UniTask SetDevice(int deviceId)
        {

        }

        protected override void ReleaseManagedResources()
        {
            onMuted.Dispose();

            base.ReleaseManagedResources();
        }
    }
}
