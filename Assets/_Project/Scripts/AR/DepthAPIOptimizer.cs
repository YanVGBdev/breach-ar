using UnityEngine;
using System.Collections.Generic;
using BreachAR.Core;

namespace BreachAR.AR
{
    /// <summary>
    /// OPT-012: Otimiza o uso da Depth API baseado no tier do device.
    /// Controla frequência de sampling, resolução e qualidade.
    /// </summary>
    public class DepthAPIOptimizer : MonoBehaviour
    {
        [Header("Sampling Settings")]
        [SerializeField] private float highTierSampleRate = 30f; // 30 Hz
        [SerializeField] private float mediumTierSampleRate = 15f; // 15 Hz
        [SerializeField] private float lowTierSampleRate = 5f; // 5 Hz
        
        [Header("Resolution Settings")]
        [SerializeField] private Vector2Int highTierResolution = new Vector2Int(640, 480);
        [SerializeField] private Vector2Int mediumTierResolution = new Vector2Int(320, 240);
        [SerializeField] private Vector2Int lowTierResolution = new Vector2Int(160, 120);
        
        [Header("Quality Settings")]
        [SerializeField] [Range(0.1f, 1f)] private float highTierConfidenceThreshold = 0.5f;
        [SerializeField] [Range(0.1f, 1f)] private float mediumTierConfidenceThreshold = 0.7f;
        [SerializeField] [Range(0.1f, 1f)] private float lowTierConfidenceThreshold = 0.9f;
        
        [Header("Feature Toggles")]
        [SerializeField] private bool enableTemporalSmoothing = true;
        [SerializeField] private bool enableEdgePreservation = true;
        
        private DeviceTier currentTier = DeviceTier.Medium;
        private DeviceTier originalTier = DeviceTier.Medium;
        private float lastSampleTime;
        private float currentSampleRate;
        private Vector2Int currentResolution;
        private float currentConfidenceThreshold;
        
        private DepthSampleBuffer sampleBuffer;
        private DepthOptimizationStats stats;
        
        private struct DepthSample
        {
            public float Timestamp;
            public float[] DepthData;
            public float[] ConfidenceData;
            public int Width;
            public int Height;
        }
        
        private class DepthSampleBuffer
        {
            private readonly Queue<DepthSample> samples;
            private readonly int maxSamples;
            
            public DepthSampleBuffer(int maxSamples = 5)
            {
                this.maxSamples = maxSamples;
                samples = new Queue<DepthSample>(maxSamples);
            }
            
            public void AddSample(DepthSample sample)
            {
                while (samples.Count >= maxSamples)
                {
                    samples.Dequeue();
                }
                samples.Enqueue(sample);
            }
            
            public DepthSample[] GetAllSamples()
            {
                return samples.ToArray();
            }
            
            public DepthSample GetLatestSample()
            {
                return samples.Count > 0 ? samples.ToArray()[^1] : default;
            }
            
            public void Clear()
            {
                samples.Clear();
            }
        }
        
        private void Start()
        {
            sampleBuffer = new DepthSampleBuffer(5);
            ConfigureForTier(currentTier);
        }
        
        /// <summary>
        /// Configure optimizer for a specific device tier.
        /// </summary>
        public void ConfigureForTier(DeviceTier tier)
        {
            originalTier = tier;
            currentTier = tier;
            
            currentSampleRate = tier switch
            {
                DeviceTier.High => highTierSampleRate,
                DeviceTier.Medium => mediumTierSampleRate,
                DeviceTier.Low => lowTierSampleRate,
                _ => mediumTierSampleRate
            };
            
            currentResolution = tier switch
            {
                DeviceTier.High => highTierResolution,
                DeviceTier.Medium => mediumTierResolution,
                DeviceTier.Low => lowTierResolution,
                _ => mediumTierResolution
            };
            
            currentConfidenceThreshold = tier switch
            {
                DeviceTier.High => highTierConfidenceThreshold,
                DeviceTier.Medium => mediumTierConfidenceThreshold,
                DeviceTier.Low => lowTierConfidenceThreshold,
                _ => mediumTierConfidenceThreshold
            };
            
            Debug.Log($"[DepthAPIOptimizer] Configured for {tier} tier: " +
                     $"SampleRate={currentSampleRate}Hz, " +
                     $"Resolution={currentResolution}, " +
                     $"Confidence={currentConfidenceThreshold}");
        }
        
        /// <summary>
        /// Check if we should sample depth this frame.
        /// </summary>
        public bool ShouldSample()
        {
            if (Time.time - lastSampleTime >= 1f / currentSampleRate)
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Record that a sample was taken.
        /// </summary>
        public void RecordSample(float[] depthData, float[] confidenceData, int width, int height)
        {
            lastSampleTime = Time.time;
            
            var sample = new DepthSample
            {
                Timestamp = Time.time,
                DepthData = depthData,
                ConfidenceData = confidenceData,
                Width = width,
                Height = height
            };
            
            sampleBuffer.AddSample(sample);
            
            stats.TotalSamples++;
            stats.TotalPixelsProcessed += width * height;
        }
        
        /// <summary>
        /// Get filtered depth data using temporal smoothing.
        /// </summary>
        public float[] GetSmoothedDepth(int width, int height)
        {
            if (!enableTemporalSmoothing)
            {
                var latest = sampleBuffer.GetLatestSample();
                return latest.DepthData;
            }
            
            var samples = sampleBuffer.GetAllSamples();
            if (samples.Length == 0) return null;
            
            float[] smoothed = new float[width * height];
            float[] weights = new float[width * height];
            
            float currentTime = Time.time;
            
            foreach (var sample in samples)
            {
                if (sample.DepthData == null) continue;
                
                // More recent samples have higher weight
                float age = currentTime - sample.Timestamp;
                float weight = Mathf.Exp(-age * 2f); // Exponential decay
                
                int minLength = Mathf.Min(smoothed.Length, sample.DepthData.Length);
                for (int i = 0; i < minLength; i++)
                {
                    if (sample.ConfidenceData != null && i < sample.ConfidenceData.Length)
                    {
                        // Weight by confidence too
                        weight *= sample.ConfidenceData[i];
                    }
                    
                    smoothed[i] += sample.DepthData[i] * weight;
                    weights[i] += weight;
                }
            }
            
            // Normalize
            for (int i = 0; i < smoothed.Length; i++)
            {
                if (weights[i] > Mathf.Epsilon)
                    smoothed[i] /= weights[i];
                else
                    smoothed[i] = float.MaxValue; // Invalid depth
            }
            
            return smoothed;
        }
        
        /// <summary>
        /// Filter depth data by confidence threshold.
        /// </summary>
        public bool IsHighConfidence(float confidence)
        {
            return confidence >= currentConfidenceThreshold;
        }
        
        /// <summary>
        /// Get the current resolution for depth rendering.
        /// </summary>
        public Vector2Int GetCurrentResolution()
        {
            return currentResolution;
        }
        
        /// <summary>
        /// Get the current sample rate.
        /// </summary>
        public float GetCurrentSampleRate()
        {
            return currentSampleRate;
        }
        
        /// <summary>
        /// Get optimization statistics.
        /// </summary>
        public DepthOptimizationStats GetStats()
        {
            stats.CurrentTier = currentTier;
            stats.SampleRate = currentSampleRate;
            stats.Resolution = currentResolution;
            stats.ConfidenceThreshold = currentConfidenceThreshold;
            stats.BufferSize = sampleBuffer?.GetAllSamples().Length ?? 0;
            
            return stats;
        }
        
        /// <summary>
        /// Reduce quality temporarily for performance recovery.
        /// </summary>
        public void ReduceQuality()
        {
            DeviceTier reducedTier = currentTier switch
            {
                DeviceTier.High => DeviceTier.Medium,
                DeviceTier.Medium => DeviceTier.Low,
                DeviceTier.Low => DeviceTier.Low,
                _ => DeviceTier.Low
            };
            
            ConfigureForTier(reducedTier);
            stats.QualityReductions++;
        }
        
        /// <summary>
        /// Restore quality to the device's natural tier.
        /// </summary>
        public void RestoreQuality()
        {
            ConfigureForTier(originalTier);
        }
        
        private void OnDestroy()
        {
            sampleBuffer?.Clear();
        }
    }
    
    public struct DepthOptimizationStats
    {
        public DeviceTier CurrentTier;
        public float SampleRate;
        public Vector2Int Resolution;
        public float ConfidenceThreshold;
        public int BufferSize;
        public long TotalSamples;
        public long TotalPixelsProcessed;
        public int QualityReductions;
    }
}
