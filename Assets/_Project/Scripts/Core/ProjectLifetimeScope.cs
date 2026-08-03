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
            // =====================================================================
            // AR Services
            // =====================================================================
            builder.Register<AR.ARSessionService>(Lifetime.Singleton);
            builder.Register<AR.CorePlacementService>(Lifetime.Singleton);
            builder.Register<AR.DeviceCompatibilityService>(Lifetime.Singleton);
            builder.Register<AR.PoissonDiscDistribution>(Lifetime.Singleton);
            builder.Register<AR.TrackingRecoveryService>(Lifetime.Singleton);
            builder.Register<AR.OcclusionService>(Lifetime.Singleton);
            builder.Register<AR.DynamicMeshCollider>(Lifetime.Singleton);
            builder.Register<AR.PlaneDetectionService>(Lifetime.Singleton);
            builder.Register<AR.RiftAnchorManager>(Lifetime.Singleton);

            // =====================================================================
            // Backend Services
            // =====================================================================
            builder.Register<Backend.SupabaseService>(Lifetime.Singleton);
            builder.Register<Backend.SaveService>(Lifetime.Singleton);
            builder.Register<Backend.EconomyService>(Lifetime.Singleton);
            builder.Register<Backend.RemoteConfigService>(Lifetime.Singleton);
            builder.Register<Backend.LeaderboardService>(Lifetime.Singleton);
            builder.Register<Backend.ScoreValidator>(Lifetime.Singleton);
            builder.Register<Backend.RateLimiter>(Lifetime.Singleton);
            builder.Register<Backend.CloudSyncService>(Lifetime.Singleton);
            builder.Register<Backend.PrivacyService>(Lifetime.Singleton);
            builder.Register<Backend.BackupService>(Lifetime.Singleton);
            builder.Register<Backend.PlayerEconomyService>(Lifetime.Singleton);
            builder.Register<Backend.IAPValidationService>(Lifetime.Singleton);
            builder.Register<Backend.EnvironmentConfigService>(Lifetime.Singleton);

            // =====================================================================
            // Analytics Services
            // =====================================================================
            builder.Register<Analytics.AnalyticsService>(Lifetime.Singleton);
            builder.Register<Analytics.PerformanceTelemetryService>(Lifetime.Singleton);

            // =====================================================================
            // AI Services
            // =====================================================================
            builder.Register<AI.DifficultyDirector>(Lifetime.Singleton);
            builder.Register<AI.WaveGenerator>(Lifetime.Singleton);
            builder.Register<AI.RiftSpawnDirector>(Lifetime.Singleton);

            // =====================================================================
            // Physics Services
            // =====================================================================
            builder.Register<Physics.PhysicsManager>(Lifetime.Singleton);
            builder.Register<Physics.LaunchSystem>(Lifetime.Singleton);

            // =====================================================================
            // Core Services
            // =====================================================================
            builder.Register<DeviceTierDetector>(Lifetime.Singleton);
            builder.Register<GameManager>(Lifetime.Singleton);
            builder.Register<GraphicsQualityService>(Lifetime.Singleton);

            // =====================================================================
            // Utility Services
            // =====================================================================
            builder.Register<Utils.EventDispatcher>(Lifetime.Singleton);
            builder.Register<Utils.InputHandler>(Lifetime.Singleton);
            builder.Register<Utils.SceneLoader>(Lifetime.Singleton);
            builder.Register<Utils.NotificationManager>(Lifetime.Singleton);
            builder.Register<Utils.CoroutineHelper>(Lifetime.Singleton);
            builder.Register<Utils.GCReductionService>(Lifetime.Singleton);
            builder.Register<Utils.MemoryProfilerService>(Lifetime.Singleton);
            builder.Register<Utils.BuildSizeOptimizer>(Lifetime.Singleton);
            builder.Register<Utils.MobileShaderOptimizer>(Lifetime.Singleton);
            builder.Register<Utils.MeshColliderOptimizer>(Lifetime.Singleton);
            builder.Register<Utils.TextureOptimizer>(Lifetime.Singleton);
            builder.Register<AR.DepthAPIOptimizer>(Lifetime.Singleton);

            // =====================================================================
            // AudioManager (if prefab provided)
            // =====================================================================
            if (audioManagerPrefab != null)
            {
                builder.RegisterComponent(audioManagerPrefab.GetComponent<Audio.AudioManager>());
            }
            else
            {
                builder.Register<Audio.AudioManager>(Lifetime.Singleton);
            }

            // =====================================================================
            // PoolManager (if prefab provided)
            // =====================================================================
            if (poolManagerPrefab != null)
            {
                builder.RegisterComponent(poolManagerPrefab.GetComponent<Utils.PoolManager>());
            }
            else
            {
                builder.Register<Utils.PoolManager>(Lifetime.Singleton);
            }

            Debug.Log("[VContainer] ProjectLifetimeScope configured with all services");
        }
    }
}
