using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using LiveKit;
using UnityEngine;
using UnityEngine.Networking;

namespace Extreal.Integration.Chat.LiveKit
{
    public class LiveKitRoomClient : DisposableBase
    {
        public Room Room { get; } = new Room();

        public LiveKitRoomClient()
        {

        }

        public async UniTask StartClientAsync(string accessTokenEndpoint, string livekitEndpoint, string roomName, string playerName)
        {
            var accessToken = await GetAccessToken(accessTokenEndpoint, roomName, playerName);

            await Room.Connect(livekitEndpoint, accessToken);
        }

        private async UniTask<string> GetAccessToken(string accessTokenEndpoint, string roomName, string participantName)
        {
            var webRequest = await UnityWebRequest.Get($"{accessTokenEndpoint}?RoomName={roomName}&ParticipantName={participantName}").SendWebRequest();
            var token = JsonUtility.FromJson<Token>(webRequest.downloadHandler.text);
            return token.AccessToken;
        }

        public void StopClient() => Room.Disconnect();

        protected override void ReleaseManagedResources()
        {
            Room.Disconnect();
            Room.Dispose();

            base.ReleaseManagedResources();
        }

        [SuppressMessage("Usage", "IDE1006")]
        private class Token
        {
            public string RoomName;
            public string ParticipantName;
            public string AccessToken;
        }
    }
}
