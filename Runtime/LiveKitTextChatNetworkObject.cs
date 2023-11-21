using System;
using UnityEngine;

namespace Extreal.Integration.Chat.LiveKit
{
    [Serializable]
    public class LiveKitTextChatNetworkObject : ISerializationCallbackReceiver
    {
        [NonSerialized]
        public Guid ObjectGuid;

        [SerializeField]
        private string ObjectId;

        public int GameObjectHash;

        public string Message;
        public string Name;

        [NonSerialized]
        public DateTime DateTime_CreatedAt;

        [NonSerialized]
        public DateTime DateTime_UpdatedAt;

        public LiveKitTextChatNetworkObject()
        {

        }

        public void OnBeforeSerialize()
        {

        }

        public void OnAfterDeserialize()
        {

        }
    }
}
