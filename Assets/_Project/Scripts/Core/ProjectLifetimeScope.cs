using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace BreachAR.Core
{
    /// <summary>
    /// Project-wide lifetime scope for VContainer DI
    /// Registers all global singleton services
    /// Referência: 99_agent_rules.md - DI via VContainer
    /// </summary>
    public class ProjectLifetimeScope : LifetimeScope
    {
        [Header("Service Prefabs (optional)")]
        [SerializeField] private GameObject audioManagerPrefab;
        [SerializeField] private GameObject analyticsServicePrefab;
        [SerializeField] private GameObject poolManagerPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            // AR
            builder.Register<AR.ARSessionService>(Lifetime.Singleton);
            builder.Register<AR.CorePlacementService>(Lifetime.Singleton);

            // Backend
            builder.Register<Backend.SaveService>(Lifetime.Singleton);
            builder.Register<Backend.EconomyService>(Lifetime.Singleton);
            builder.Register<Backend.RemoteConfigService>(Lifetime.Singleton);
            builder.Register<Backend.LeaderboardService>(Lifetime.Singleton);
            builder.Register<Backend.ScoreValidator>(Lifetime.Singleton);
            builder.Register<Backend.RateLimiter>(Lifetime.Singleton);
            builder.Register<Backend.CloudSyncService>(Lifetime.Singleton);

            // Analytics
            builder.Register<Analytics.AnalyticsService>(Lifetime.Singleton);

            // AI
            builder.Register<AI.DifficultyDirector>(Lifetime.Singleton);
            builder.Register<AI.WaveGenerator>(Lifetime.Singleton);
            builder.Register<AI.RiftSpawnDirector>(Lifetime.Singleton);

            // Physics
            builder.Register<Physics.PhysicsManager>(Lifetime.Singleton);
            builder.Register<Physics.LaunchSystem>(Lifetime.Singleton);

            // Core
            builder.Register<DeviceTierDetector>(Lifetime.Singleton);
            builder.Register<GameManager>(Lifetime.Singleton);

            // Utils
            builder.Register<Utils.EventDispatcher>(Lifetime.Singleton);
            builder.Register<Utils.InputHandler>(Lifetime.Singleton);
            builder.Register<Utils.SceneLoader>(Lifetime.Singleton);
            builder.Register<Utils.NotificationManager>(Lifetime.Singleton);
            builder.Register<Utils.CoroutineHelper>(Lifetime.Singleton);

            // AudioManager (if prefab provided)
            if (audioManagerPrefab != null)
            {
                builder.RegisterComponent(audioManagerPrefab.GetComponent<Audio.AudioManager>());
            }
            else
            {
                builder.Register<Audio.AudioManager>(Lifetime.Singleton);
            }

            // PoolManager (if prefab provided)
            if (poolManagerPrefab != null)
            {
                builder.RegisterComponent(poolManagerPrefab.GetComponent<Utils.PoolManager>());
            }
            else
            {
                builder.Register<Utils.PoolManager>(Lifetime.Singleton);
            }

            Debug.Log("[VContainer] ProjectLifetimeScope configured");
        }
    }
}
