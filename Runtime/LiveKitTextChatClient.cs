using Cysharp.Threading.Tasks;

namespace Extreal.Integration.Chat.LiveKit
{
    public class LiveKitTextChatClient
    {
        public enum StatType
        {
            TXTotal,
            RXTotal,
            TXNum,
            RXNum
        }

        public void EnqueueMessage(string message)
        {

        }

        public void StartClient(string endpoint, string roomName, string playerName)
        {
        }

        public async UniTask StopClient()
        {

        }

        public ulong GetStats(StatType type)
        {
            return 0 ;
        }
    }
}
