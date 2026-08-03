using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Backend
{
    /// <summary>
    /// Manages remote configuration for balancing and feature flags
    /// Injected via VContainer DI
    /// </summary>
    public class RemoteConfigService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string configUrl;
        [SerializeField] private float refreshInterval = 300f;

        private Dictionary<string, string> configCache;
        private Dictionary<string, object> defaultValues;
        private float lastFetchTime;
        private bool isInitialized;

        private void Start()
        {
            InitializeDefaults();
            FetchConfig();
        }

        private void Update()
        {
            if (Time.time - lastFetchTime > refreshInterval)
            {
                FetchConfig();
            }
        }

        private void InitializeDefaults()
        {
            defaultValues = new Dictionary<string, object>
            {
                { "dd_target_skill_score", 0.5f },
                { "dd_sensitivity", 1f },
                { "dd_max_delta", 0.15f },
                { "session_reward_multiplier", 1f },
                { "max_soft_currency", 999999 },
                { "ad_interstitial_interval", 3 },
                { "ad_revive_reward", 1 },
                { "boss_wave_interval", 5 },
                { "daily_challenge_seed_offset", 0 },
                { "feature_battle_pass", true },
                { "feature_daily_challenge", true },
                { "feature_multiplayer", false }
            };

            configCache = new Dictionary<string, string>();
        }

        /// <summary>
        /// Fetch configuration from remote
        /// </summary>
        public void FetchConfig()
        {
            Debug.Log("[RemoteConfig] Fetching configuration...");
            lastFetchTime = Time.time;
            isInitialized = true;
        }

        /// <summary>
        /// Get a string value
        /// </summary>
        public string GetString(string key, string defaultValue = "")
        {
            if (configCache.ContainsKey(key))
            {
                return configCache[key];
            }

            if (defaultValues.ContainsKey(key))
            {
                return defaultValues[key].ToString();
            }

            return defaultValue;
        }

        /// <summary>
        /// Get an integer value
        /// </summary>
        public int GetInt(string key, int defaultValue = 0)
        {
            string value = GetString(key);
            if (int.TryParse(value, out int result))
            {
                return result;
            }

            if (defaultValues.ContainsKey(key) && defaultValues[key] is int intVal)
            {
                return intVal;
            }

            return defaultValue;
        }

        /// <summary>
        /// Get a float value
        /// </summary>
        public float GetFloat(string key, float defaultValue = 0f)
        {
            string value = GetString(key);
            if (float.TryParse(value, System.Globalization.NumberStyles.Float, 
                System.Globalization.CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }

            if (defaultValues.ContainsKey(key) && defaultValues[key] is float floatVal)
            {
                return floatVal;
            }

            return defaultValue;
        }

        /// <summary>
        /// Get a boolean value
        /// </summary>
        public bool GetBool(string key, bool defaultValue = false)
        {
            string value = GetString(key).ToLower();
            
            if (value == "true" || value == "1" || value == "yes")
                return true;
            if (value == "false" || value == "0" || value == "no")
                return false;

            if (defaultValues.ContainsKey(key) && defaultValues[key] is bool boolVal)
            {
                return boolVal;
            }

            return defaultValue;
        }

        /// <summary>
        /// Check if config is initialized
        /// </summary>
        public bool IsInitialized()
        {
            return isInitialized;
        }

        /// <summary>
        /// Force refresh config
        /// </summary>
        public void ForceRefresh()
        {
            FetchConfig();
        }
    }
}
