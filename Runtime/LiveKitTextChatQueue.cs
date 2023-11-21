using UnityEngine;

using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

using System.Runtime.InteropServices;
using AOT;

namespace Extreal.Integration.Chat.LiveKit
{
    public class LiveKitTextChatQueue
    {
        // リクエストキュー
        public Queue<LiveKitMessage> RequestQueue = new Queue<LiveKitMessage>();

        // レスポンスキュー
        public static Queue<LiveKitMessage> ResponseQueue = new Queue<LiveKitMessage>();

        public int ResponseQueueCount()
        {
            return ResponseQueue.Count;
        }

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

        public LiveKitTextChatQueue(string url, string roomName, LiveKitTextChatNetworkObject networkObj)
        {

        }

        public async UniTask Connect()
        {

        }

        public async UniTask Update()
        {
        }

        public async UniTask Close()
        {
        }
    }
}
