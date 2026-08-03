using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// Monitors and validates memory consumption
    /// Referência: OPT-025
    /// </summary>
    public class MemoryProfilerService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float monitoringInterval = 5f;
        [SerializeField] private int historySize = 60;
        [SerializeField] private bool enableMonitoring = true;

        [Header("Thresholds")]
        [SerializeField] private long warningThresholdMB = 400;
        [SerializeField] private long criticalThresholdMB = 600;
        [SerializeField] private long oomThresholdMB = 800;

        private Queue<MemorySnapshot> memoryHistory = new Queue<MemorySnapshot>();
        private float lastMonitoringTime;
        private long peakMemory;
        private bool isMonitoring;

        public long CurrentMemoryMB => GetTotalMemoryMB();
        public long PeakMemoryMB => peakMemory / (1024 * 1024);
        public MemoryStatus CurrentStatus => GetMemoryStatus();

        /// <summary>
        /// Event raised when memory threshold is exceeded
        /// </summary>
        public event Action<MemoryAlert> OnMemoryAlert;

        private void Start()
        {
            if (enableMonitoring)
            {
                StartCoroutine(MonitoringLoop());
            }
        }

        /// <summary>
        /// Memory monitoring loop
        /// </summary>
        private IEnumerator MonitoringLoop()
        {
            isMonitoring = true;

            while (isMonitoring)
            {
                yield return new WaitForSeconds(monitoringInterval);
                TakeSnapshot();
            }
        }

        /// <summary>
        /// Take a memory snapshot
        /// </summary>
        public void TakeSnapshot()
        {
            var snapshot = new MemorySnapshot
            {
                Timestamp = Time.time,
                TotalMemoryMB = GetTotalMemoryMB(),
                GCHeapMB = GetGCHeapMB(),
                NativeMemoryMB = GetNativeMemoryMB(),
                TextureMemoryMB = GetTextureMemoryMB(),
                AudioMemoryMB = GetAudioMemoryMB(),
                MeshMemoryMB = GetMeshMemoryMB()
            };

            // Update history
            memoryHistory.Enqueue(snapshot);
            while (memoryHistory.Count > historySize)
            {
                memoryHistory.Dequeue();
            }

            // Update peak
            long currentBytes = snapshot.TotalMemoryMB * 1024 * 1024;
            if (currentBytes > peakMemory)
            {
                peakMemory = currentBytes;
            }

            // Check thresholds
            CheckThresholds(snapshot);
        }

        /// <summary>
        /// Check memory thresholds and raise alerts
        /// </summary>
        private void CheckThresholds(MemorySnapshot snapshot)
        {
            if (snapshot.TotalMemoryMB >= oomThresholdMB)
            {
                OnMemoryAlert?.Invoke(new MemoryAlert
                {
                    Level = AlertLevel.Critical,
                    Message = $"Memory at OOM risk: {snapshot.TotalMemoryMB}MB",
                    CurrentMB = snapshot.TotalMemoryMB,
                    ThresholdMB = oomThresholdMB
                });

                // Force garbage collection
                Debug.LogWarning("[Memory] OOM risk - forcing GC");
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
            else if (snapshot.TotalMemoryMB >= criticalThresholdMB)
            {
                OnMemoryAlert?.Invoke(new MemoryAlert
                {
                    Level = AlertLevel.Critical,
                    Message = $"Memory usage critical: {snapshot.TotalMemoryMB}MB",
                    CurrentMB = snapshot.TotalMemoryMB,
                    ThresholdMB = criticalThresholdMB
                });
            }
            else if (snapshot.TotalMemoryMB >= warningThresholdMB)
            {
                OnMemoryAlert?.Invoke(new MemoryAlert
                {
                    Level = AlertLevel.Warning,
                    Message = $"Memory usage high: {snapshot.TotalMemoryMB}MB",
                    CurrentMB = snapshot.TotalMemoryMB,
                    ThresholdMB = warningThresholdMB
                });
            }
        }

        /// <summary>
        /// Get total memory usage in MB
        /// </summary>
        private long GetTotalMemoryMB()
        {
            return GetTotalMemoryBytes() / (1024 * 1024);
        }

        /// <summary>
        /// Get total memory usage in bytes
        /// </summary>
        public long GetTotalMemoryBytes()
        {
            return UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
        }

        /// <summary>
        /// Get GC heap size in MB
        /// </summary>
        private long GetGCHeapMB()
        {
            return GC.GetTotalMemory(false) / (1024 * 1024);
        }

        /// <summary>
        /// Get native memory usage in MB (estimated)
        /// </summary>
        private long GetNativeMemoryMB()
        {
            return UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
        }

        /// <summary>
        /// Get texture memory usage in MB
        /// </summary>
        private long GetTextureMemoryMB()
        {
            return UnityEngine.Profiling.Profiler.GetAllocatedMemoryForGraphicsDriver() / (1024 * 1024);
        }

        /// <summary>
        /// Get audio memory usage in MB (estimated)
        /// </summary>
        private long GetAudioMemoryMB()
        {
            // Rough estimate based on active audio sources
            int audioSources = FindObjectsOfType<AudioSource>().Length;
            return audioSources * 2; // ~2MB per audio source estimate
        }

        /// <summary>
        /// Get mesh memory usage in MB (estimated)
        /// </summary>
        private long GetMeshMemoryMB()
        {
            var meshes = FindObjectsOfType<MeshFilter>();
            long totalVertices = 0;

            foreach (var mf in meshes)
            {
                if (mf.sharedMesh != null)
                {
                    totalVertices += mf.sharedMesh.vertexCount;
                }
            }

            // Rough estimate: 32 bytes per vertex
            return (totalVertices * 32) / (1024 * 1024);
        }

        /// <summary>
        /// Get current memory status
        /// </summary>
        private MemoryStatus GetMemoryStatus()
        {
            long currentMB = GetTotalMemoryMB();

            if (currentMB >= oomThresholdMB) return MemoryStatus.Critical;
            if (currentMB >= criticalThresholdMB) return MemoryStatus.High;
            if (currentMB >= warningThresholdMB) return MemoryStatus.Moderate;
            return MemoryStatus.Normal;
        }

        /// <summary>
        /// Get memory statistics
        /// </summary>
        public MemoryStats GetStats()
        {
            return new MemoryStats
            {
                CurrentMB = GetTotalMemoryMB(),
                PeakMB = peakMemory / (1024 * 1024),
                GCHeapMB = GetGCHeapMB(),
                NativeMB = GetNativeMemoryMB(),
                TextureMB = GetTextureMemoryMB(),
                Status = GetMemoryStatus(),
                GCCount = GC.CollectionCount(0) + GC.CollectionCount(1) + GC.CollectionCount(2)
            };
        }

        /// <summary>
        /// Get memory trend (growing, stable, decreasing)
        /// </summary>
        public MemoryTrend GetTrend()
        {
            if (memoryHistory.Count < 10) return MemoryTrend.Unknown;

            var recent = new List<MemorySnapshot>(memoryHistory);
            int halfIndex = recent.Count / 2;

            long firstHalfAvg = 0;
            long secondHalfAvg = 0;

            for (int i = 0; i < halfIndex; i++)
            {
                firstHalfAvg += recent[i].TotalMemoryMB;
            }
            firstHalfAvg /= halfIndex;

            for (int i = halfIndex; i < recent.Count; i++)
            {
                secondHalfAvg += recent[i].TotalMemoryMB;
            }
            secondHalfAvg /= (recent.Count - halfIndex);

            if (secondHalfAvg > firstHalfAvg * 1.1f) return MemoryTrend.Growing;
            if (secondHalfAvg < firstHalfAvg * 0.9f) return MemoryTrend.Decreasing;
            return MemoryTrend.Stable;
        }

        /// <summary>
        /// Force garbage collection
        /// </summary>
        public void ForceGC()
        {
            Debug.Log("[Memory] Forcing garbage collection");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            TakeSnapshot();
        }

        private void OnDestroy()
        {
            isMonitoring = false;
        }
    }

    /// <summary>
    /// Memory snapshot data
    /// </summary>
    [System.Serializable]
    public class MemorySnapshot
    {
        public float Timestamp;
        public long TotalMemoryMB;
        public long GCHeapMB;
        public long NativeMemoryMB;
        public long TextureMemoryMB;
        public long AudioMemoryMB;
        public long MeshMemoryMB;
    }

    /// <summary>
    /// Memory status levels
    /// </summary>
    public enum MemoryStatus
    {
        Normal,
        Moderate,
        High,
        Critical
    }

    /// <summary>
    /// Alert levels
    /// </summary>
    public enum AlertLevel
    {
        Info,
        Warning,
        Critical
    }

    /// <summary>
    /// Memory trend
    /// </summary>
    public enum MemoryTrend
    {
        Unknown,
        Growing,
        Stable,
        Decreasing
    }

    /// <summary>
    /// Memory alert data
    /// </summary>
    [System.Serializable]
    public class MemoryAlert
    {
        public AlertLevel Level;
        public string Message;
        public long CurrentMB;
        public long ThresholdMB;
    }

    /// <summary>
    /// Memory statistics
    /// </summary>
    [System.Serializable]
    public struct MemoryStats
    {
        public long CurrentMB;
        public long PeakMB;
        public long GCHeapMB;
        public long NativeMB;
        public long TextureMB;
        public MemoryStatus Status;
        public int GCCount;
    }
}
