using UnityEngine;
using BreachAR.Core;
using VContainer;

namespace BreachAR.AR
{
    /// <summary>
    /// Checks device compatibility for AR features
    /// Referência: AR-016, specs/DeviceTier.md
    /// </summary>
    public class DeviceCompatibilityService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private int minRAMGB = 3;
        [SerializeField] private int minGPUTextureMemoryMB = 512;
        [SerializeField] private float minAndroidVersion = 7f; // API 24
        [SerializeField] private int miniOSVersionMajor = 12;

        private ARDeviceCapability cachedCapability;
        private DeviceTier detectedTier;
        private bool isInitialized;

        public ARDeviceCapability CachedCapability => cachedCapability;
        public DeviceTier DetectedTier => detectedTier;
        public bool IsInitialized => isInitialized;

        private void Awake()
        {
            DetectDevice();
        }

        /// <summary>
        /// Detect device capabilities and tier
        /// Referência: OPT-001, OPT-002
        /// </summary>
        public void DetectDevice()
        {
            cachedCapability = new ARDeviceCapability
            {
                SupportsAR = CheckARSupport(),
                SupportsDepthAPI = CheckDepthAPISupport(),
                RAMGB = SystemInfo.systemMemorySize / 1024,
                GPUMemoryMB = SystemInfo.graphicsMemorySize,
                HasGyroscope = SystemInfo.supportsGyroscope,
                ProcessorCount = SystemInfo.processorCount,
                SupportsVulkan = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf)
            };

            detectedTier = ClassifyDeviceTier(cachedCapability);
            isInitialized = true;

            Debug.Log($"[DeviceCompat] Tier: {detectedTier} | RAM: {cachedCapability.RAMGB}GB | " +
                     $"GPU Mem: {cachedCapability.GPUMemoryMB}MB | AR: {cachedCapability.SupportsAR} | " +
                     $"Depth: {cachedCapability.SupportsDepthAPI}");
        }

        /// <summary>
        /// Check if device supports AR
        /// </summary>
        private bool CheckARSupport()
        {
#if UNITY_ANDROID
            // ARCore support check
            return UnityEngine.XR.ARCore.ARCoreLoader != null ||
                   UnityEngine.XR.Management.XRGeneralSettings.Instance?.Manager?.activeLoader != null;
#elif UNITY_IOS
            return true; // ARKit supported on iOS 11+
#else
            return false;
#endif
        }

        /// <summary>
        /// Check if device supports Depth API
        /// </summary>
        private bool CheckDepthAPISupport()
        {
#if UNITY_ANDROID
            // Depth API requires Android 10+ and specific ARCore version
            float androidVersion = 0f;
            float.TryParse(SystemInfo.operatingSystem.Split(' ')[^1], out androidVersion);
            return androidVersion >= 10f && cachedCapability.RAMGB >= 4;
#elif UNITY_IOS
            // LiDAR required for iOS depth
            return false; // Simplified check
#else
            return false;
#endif
        }

        /// <summary>
        /// Classify device into tier based on capabilities
        /// Referência: OPT-001
        /// </summary>
        private DeviceTier ClassifyDeviceTier(ARDeviceCapability capability)
        {
            int score = 0;

            // RAM scoring
            if (capability.RAMGB >= 6) score += 3;
            else if (capability.RAMGB >= 4) score += 2;
            else score += 1;

            // GPU memory scoring
            if (capability.GPUMemoryMB >= 2048) score += 3;
            else if (capability.GPUMemoryMB >= 1024) score += 2;
            else score += 1;

            // Depth API bonus
            if (capability.SupportsDepthAPI) score += 2;

            // Processor count
            if (capability.ProcessorCount >= 8) score += 2;
            else if (capability.ProcessorCount >= 4) score += 1;

            // Classify
            if (score >= 8) return DeviceTier.High;
            if (score >= 5) return DeviceTier.Medium;
            return DeviceTier.Low;
        }

        /// <summary>
        /// Get recommended settings for detected tier
        /// Referência: OPT-004
        /// </summary>
        public TierSettings GetRecommendedSettings()
        {
            return detectedTier switch
            {
                DeviceTier.High => new TierSettings
                {
                    GraphicsQuality = 3,
                    ParticleBudget = 100,
                    EnableOcclusion = true,
                    EnableShadows = true,
                    TargetFPS = 60,
                    TextureQuality = 0 // Full res
                },
                DeviceTier.Medium => new TierSettings
                {
                    GraphicsQuality = 2,
                    ParticleBudget = 50,
                    EnableOcclusion = true,
                    EnableShadows = false,
                    TargetFPS = 45,
                    TextureQuality = 1 // Half res
                },
                DeviceTier.Low => new TierSettings
                {
                    GraphicsQuality = 0,
                    ParticleBudget = 20,
                    EnableOcclusion = false,
                    EnableShadows = false,
                    TargetFPS = 30,
                    TextureQuality = 2 // Quarter res
                },
                _ => new TierSettings
                {
                    GraphicsQuality = 1,
                    ParticleBudget = 30,
                    EnableOcclusion = false,
                    EnableShadows = false,
                    TargetFPS = 30,
                    TextureQuality = 2
                }
            };
        }

        /// <summary>
        /// Check if a specific feature is supported
        /// </summary>
        public bool IsFeatureSupported(ARFeature feature)
        {
            return feature switch
            {
                ARFeature.BasicAR => cachedCapability.SupportsAR,
                ARFeature.DepthOcclusion => cachedCapability.SupportsDepthAPI,
                ARFeature.LightEstimation => cachedCapability.SupportsAR,
                ARFeature.CloudAnchors => cachedCapability.SupportsAR && cachedCapability.RAMGB >= 4,
                ARFeature.HighQualityParticles => detectedTier == DeviceTier.High,
                ARFeature.AdvancedShadows => detectedTier == DeviceTier.High,
                _ => false
            };
        }

        /// <summary>
        /// Get compatibility report for debugging
        /// </summary>
        public string GetCompatibilityReport()
        {
            return $"=== Device Compatibility Report ===\n" +
                   $"Tier: {detectedTier}\n" +
                   $"AR Support: {cachedCapability.SupportsAR}\n" +
                   $"Depth API: {cachedCapability.SupportsDepthAPI}\n" +
                   $"RAM: {cachedCapability.RAMGB} GB\n" +
                   $"GPU Memory: {cachedCapability.GPUMemoryMB} MB\n" +
                   $"Gyroscope: {cachedCapability.HasGyroscope}\n" +
                   $"Processor Count: {cachedCapability.ProcessorCount}\n" +
                   $"Vulkan: {cachedCapability.SupportsVulkan}\n" +
                   $"OS: {SystemInfo.operatingSystem}\n" +
                   $"Device: {SystemInfo.deviceModel}";
        }
    }

    /// <summary>
    /// AR features that may or may not be supported
    /// </summary>
    public enum ARFeature
    {
        BasicAR,
        DepthOcclusion,
        LightEstimation,
        CloudAnchors,
        HighQualityParticles,
        AdvancedShadows
    }

    /// <summary>
    /// Tier-specific settings
    /// Referência: OPT-004
    /// </summary>
    [System.Serializable]
    public class TierSettings
    {
        public int GraphicsQuality;
        public int ParticleBudget;
        public bool EnableOcclusion;
        public bool EnableShadows;
        public int TargetFPS;
        public int TextureQuality;
    }

    /// <summary>
    /// Extended device capability with more details
    /// </summary>
    [System.Serializable]
    public struct ARDeviceCapability
    {
        public bool SupportsAR;
        public bool SupportsDepthAPI;
        public int RAMGB;
        public int GPUMemoryMB;
        public bool HasGyroscope;
        public int ProcessorCount;
        public bool SupportsVulkan;
    }
}
