

using System;

namespace Extreal.Integration.Chat.LiveKit
{
    [Serializable]
    public class LiveKitMessage
    {
        public string Topic;
        public LiveKitTextChatNetworkObject Payload;

        public LiveKitMessage(string topic, LiveKitTextChatNetworkObject payload)
        {

        }

        public LiveKitMessage(string topic)
        {

        }

        public string ToJson()
        {
            return string.Empty;
        }
    }
}
