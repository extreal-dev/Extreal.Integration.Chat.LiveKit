using System;
using UnityEngine;

namespace Extreal.Integration.Chat.LiveKit
{
    [Serializable]
    public class LiveKitMessage
    {
        public string Topic;
        public LiveKitTextChatNetworkObject Payload;

        public LiveKitMessage(string topic, LiveKitTextChatNetworkObject payload)
        {
            Topic = topic;
            Payload = payload;
        }

        public LiveKitMessage(string topic)
        {
            Topic = topic;
            Payload = null;
        }

        public string ToJson() => JsonUtility.ToJson(this);
    }
}
