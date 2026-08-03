using UnityEngine;
using VContainer;

namespace BreachAR.Core
{
    /// <summary>
    /// Detects device performance tier for optimization
    /// </summary>
    public class DeviceTierDetector : MonoBehaviour
    {

        [Header("Detected Values")]
        [SerializeField] private DeviceTier detectedTier;
        [SerializeField] private int ramGB;
        [SerializeField] private int gpuMemoryMB;
        [SerializeField] private bool supportsDepthAPI;

        public DeviceTier DetectedTier => detectedTier;
        public int RAMGB => ramGB;
        public int GPUMemoryMB => gpuMemoryMB;
        public bool SupportsDepthAPI => supportsDepthAPI;

        [Inject]
        private void Initialize()
        {
            DetectTier();
        }

        /// <summary>
        /// Detect device tier
        /// </summary>
        public void DetectTier()
        {
            ramGB = SystemInfo.systemMemorySize / 1024;
            gpuMemoryMB = SystemInfo.graphicsMemorySize;
            supportsDepthAPI = CheckDepthAPISupport();

            // Simple heuristic for tier detection
            if (ramGB >= 6 && gpuMemoryMB >= 2048 && supportsDepthAPI)
            {
                detectedTier = DeviceTier.High;
            }
            else if (ramGB >= 4 && gpuMemoryMB >= 1024)
            {
                detectedTier = DeviceTier.Medium;
            }
            else
            {
                detectedTier = DeviceTier.Low;
            }

            Debug.Log($"[DeviceTier] Detected: {detectedTier} (RAM: {ramGB}GB, GPU: {gpuMemoryMB}MB, Depth: {supportsDepthAPI})");
        }

        /// <summary>
        /// Check if device supports Depth API
        /// </summary>
        private bool CheckDepthAPISupport()
        {
            // TODO: Check ARCore Depth API support
            // This would use ARCore's ArCoreExtensions or similar
            return false; // Default to false until AR Foundation is set up
        }

        /// <summary>
        /// Get quality settings for detected tier
        /// </summary>
        public QualitySettings GetQualitySettings()
        {
            switch (detectedTier)
            {
                case DeviceTier.High:
                    return new QualitySettings
                    {
                        ParticleBudget = 500,
                        UseAdvancedOcclusion = true,
                        ShadowQuality = ShadowQuality.High,
                        TextureQuality = TextureQuality.FullRes,
                        TargetFPS = 60
                    };

                case DeviceTier.Medium:
                    return new QualitySettings
                    {
                        ParticleBudget = 250,
                        UseAdvancedOcclusion = true,
                        ShadowQuality = ShadowQuality.Medium,
                        TextureQuality = TextureQuality.HalfRes,
                        TargetFPS = 45
                    };

                case DeviceTier.Low:
                default:
                    return new QualitySettings
                    {
                        ParticleBudget = 100,
                        UseAdvancedOcclusion = false,
                        ShadowQuality = ShadowQuality.Low,
                        TextureQuality = TextureQuality.QuarterRes,
                        TargetFPS = 30
                    };
            }
        }
    }

    /// <summary>
    /// Device performance tiers
    /// </summary>
    public enum DeviceTier
    {
        Low,
        Medium,
        High
    }

    /// <summary>
    /// Quality settings for a device tier
    /// </summary>
    [System.Serializable]
    public class QualitySettings
    {
        public int ParticleBudget = 250;
        public bool UseAdvancedOcclusion = true;
        public ShadowQuality ShadowQuality = ShadowQuality.Medium;
        public TextureQuality TextureQuality = TextureQuality.HalfRes;
        public int TargetFPS = 45;
    }

    /// <summary>
    /// Shadow quality levels
    /// </summary>
    public enum ShadowQuality
    {
        Off,
        Low,
        Medium,
        High
    }

    /// <summary>
    /// Texture quality levels
    /// </summary>
    public enum TextureQuality
    {
        QuarterRes,
        HalfRes,
        FullRes
    }
}
