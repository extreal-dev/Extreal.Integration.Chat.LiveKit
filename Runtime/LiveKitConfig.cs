public class LiveKitConfig
{
    public string AccessTokenUrl { get; }
    public string ServerUrl { get; }

    public LiveKitConfig(string accessTokenUrl, string serverUrl)
    {
        AccessTokenUrl = accessTokenUrl;
        ServerUrl = serverUrl;
    }
}
