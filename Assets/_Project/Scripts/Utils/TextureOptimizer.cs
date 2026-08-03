using UnityEngine;
using System.Collections.Generic;
using BreachAR.Core;

namespace BreachAR.Utils
{
    /// <summary>
    /// OPT-009: Gerencia compressão e otimização de texturas para mobile.
    /// Suporta ASTC, ETC2 e formato por tier de device.
    /// </summary>
    public class TextureOptimizer : MonoBehaviour
    {
        [Header("Compression Settings")]
        [SerializeField] private TextureFormat highTierFormat = TextureFormat.ASTC_4x4;
        [SerializeField] private TextureFormat mediumTierFormat = TextureFormat.ASTC_6x6;
        [SerializeField] private TextureFormat lowTierFormat = TextureFormat.ASTC_8x8;
        
        [Header("Size Limits (KB)")]
        [SerializeField] private int maxTextureSizeHigh = 2048;
        [SerializeField] private int maxTextureSizeMedium = 1024;
        [SerializeField] private int maxTextureSizeLow = 512;
        
        [Header("Mipmap Settings")]
        [SerializeField] private bool enableMipmaps = true;
        [SerializeField] private int maxMipMapLevel = 3;
        
        private Dictionary<string, TextureInfo> textureRegistry = new Dictionary<string, TextureInfo>();
        private int totalTextures;
        private long totalMemoryBytes;
        
        private struct TextureInfo
        {
            public Texture Texture;
            public TextureFormat OriginalFormat;
            public int OriginalSize;
            public TextureFormat OptimizedFormat;
            public long MemoryBefore;
            public long MemoryAfter;
        }
        
        /// <summary>
        /// Get the appropriate max texture size for a device tier.
        /// </summary>
        public int GetMaxTextureSizeForTier(DeviceTier tier)
        {
            return tier switch
            {
                DeviceTier.High => maxTextureSizeHigh,
                DeviceTier.Medium => maxTextureSizeMedium,
                DeviceTier.Low => maxTextureSizeLow,
                _ => maxTextureSizeMedium
            };
        }
        
        /// <summary>
        /// Get the appropriate texture format for a device tier.
        /// </summary>
        public TextureFormat GetOptimalFormat(DeviceTier tier)
        {
            return tier switch
            {
                DeviceTier.High => highTierFormat,
                DeviceTier.Medium => mediumTierFormat,
                DeviceTier.Low => lowTierFormat,
                _ => mediumTierFormat
            };
        }
        
        /// <summary>
        /// Calculate memory usage of a texture.
        /// </summary>
        public long CalculateTextureMemory(Texture2D texture)
        {
            if (texture == null) return 0;
            
            int width = texture.width;
            int height = texture.height;
            int mipCount = texture.mipmapCount;
            
            long memory = 0;
            for (int i = 0; i < Mathf.Min(mipCount, maxMipMapLevel + 1); i++)
            {
                int mipWidth = Mathf.Max(1, width >> i);
                int mipHeight = Mathf.Max(1, height >> i);
                memory += mipWidth * mipHeight * GetBytesPerPixel(texture.format);
            }
            
            return memory;
        }
        
        private int GetBytesPerPixel(TextureFormat format)
        {
            // Note: For compressed formats (ASTC, ETC2), this is an approximate value
            // Actual compressed size varies by content, typically 0.5-1.5 bytes per pixel
            return format switch
            {
                TextureFormat.ASTC_4x4 => 1, // ~1 byte per pixel for 4x4 block
                TextureFormat.ASTC_6x6 => 1, // ~0.56 bytes per pixel for 6x6 block
                TextureFormat.ASTC_8x8 => 1, // ~0.31 bytes per pixel for 8x8 block
                TextureFormat.ETC2_RGBA8 => 1, // ~1 byte per pixel
                TextureFormat.RGBA32 => 4,
                TextureFormat.RGB24 => 3,
                TextureFormat.RGBAFloat => 16,
                _ => 4
            };
        }
        
        /// <summary>
        /// Optimize a texture for the given device tier.
        /// </summary>
        public Texture2D OptimizeTexture(Texture2D source, DeviceTier tier)
        {
            if (source == null) return null;
            
            int maxSize = GetMaxTextureSizeForTier(tier);
            TextureFormat targetFormat = GetOptimalFormat(tier);
            
            // Calculate target size
            int targetWidth = Mathf.Min(source.width, maxSize);
            int targetHeight = Mathf.Min(source.height, maxSize);
            
            // Maintain aspect ratio
            float aspect = (float)source.width / source.height;
            if (source.width > source.height)
            {
                targetHeight = Mathf.RoundToInt(targetWidth / aspect);
            }
            else
            {
                targetWidth = Mathf.RoundToInt(targetHeight * aspect);
            }
            
            // Resize if needed
            Texture2D optimized = source;
            if (targetWidth != source.width || targetHeight != source.height)
            {
                optimized = new Texture2D(targetWidth, targetHeight, targetFormat, enableMipmaps);
                
                // Use Graphics.CopyTexture for better quality
                Graphics.CopyTexture(source, 0, 0, optimized, 0, 0);
            }
            else if (source.format != targetFormat)
            {
                // Reformat without resizing
                var readable = GetReadableTexture(source);
                optimized = new Texture2D(source.width, source.height, targetFormat, enableMipmaps);
                optimized.SetPixels(readable.GetPixels());
                optimized.Apply();
            }
            
            // Register for tracking
            string key = source.GetInstanceID().ToString();
            if (!textureRegistry.ContainsKey(key))
            {
                textureRegistry[key] = new TextureInfo
                {
                    Texture = source,
                    OriginalFormat = source.format,
                    OriginalSize = source.width * source.height * GetBytesPerPixel(source.format),
                    OptimizedFormat = targetFormat,
                    MemoryBefore = CalculateTextureMemory(source),
                    MemoryAfter = CalculateTextureMemory(optimized)
                };
                
                totalTextures++;
                totalMemoryBytes += textureRegistry[key].MemoryAfter;
            }
            
            return optimized;
        }
        
        private Texture2D GetReadableTexture(Texture2D source)
        {
            RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);
            Graphics.Blit(source, temp);
            
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = temp;
            
            Texture2D readable = new Texture2D(source.width, source.height);
            readable.ReadPixels(new Rect(0, 0, source.width, source.height), 0, 0);
            readable.Apply();
            
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(temp);
            
            return readable;
        }
        
        /// <summary>
        /// Get compression statistics.
        /// </summary>
        public TextureOptimizationStats GetStats()
        {
            long totalBefore = 0;
            long totalAfter = 0;
            
            foreach (var info in textureRegistry.Values)
            {
                totalBefore += info.MemoryBefore;
                totalAfter += info.MemoryAfter;
            }
            
            return new TextureOptimizationStats
            {
                TotalTextures = totalTextures,
                TotalMemoryBefore = totalBefore,
                TotalMemoryAfter = totalAfter,
                CompressionRatio = totalBefore > 0 ? (float)totalAfter / totalBefore : 1f
            };
        }
        
        /// <summary>
        /// Check if a texture needs optimization based on size.
        /// </summary>
        public bool NeedsOptimization(Texture2D texture, DeviceTier tier)
        {
            if (texture == null) return false;
            
            int maxSize = GetMaxTextureSizeForTier(tier);
            return texture.width > maxSize || texture.height > maxSize;
        }
        
        private void OnDestroy()
        {
            textureRegistry.Clear();
        }
    }
    
    public struct TextureOptimizationStats
    {
        public int TotalTextures;
        public long TotalMemoryBefore;
        public long TotalMemoryAfter;
        public float CompressionRatio;
    }
}
