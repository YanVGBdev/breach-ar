using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace BreachAR.Utils
{
    /// <summary>
    /// Utilities for optimizing build size
    /// Referência: OPT-015
    /// </summary>
    public class BuildSizeOptimizer : MonoBehaviour
    {
        [Header("Texture Settings")]
        [SerializeField] private int maxTextureSize = 1024;
        [SerializeField] private int mobileTextureSize = 512;
        [SerializeField] private bool forceASTCCompression = true;

        [Header("Mesh Settings")]
        [SerializeField] private int maxMeshVertices = 5000;
        [SerializeField] private bool stripBlendShapes = false;

        [Header("Audio Settings")]
        [SerializeField] private bool streamAudioInBackground = true;
        [SerializeField] private int audioLoadType = 1; // 0=Decompress, 1=Streaming, 2=Compressed

        /// <summary>
        /// Get recommended texture size for device tier
        /// </summary>
        public int GetRecommendedTextureSize(Core.DeviceTier tier)
        {
            return tier switch
            {
                Core.DeviceTier.High => maxTextureSize,
                Core.DeviceTier.Medium => mobileTextureSize,
                Core.DeviceTier.Low => mobileTextureSize / 2,
                _ => mobileTextureSize
            };
        }

        /// <summary>
        /// Analyze and report potential size savings
        /// NOTE: This method uses Resources.FindObjectsOfTypeAll which is expensive.
        /// Only use in Editor or development builds for analysis purposes.
        /// </summary>
        public BuildSizeReport AnalyzeBuildSize()
        {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
            Debug.LogWarning("[BuildSizeOptimizer] AnalyzeBuildSize should only be used in Editor/Development");
            return new BuildSizeReport { Timestamp = System.DateTime.UtcNow.ToString("o") };
#endif

            var report = new BuildSizeReport
            {
                Timestamp = System.DateTime.UtcNow.ToString("o")
            };

            // Analyze textures
            var textures = Resources.FindObjectsOfTypeAll<Texture2D>();
            long totalTextureSize = 0;
            int oversizedTextures = 0;

            foreach (var tex in textures)
            {
                long texSize = (long)tex.width * tex.height * 4; // Rough estimate
                totalTextureSize += texSize;

                if (tex.width > maxTextureSize || tex.height > maxTextureSize)
                {
                    oversizedTextures++;
                }
            }

            report.TextureCount = textures.Length;
            report.EstimatedTextureSizeMB = totalTextureSize / (1024 * 1024);
            report.OversizedTextures = oversizedTextures;

            // Analyze meshes
            var meshes = Resources.FindObjectsOfTypeAll<Mesh>();
            int highPolyMeshes = 0;

            foreach (var mesh in meshes)
            {
                if (mesh.vertexCount > maxMeshVertices)
                {
                    highPolyMeshes++;
                }
            }

            report.MeshCount = meshes.Length;
            report.HighPolyMeshes = highPolyMeshes;

            // Analyze audio
            var audioClips = Resources.FindObjectsOfTypeAll<AudioClip>();
            long totalAudioSize = 0;

            foreach (var clip in audioClips)
            {
                // Rough estimate based on length and channels
                totalAudioSize += (long)(clip.length * clip.channels * 44100 * 2);
            }

            report.AudioClipCount = audioClips.Length;
            report.EstimatedAudioSizeMB = totalAudioSize / (1024 * 1024);

            // Calculate recommendations
            report.Recommendations = GenerateRecommendations(report);

            return report;
        }

        /// <summary>
        /// Generate optimization recommendations
        /// </summary>
        private List<string> GenerateRecommendations(BuildSizeReport report)
        {
            var recommendations = new List<string>();

            if (report.OversizedTextures > 0)
            {
                recommendations.Add($"Resize {report.OversizedTextures} oversized textures to {maxTextureSize}px max");
            }

            if (report.HighPolyMeshes > 0)
            {
                recommendations.Add($"Simplify {report.HighPolyMeshes} high-poly meshes (> {maxMeshVertices} vertices)");
            }

            if (report.EstimatedTextureSizeMB > 50)
            {
                recommendations.Add("Enable ASTC texture compression for mobile");
                recommendations.Add("Use texture atlases to reduce draw calls");
            }

            if (report.EstimatedAudioSizeMB > 20)
            {
                recommendations.Add("Enable audio streaming for music tracks");
                recommendations.Add("Compress SFX audio files (Vorbis quality 0.3-0.5)");
            }

            return recommendations;
        }

        /// <summary>
        /// Get compression format recommendation
        /// </summary>
        public TextureCompressionFormat GetRecommendedCompression(Core.DeviceTier tier)
        {
            return tier switch
            {
                Core.DeviceTier.High => TextureCompressionFormat.ASTC_4x4,
                Core.DeviceTier.Medium => TextureCompressionFormat.ASTC_6x6,
                Core.DeviceTier.Low => TextureCompressionFormat.ASTC_8x8,
                _ => TextureCompressionFormat.ASTC_6x6
            };
        }
    }

    /// <summary>
    /// Build size analysis report
    /// </summary>
    [System.Serializable]
    public class BuildSizeReport
    {
        public string Timestamp;
        public int TextureCount;
        public long EstimatedTextureSizeMB;
        public int OversizedTextures;
        public int MeshCount;
        public int HighPolyMeshes;
        public int AudioClipCount;
        public long EstimatedAudioSizeMB;
        public List<string> Recommendations;
    }

    /// <summary>
    /// Texture compression formats
    /// </summary>
    public enum TextureCompressionFormat
    {
        ASTC_4x4,
        ASTC_6x6,
        ASTC_8x8,
        ETC2,
        PVRTC
    }
}
