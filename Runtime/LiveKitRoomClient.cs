using System.ComponentModel.Design;
using System;
using System.Diagnostics.CodeAnalysis;
using Cysharp.Threading.Tasks;
using Extreal.Core.Common.System;
using LiveKit;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;

namespace Extreal.Integration.Chat.LiveKit
{
    public class LiveKitRoomClient : DisposableBase
    {
        private readonly LiveKitConfig liveKitConfig;

        private readonly Room room = new Room();

        public IObservable<(byte[] data, RemoteParticipant participant, DataPacketKind? kind)> OnDataReceived => onDataReceived;
        private readonly Subject<(byte[], RemoteParticipant, DataPacketKind?)> onDataReceived = new Subject<(byte[], RemoteParticipant, DataPacketKind?)>();

        public IObservable<(RemoteTrack track, RemoteTrackPublication publication, RemoteParticipant participant)> OnTrackSubscribed => onTrackSubscribed;
        private readonly Subject<(RemoteTrack, RemoteTrackPublication, RemoteParticipant)> onTrackSubscribed = new Subject<(RemoteTrack, RemoteTrackPublication, RemoteParticipant)>();

        public IObservable<(RemoteTrack track, RemoteTrackPublication publication, RemoteParticipant participant)> OnTrackUnsubscribed => onTrackUnsubscribed;
        private readonly Subject<(RemoteTrack, RemoteTrackPublication, RemoteParticipant)> onTrackUnsubscribed = new Subject<(RemoteTrack, RemoteTrackPublication, RemoteParticipant)>();

        public IObservable<JSArray<Participant>> OnActiveSpeakersChanged => onActiveSpeakersChanged;
        private readonly Subject<JSArray<Participant>> onActiveSpeakersChanged = new Subject<JSArray<Participant>>();

        public IObservable<RemoteParticipant> OnParticipantConnected => onParticipantConnected;
        private readonly Subject<RemoteParticipant> onParticipantConnected = new Subject<RemoteParticipant>();

        public IObservable<RemoteParticipant> OnParticipantDisconnected => onParticipantDisconnected;
        private readonly Subject<RemoteParticipant> onParticipantDisconnected = new Subject<RemoteParticipant>();

        public JSMap<string, RemoteParticipant> Participants => room.Participants;

        public LiveKitRoomClient(LiveKitConfig liveKitConfig)
        {
            this.liveKitConfig = liveKitConfig;

            room.DataReceived += DataReceivedHandler;
            room.TrackSubscribed += TrackSubscribedHandler;
            room.TrackUnsubscribed += TrackUnsubscribedHandler;
            room.ActiveSpeakersChanged += ActiveSpeakersChangedHandler;
            room.ParticipantConnected += ParticipantConnectedHandler;
            room.ParticipantDisconnected += ParticipantDisconnectedHandler;
        }

        public async UniTask StartClientAsync(string roomName, string playerName)
        {
            var accessToken = await GetAccessToken(liveKitConfig.AccessTokenUrl, roomName, playerName);

            await room.Connect(liveKitConfig.ServerUrl, accessToken);
        }

        private async UniTask<string> GetAccessToken(string accessTokenEndpoint, string roomName, string participantName)
        {
            var webRequest = await UnityWebRequest.Get($"{accessTokenEndpoint}?RoomName={roomName}&ParticipantName={participantName}").SendWebRequest();
            var token = JsonUtility.FromJson<Token>(webRequest.downloadHandler.text);
            return token.AccessToken;
        }

        public void StopClient() => room.Disconnect();

        private void DataReceivedHandler(byte[] data, RemoteParticipant participant, DataPacketKind? kind)
            => onDataReceived.OnNext((data, participant, kind));

        private void TrackSubscribedHandler(RemoteTrack track, RemoteTrackPublication publication, RemoteParticipant participant)
            => onTrackSubscribed.OnNext((track, publication, participant));

        private void TrackUnsubscribedHandler(RemoteTrack track, RemoteTrackPublication publication, RemoteParticipant participant)
            => onTrackUnsubscribed.OnNext((track, publication, participant));

        private void ActiveSpeakersChangedHandler(JSArray<Participant> speakers)
            => onActiveSpeakersChanged.OnNext(speakers);

        private void ParticipantConnectedHandler(RemoteParticipant participant)
            => onParticipantConnected.OnNext(participant);

        private void ParticipantDisconnectedHandler(RemoteParticipant participant)
            => onParticipantDisconnected.OnNext(participant);

        public async UniTask SetMicrophoneEnabled(bool value)
            => await room.LocalParticipant.SetMicrophoneEnabled(value);

        protected override void ReleaseManagedResources()
        {
            room.Disconnect();
            room.Dispose();

            onDataReceived.Dispose();
            onTrackSubscribed.Dispose();
            onTrackUnsubscribed.Dispose();
            onActiveSpeakersChanged.Dispose();
            onParticipantConnected.Dispose();

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
