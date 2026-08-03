using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.Analytics
{
    /// <summary>
    /// Collects performance telemetry in production
    /// Referência: OPT-027
    /// </summary>
    public class PerformanceTelemetryService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float telemetryInterval = 60f;
        [SerializeField] private int maxEventsPerBatch = 50;
        [SerializeField] private bool enableTelemetry = true;

        [Header("FPS Tracking")]
        [SerializeField] private float fpsUpdateInterval = 0.5f;
        [SerializeField] private int fpsHistorySize = 120;

        [Inject] private SupabaseService supabaseService;

        private Queue<FPSData> fpsHistory = new Queue<FPSData>();
        private List<PerformanceEvent> pendingEvents = new List<PerformanceEvent>();
        private float lastTelemetryTime;
        private float lastFPSTime;
        private float currentFPS;
        private float avgFPS;
        private float minFPS = float.MaxValue;
        private float maxFPS;
        private int frameCount;
        private float frameTimeAccumulator;

        public float CurrentFPS => currentFPS;
        public float AvgFPS => avgFPS;
        public float MinFPS => minFPS;
        public int PendingEvents => pendingEvents.Count;

        private void Start()
        {
            lastTelemetryTime = Time.time;
            lastFPSTime = Time.time;
            StartCoroutine(TelemetryLoop());
        }

        private void Update()
        {
            if (!enableTelemetry) return;

            // Track FPS
            TrackFPS();

            // Track frame spikes
            if (Time.unscaledDeltaTime > 0.033f) // >33ms (below 30 FPS)
            {
                RecordEvent(new PerformanceEvent
                {
                    EventType = "frame_spike",
                    Value = Time.unscaledDeltaTime * 1000f,
                    Timestamp = DateTime.UtcNow.ToString("o")
                });
            }
        }

        /// <summary>
        /// Track FPS metrics
        /// </summary>
        private void TrackFPS()
        {
            frameCount++;
            frameTimeAccumulator += Time.unscaledDeltaTime;

            if (Time.time - lastFPSTime >= fpsUpdateInterval)
            {
                currentFPS = frameCount / frameTimeAccumulator;
                avgFPS = (avgFPS * 0.95f) + (currentFPS * 0.05f); // Smoothed average

                if (currentFPS < minFPS) minFPS = currentFPS;
                if (currentFPS > maxFPS) maxFPS = currentFPS;

                // Add to history
                fpsHistory.Enqueue(new FPSData
                {
                    Timestamp = Time.time,
                    FPS = currentFPS
                });

                while (fpsHistory.Count > fpsHistorySize)
                {
                    fpsHistory.Dequeue();
                }

                frameCount = 0;
                frameTimeAccumulator = 0f;
                lastFPSTime = Time.time;
            }
        }

        /// <summary>
        /// Record a performance event
        /// </summary>
        public void RecordEvent(PerformanceEvent performanceEvent)
        {
            if (!enableTelemetry) return;

            pendingEvents.Add(performanceEvent);

            if (pendingEvents.Count >= maxEventsPerBatch)
            {
                StartCoroutine(FlushEvents());
            }
        }

        /// <summary>
        /// Record GC allocation spike
        /// </summary>
        public void RecordGCAllocation(long bytes)
        {
            RecordEvent(new PerformanceEvent
            {
                EventType = "gc_allocation",
                Value = bytes,
                Timestamp = DateTime.UtcNow.ToString("o")
            });
        }

        /// <summary>
        /// Record crash-free session end
        /// </summary>
        public void RecordSessionEnd(float duration, int score, string mode)
        {
            RecordEvent(new PerformanceEvent
            {
                EventType = "session_end",
                Value = duration,
                Metadata = $"score={score},mode={mode}",
                Timestamp = DateTime.UtcNow.ToString("o")
            });

            // Flush immediately for session data
            StartCoroutine(FlushEvents());
        }

        /// <summary>
        /// Record error/crash
        /// </summary>
        public void RecordError(string errorType, string message)
        {
            RecordEvent(new PerformanceEvent
            {
                EventType = "error",
                Metadata = $"type={errorType},msg={message}",
                Timestamp = DateTime.UtcNow.ToString("o")
            });
        }

        /// <summary>
        /// Telemetry send loop
        /// </summary>
        private IEnumerator TelemetryLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(telemetryInterval);

                if (pendingEvents.Count > 0)
                {
                    yield return FlushEvents();
                }
            }
        }

        /// <summary>
        /// Flush pending events to server
        /// </summary>
        private IEnumerator FlushEvents()
        {
            if (pendingEvents.Count == 0) yield break;
            if (supabaseService == null || !supabaseService.IsAuthenticated)
            {
                Debug.LogWarning("[Telemetry] Not authenticated, skipping flush");
                yield break;
            }

            var batch = new List<PerformanceEvent>(pendingEvents);
            pendingEvents.Clear();

            // Create telemetry payload
            var payload = new TelemetryPayload
            {
                device_info = GetDeviceInfo(),
                session_id = SystemInfo.deviceUniqueIdentifier,
                events = batch.ToArray(),
                fps_summary = new FPSSummary
                {
                    avg_fps = avgFPS,
                    min_fps = minFPS,
                    max_fps = maxFPS,
                    sample_count = fpsHistory.Count
                }
            };

            string json = JsonUtility.ToJson(payload);
            var data = new Dictionary<string, object>
            {
                { "payload", json }
            };

            yield return supabaseService.SaveData("performance_telemetry", null, data);

            Debug.Log($"[Telemetry] Flushed {batch.Count} events");
        }

        /// <summary>
        /// Get device info for telemetry
        /// </summary>
        private DeviceInfo GetDeviceInfo()
        {
            return new DeviceInfo
            {
                model = SystemInfo.deviceModel,
                os = SystemInfo.operatingSystem,
                ram_mb = SystemInfo.systemMemorySize / 1024,
                gpu = SystemInfo.graphicsDeviceName,
                gpu_memory_mb = SystemInfo.graphicsMemorySize,
                cpu = SystemInfo.processorType,
                cpu_cores = SystemInfo.processorCount
            };
        }

        /// <summary>
        /// Get FPS summary for reporting
        /// </summary>
        public FPSSummary GetFPSSummary()
        {
            return new FPSSummary
            {
                avg_fps = avgFPS,
                min_fps = minFPS,
                max_fps = maxFPS,
                sample_count = fpsHistory.Count
            };
        }
    }

    /// <summary>
    /// Performance telemetry event
    /// </summary>
    [System.Serializable]
    public class PerformanceEvent
    {
        public string EventType;
        public float Value;
        public string Metadata;
        public string Timestamp;
    }

    /// <summary>
    /// FPS tracking data point
    /// </summary>
    [System.Serializable]
    public struct FPSData
    {
        public float Timestamp;
        public float FPS;
    }

    /// <summary>
    /// FPS summary statistics
    /// </summary>
    [System.Serializable]
    public struct FPSSummary
    {
        public float avg_fps;
        public float min_fps;
        public float max_fps;
        public int sample_count;
    }

    /// <summary>
    /// Device information for telemetry
    /// </summary>
    [System.Serializable]
    public struct DeviceInfo
    {
        public string model;
        public string os;
        public int ram_mb;
        public string gpu;
        public int gpu_memory_mb;
        public string cpu;
        public int cpu_cores;
    }

    /// <summary>
    /// Telemetry payload
    /// </summary>
    [System.Serializable]
    public class TelemetryPayload
    {
        public DeviceInfo device_info;
        public string session_id;
        public PerformanceEvent[] events;
        public FPSSummary fps_summary;
    }
}
