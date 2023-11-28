using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Extreal.Core.Logging;
using LiveKit;
using Extreal.Core.Common.System;
using UniRx;

namespace Extreal.Integration.Chat.LiveKit
{
    public class LiveKitTextChatQueue : DisposableBase
    {
        private static readonly ELogger Logger = LoggingManager.GetLogger(nameof(LiveKitTextChatClient));
        public Queue<LiveKitMessage> RequestQueue = new Queue<LiveKitMessage>();
        public static Queue<LiveKitMessage> ResponseQueue = new Queue<LiveKitMessage>();

        private LiveKitRoomClient liveKitRoomClient;
        private readonly CompositeDisposable disposable = new CompositeDisposable();

        public LiveKitTextChatQueue(LiveKitRoomClient liveKitRoomClient)
        {
            this.liveKitRoomClient = liveKitRoomClient;

            liveKitRoomClient.OnDataReceived
                .Subscribe(param => OnDataReceived(param.data, param.participant, param.kind))
                .AddTo(disposable);

            RequestQueue.Clear();
            ResponseQueue.Clear();
        }

        private void OnDataReceived(byte[] data, RemoteParticipant participant, DataPacketKind? kind)
        {
            if (Logger.IsDebug())
            {
                var msg = Encoding.UTF8.GetString(data);
                Logger.LogDebug($"Received Message {msg}");
            }
        }

        /// <summary>
        /// Return the number of response queues
        /// </summary>
        public int ResponseQueueCount() => ResponseQueue.Count;

        public LiveKitMessage DequeueResponse()
        {
            if (ResponseQueue.Count == 0)
            {
                return null;
            }
            else
            {
                return ResponseQueue.Dequeue();
            }
        }

        /// <summary>
        /// Constantly check the request queue and process sending messages.
        /// </summary>
        public async UniTask QueueCheck()
        {
            while (RequestQueue.Count > 0)
            {
                LiveKitMessage liveKitMessage = RequestQueue.Dequeue();
                string jsonLiveKitMessage = liveKitMessage.ToJson();

                liveKitRoomClient.PublishData(Encoding.ASCII.GetBytes(jsonLiveKitMessage), DataPacketKind.RELIABLE);
            }
        }

        protected override void ReleaseManagedResources()
        {
            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Close");
            }

            RequestQueue.Clear();
            ResponseQueue.Clear();

            liveKitRoomClient.StopClient();

            if (Logger.IsDebug())
            {
                Logger.LogDebug($"Disconnected");
            }

            base.ReleaseManagedResources();
        }
    }
}
