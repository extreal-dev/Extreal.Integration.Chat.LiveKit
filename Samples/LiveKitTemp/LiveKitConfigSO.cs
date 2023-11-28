using UnityEngine;

namespace Extreal.Integration.Chat.LiveKit.LiveKitTemp
{
    [CreateAssetMenu(
            menuName = nameof(Extreal) + "/" + nameof(LiveKitConfigSO),
            fileName = nameof(LiveKitConfigSO))]
    public class LiveKitConfigSO : ScriptableObject
    {
        [SerializeField] private string accessTokenEndpoint = "http://localhost:3000/getToken";
        [SerializeField] private string serverUrl = "http://localhost:7880/";

        public LiveKitConfig LiveKitConfig => new LiveKitConfig(accessTokenEndpoint, serverUrl);
    }
}
