namespace BreachAR.Utils
{
    /// <summary>
    /// Game-wide constants
    /// Referência: 99_agent_rules.md - Configuration via SOs
    /// </summary>
    public static class GameConstants
    {
        // Scene Names
        public const string SCENE_BOOT = "Boot";
        public const string SCENE_MAIN_MENU = "MainMenu";
        public const string SCENE_GAMEPLAY = "Gameplay";

        // Tags
        public const string TAG_ORB = "Orb";
        public const string TAG_FRAGMENT = "Fragment";
        public const string TAG_RIFT = "Rift";
        public const string TAG_CORE = "Core";
        public const string TAG_POWERUP = "PowerUp";
        public const string TAG_REAL_WORLD_SURFACE = "RealWorldSurface";

        // Layers
        public const int LAYER_REAL_WORLD_SURFACE = 8;
        public const int LAYER_ORB = 9;
        public const int LAYER_FRAGMENT = 10;
        public const int LAYER_RIFT = 11;
        public const int LAYER_CORE = 12;
        public const int LAYER_POWERUP = 13;
        public const int LAYER_AR_PLANE = 14;

        // Pool Tags
        public const string POOL_ORB = "Orb";
        public const string POOL_FRAGMENT = "Fragment";
        public const string POOL_RIFT = "Rift";
        public const string POOL_POWERUP = "PowerUp";
        public const string POOL_VFX = "VFX";

        // PlayerPrefs Keys
        public const string PREF_ONBOARDING_COMPLETED = "OnboardingCompleted";
        public const string PREF_PERSONAL_BEST = "PersonalBest";
        public const string PREF_PRIVACY_CONSENT_MADE = "PrivacyConsentMade";
        public const string PREF_ANALYTICS_CONSENT = "AnalyticsConsent";
        public const string PREF_ADS_CONSENT = "AdsConsent";
        public const string PREF_PERSONALIZATION_CONSENT = "PersonalizationConsent";

        // Gameplay
        public const int MAX_COMBO_MULTIPLIER = 5;
        public const float COMBO_WINDOW_SECONDS = 2.5f;
        public const float ORB_MAX_LIFETIME = 10f;
        public const float FRAGMENT_ATTACK_RANGE = 1.5f;
        public const float RIFT_SPAWN_INTERVAL = 3f;

        // UI
        public const float SCORE_DELTA_DISPLAY_TIME = 1f;
        public const float NOTIFICATION_DEFAULT_DURATION = 2f;
        public const float TRANSITION_DEFAULT_DURATION = 0.3f;

        // AR
        public const float MIN_PLANE_AREA = 0.3f;
        public const float SCAN_TIMEOUT = 8f;
        public const int MIN_PLANES_REQUIRED = 2;
        public const float CORE_PLACEMENT_HEIGHT = 0.5f;

        // Economy
        public const int REVIVE_COST_CRYSTALS = 50;
        public const int SCORE_TO_SOFT_CURRENCY_RATIO = 100;
    }
}
