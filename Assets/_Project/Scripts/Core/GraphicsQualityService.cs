using UnityEngine;
using VContainer;

namespace BreachAR.Core
{
    /// <summary>
    /// Automatically adjusts graphics quality based on device tier
    /// Referência: OPT-004
    /// </summary>
    public class GraphicsQualityService : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private DeviceTierDetector tierDetector;

        [Header("Quality Presets")]
        [SerializeField] private QualityPreset lowPreset;
        [SerializeField] private QualityPreset mediumPreset;
        [SerializeField] private QualityPreset highPreset;

        [Header("Adaptation")]
        [SerializeField] private bool enableAdaptation = true;
        [SerializeField] private float fpsCheckInterval = 5f;
        [SerializeField] private float targetFPSBuffer = 5f;

        private QualityPreset currentPreset;
        private float lastFPSCheck;
        private float currentFPS;
        private int currentQualityLevel;
        private bool isAdapting;

        public QualityPreset CurrentPreset => currentPreset;
        public float CurrentFPS => currentFPS;

        private void Start()
        {
            InitializePresets();
            ApplyTierPreset();
        }

        private void Update()
        {
            if (!enableAdaptation) return;

            // Update FPS calculation
            currentFPS = 1f / Time.unscaledDeltaTime;

            // Check if we need to adapt
            if (Time.time - lastFPSCheck >= fpsCheckInterval)
            {
                CheckAndAdapt();
                lastFPSCheck = Time.time;
            }
        }

        /// <summary>
        /// Initialize quality presets
        /// </summary>
        private void InitializePresets()
        {
            if (lowPreset == null)
            {
                lowPreset = new QualityPreset
                {
                    Name = "Low",
                    QualityLevel = 0,
                    ParticleBudget = 50,
                    ShadowQuality = ShadowQuality.Low,
                    TextureQuality = TextureQuality.QuarterRes,
                    AntiAliasing = 0,
                    VSyncCount = 0,
                    TargetFPS = 30
                };
            }

            if (mediumPreset == null)
            {
                mediumPreset = new QualityPreset
                {
                    Name = "Medium",
                    QualityLevel = 1,
                    ParticleBudget = 150,
                    ShadowQuality = ShadowQuality.Medium,
                    TextureQuality = TextureQuality.HalfRes,
                    AntiAliasing = 2,
                    VSyncCount = 0,
                    TargetFPS = 45
                };
            }

            if (highPreset == null)
            {
                highPreset = new QualityPreset
                {
                    Name = "High",
                    QualityLevel = 2,
                    ParticleBudget = 300,
                    ShadowQuality = ShadowQuality.High,
                    TextureQuality = TextureQuality.FullRes,
                    AntiAliasing = 4,
                    VSyncCount = 1,
                    TargetFPS = 60
                };
            }
        }

        /// <summary>
        /// Apply preset for detected tier
        /// Referência: OPT-004
        /// </summary>
        public void ApplyTierPreset()
        {
            if (tierDetector == null)
            {
                Debug.LogWarning("[GraphicsQuality] No tier detector, using medium preset");
                ApplyPreset(mediumPreset);
                return;
            }

            DeviceTier tier = tierDetector.DetectedTier;

            switch (tier)
            {
                case DeviceTier.Low:
                    ApplyPreset(lowPreset);
                    break;
                case DeviceTier.Medium:
                    ApplyPreset(mediumPreset);
                    break;
                case DeviceTier.High:
                    ApplyPreset(highPreset);
                    break;
            }

            Debug.Log($"[GraphicsQuality] Applied {currentPreset.Name} preset for tier {tier}");
        }

        /// <summary>
        /// Apply a specific quality preset
        /// </summary>
        public void ApplyPreset(QualityPreset preset)
        {
            if (preset == null) return;

            currentPreset = preset;
            currentQualityLevel = preset.QualityLevel;

            // Apply Unity quality settings
            QualitySettings.SetQualityLevel(preset.QualityLevel);
            QualitySettings.antiAliasing = preset.AntiAliasing;
            QualitySettings.vSyncCount = preset.VSyncCount;
            Application.targetFrameRate = preset.TargetFPS;

            // Apply shadow quality
            switch (preset.ShadowQuality)
            {
                case ShadowQuality.Off:
                    QualitySettings.shadows = UnityEngine.ShadowQuality.Disable;
                    break;
                case ShadowQuality.Low:
                    QualitySettings.shadows = UnityEngine.ShadowQuality.HardOnly;
                    QualitySettings.shadowDistance = 20f;
                    break;
                case ShadowQuality.Medium:
                    QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                    QualitySettings.shadowDistance = 40f;
                    break;
                case ShadowQuality.High:
                    QualitySettings.shadows = UnityEngine.ShadowQuality.All;
                    QualitySettings.shadowDistance = 80f;
                    QualitySettings.shadowResolution = UnityEngine.ShadowResolution.High;
                    break;
            }

            // Apply texture quality
            switch (preset.TextureQuality)
            {
                case TextureQuality.FullRes:
                    QualitySettings.globalTextureMipmapLimit = 0;
                    break;
                case TextureQuality.HalfRes:
                    QualitySettings.globalTextureMipmapLimit = 1;
                    break;
                case TextureQuality.QuarterRes:
                    QualitySettings.globalTextureMipmapLimit = 2;
                    break;
            }

            // Broadcast quality change
            GameEvents.OnSettingsChanged?.Invoke(new SettingsChangedData
            {
                SettingName = $"GraphicsQuality:{preset.Name}"
            });
        }

        /// <summary>
        /// Check FPS and adapt quality if needed
        /// Referência: OPT-004
        /// </summary>
        private void CheckAndAdapt()
        {
            if (currentPreset == null || isAdapting) return;

            float targetFPS = currentPreset.TargetFPS - targetFPSBuffer;

            // If FPS is too low, downgrade
            if (currentFPS < targetFPS * 0.8f && currentQualityLevel > 0)
            {
                Debug.Log($"[GraphicsQuality] FPS too low ({currentFPS:F0}), downgrading");
                DowngradeQuality();
            }
            // If FPS is good and we're not at max, try upgrading
            else if (currentFPS > currentPreset.TargetFPS * 0.95f && currentQualityLevel < 2)
            {
                // Only upgrade after sustained good performance
                // (simplified - in production, use a window)
            }
        }

        /// <summary>
        /// Downgrade quality level
        /// </summary>
        private void DowngradeQuality()
        {
            isAdapting = true;

            if (currentQualityLevel == 2)
                ApplyPreset(mediumPreset);
            else if (currentQualityLevel == 1)
                ApplyPreset(lowPreset);

            isAdapting = false;
        }

        /// <summary>
        /// Manually set quality level
        /// </summary>
        public void SetQualityLevel(int level)
        {
            switch (level)
            {
                case 0: ApplyPreset(lowPreset); break;
                case 1: ApplyPreset(mediumPreset); break;
                case 2: ApplyPreset(highPreset); break;
            }
        }

        /// <summary>
        /// Get current quality stats
        /// </summary>
        public string GetStats()
        {
            return $"Preset: {currentPreset?.Name ?? "None"} | " +
                   $"FPS: {currentFPS:F0} | " +
                   $"Target: {currentPreset?.TargetFPS ?? 0} | " +
                   $"Level: {currentQualityLevel}";
        }
    }

    /// <summary>
    /// Quality preset configuration
    /// </summary>
    [System.Serializable]
    public class QualityPreset
    {
        public string Name;
        public int QualityLevel;
        public int ParticleBudget;
        public ShadowQuality ShadowQuality;
        public TextureQuality TextureQuality;
        public int AntiAliasing;
        public int VSyncCount;
        public int TargetFPS;
    }
}
