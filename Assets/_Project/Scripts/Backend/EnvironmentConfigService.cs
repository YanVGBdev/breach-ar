using UnityEngine;
using System;

namespace BreachAR.Backend
{
    /// <summary>
    /// Manages environment configuration (staging vs production)
    /// Referência: BK-025
    /// </summary>
    public class EnvironmentConfigService : MonoBehaviour
    {
        [Header("Environment")]
        [SerializeField] private AppEnvironment currentEnvironment = AppEnvironment.Development;
        [SerializeField] private bool autoDetectEnvironment = true;

        [Header("Staging URLs")]
        [SerializeField] private string stagingSupabaseUrl = "https://staging-project.supabase.co";
        [SerializeField] private string stagingSupabaseKey = "staging-anon-key";

        [Header("Production URLs")]
        [SerializeField] private string productionSupabaseUrl = "https://production-project.supabase.co";
        [SerializeField] private string productionSupabaseKey = "production-anon-key";

        [Header("Feature Flags")]
        [SerializeField] private bool enableCloudSync = true;
        [SerializeField] private bool enableAnalytics = true;
        [SerializeField] private bool enableLeaderboards = true;
        [SerializeField] private bool enableIAP = true;

        private static EnvironmentConfigService instance;
        public static EnvironmentConfigService Instance => instance;

        public AppEnvironment CurrentEnvironment => currentEnvironment;
        public bool IsStaging => currentEnvironment == AppEnvironment.Staging;
        public bool IsProduction => currentEnvironment == AppEnvironment.Production;

        public string SupabaseUrl => currentEnvironment == AppEnvironment.Production
            ? productionSupabaseUrl : stagingSupabaseUrl;

        public string SupabaseKey => currentEnvironment == AppEnvironment.Production
            ? productionSupabaseKey : stagingSupabaseKey;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            if (autoDetectEnvironment)
            {
                DetectEnvironment();
            }
        }

        /// <summary>
        /// Auto-detect environment based on build configuration
        /// Referência: BK-025
        /// </summary>
        private void DetectEnvironment()
        {
#if UNITY_EDITOR
            currentEnvironment = AppEnvironment.Development;
#elif DEVELOPMENT_BUILD
            currentEnvironment = AppEnvironment.Staging;
#else
            currentEnvironment = AppEnvironment.Production;
#endif

            Debug.Log($"[Environment] Detected: {currentEnvironment}");
        }

        /// <summary>
        /// Manually set environment
        /// </summary>
        public void SetEnvironment(AppEnvironment environment)
        {
            currentEnvironment = environment;
            Debug.Log($"[Environment] Set to: {environment}");
        }

        /// <summary>
        /// Check if a feature is enabled for current environment
        /// </summary>
        public bool IsFeatureEnabled(FeatureFlag flag)
        {
            // Features can be disabled in staging for testing
            return flag switch
            {
                FeatureFlag.CloudSync => enableCloudSync || IsProduction,
                FeatureFlag.Analytics => enableAnalytics || IsProduction,
                FeatureFlag.Leaderboards => enableLeaderboards || IsProduction,
                FeatureFlag.IAP => enableIAP && IsProduction,
                FeatureFlag.BattlePass => IsProduction,
                FeatureFlag.DailyChallenge => IsProduction,
                FeatureFlag.Multiplayer => false, // Not yet implemented
                _ => true
            };
        }

        /// <summary>
        /// Get API endpoint for current environment
        /// </summary>
        public string GetApiEndpoint(string service)
        {
            string baseUrl = IsProduction
                ? "https://api.breachar.com"
                : "https://staging-api.breachar.com";

            return $"{baseUrl}/{service}";
        }

        /// <summary>
        /// Get configuration summary for debugging
        /// </summary>
        public string GetConfigSummary()
        {
            return $"=== Environment Config ===\n" +
                   $"Environment: {currentEnvironment}\n" +
                   $"Supabase URL: {SupabaseUrl}\n" +
                   $"Cloud Sync: {IsFeatureEnabled(FeatureFlag.CloudSync)}\n" +
                   $"Analytics: {IsFeatureEnabled(FeatureFlag.Analytics)}\n" +
                   $"Leaderboards: {IsFeatureEnabled(FeatureFlag.Leaderboards)}\n" +
                   $"IAP: {IsFeatureEnabled(FeatureFlag.IAP)}\n" +
                   $"Battle Pass: {IsFeatureEnabled(FeatureFlag.BattlePass)}";
        }
    }

    /// <summary>
    /// Application environments
    /// </summary>
    public enum AppEnvironment
    {
        Development,
        Staging,
        Production
    }

    /// <summary>
    /// Feature flags
    /// </summary>
    public enum FeatureFlag
    {
        CloudSync,
        Analytics,
        Leaderboards,
        IAP,
        BattlePass,
        DailyChallenge,
        Multiplayer
    }
}
