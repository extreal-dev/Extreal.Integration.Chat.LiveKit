using Extreal.Core.Logging;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Extreal.Integration.Chat.LiveKit.LiveKitTemp
{
    public class LiveKitTempScope : LifetimeScope
    {
        [SerializeField] private LiveKitTempView liveKitTempView;
        [SerializeField] private LiveKitConfigSO liveKitConfig;

        protected override void Awake()
        {
            const LogLevel logLevel = LogLevel.Debug;
            LoggingManager.Initialize(logLevel);
            base.Awake();
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(liveKitTempView);
            builder.RegisterComponent(liveKitConfig.LiveKitConfig);

            builder.RegisterComponent(new LiveKitVoiceChatConfig());

            builder.Register<LiveKitRoomClient>(Lifetime.Singleton);
            builder.Register<LiveKitVoiceChatClient>(Lifetime.Singleton);

            builder.RegisterEntryPoint<LiveKitTempPresenter>(Lifetime.Singleton);
        }
    }
}
