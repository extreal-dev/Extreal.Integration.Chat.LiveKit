using Cysharp.Threading.Tasks;

namespace Extreal.Integration.Chat.LiveKit
{
    public interface ILiveKitQueue
    {
        public UniTask Connect();
        public UniTask Close();
    }
}
