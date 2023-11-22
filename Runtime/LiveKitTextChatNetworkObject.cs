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

        public string Message;
        public string Name;

        [NonSerialized]
        public DateTime DateTime_CreatedAt;

        private long CreatedAt;

        [NonSerialized]
        public DateTime DateTime_UpdatedAt;

        private long UpdatedAt;

        public LiveKitTextChatNetworkObject()
        {
            this.ObjectGuid = Guid.Empty;

            this.DateTime_CreatedAt = DateTime.Now;
            this.DateTime_UpdatedAt = DateTime.Now;

            this.Message = string.Empty;

            this.Name = string.Empty;
        }

        public void OnBeforeSerialize()
        {
            ObjectId = ObjectGuid.ToString();

            CreatedAt = DateTime_CreatedAt.ToBinary();
            UpdatedAt = DateTime_UpdatedAt.ToBinary();
        }

        public void OnAfterDeserialize()
        {
            ObjectGuid = new Guid(ObjectId);

            DateTime_CreatedAt = DateTime.FromBinary(CreatedAt);
            DateTime_UpdatedAt = DateTime.FromBinary(UpdatedAt);
        }
    }
}
