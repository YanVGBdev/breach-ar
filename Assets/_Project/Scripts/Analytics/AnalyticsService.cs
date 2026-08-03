using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Analytics
{
    /// <summary>
    /// Manages analytics event tracking
    /// Injected via VContainer DI
    /// </summary>
    public class AnalyticsService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enabled = true;
        [SerializeField] private bool requiresConsent = true;

        private bool hasConsent;
        private Queue<AnalyticsEvent> eventQueue;
        private int maxQueueSize = 100;

        private void Awake()
        {
            eventQueue = new Queue<AnalyticsEvent>();
        }

        /// <summary>
        /// Set privacy consent
        /// </summary>
        public void SetConsent(bool consent)
        {
            hasConsent = consent;
            PlayerPrefs.SetInt("AnalyticsConsent", consent ? 1 : 0);
            PlayerPrefs.Save();

            if (consent)
            {
                FlushEventQueue();
            }
        }

        /// <summary>
        /// Check if analytics is enabled
        /// </summary>
        public bool IsEnabled()
        {
            return enabled && (!requiresConsent || hasConsent);
        }

        /// <summary>
        /// Track a session start event
        /// </summary>
        public void TrackSessionStart(string deviceTier, string platform, bool depthSupported)
        {
            TrackEvent("session_start", new Dictionary<string, object>
            {
                { "device_tier", deviceTier },
                { "platform", platform },
                { "ar_depth_supported", depthSupported }
            });
        }

        /// <summary>
        /// Track AR scan completion
        /// </summary>
        public void TrackARScanCompleted(float durationSeconds, int floorCount, int wallCount, int ceilingCount, int furnitureCount)
        {
            TrackEvent("ar_scan_completed", new Dictionary<string, object>
            {
                { "duration_seconds", durationSeconds },
                { "floor_count", floorCount },
                { "wall_count", wallCount },
                { "ceiling_count", ceilingCount },
                { "furniture_count", furnitureCount }
            });
        }

        /// <summary>
        /// Track run started
        /// </summary>
        public void TrackRunStarted(string gameMode, string biomeId)
        {
            TrackEvent("run_started", new Dictionary<string, object>
            {
                { "game_mode", gameMode },
                { "biome_id", biomeId }
            });
        }

        /// <summary>
        /// Track wave events
        /// </summary>
        public void TrackWaveStarted(int waveIndex, float difficultyDelta)
        {
            TrackEvent("wave_started", new Dictionary<string, object>
            {
                { "wave_index", waveIndex },
                { "difficulty_delta", difficultyDelta }
            });
        }

        public void TrackWaveCompleted(int waveIndex, float timeTaken, float coreHpRemaining)
        {
            TrackEvent("wave_completed", new Dictionary<string, object>
            {
                { "wave_index", waveIndex },
                { "time_taken", timeTaken },
                { "core_hp_remaining", coreHpRemaining }
            });
        }

        /// <summary>
        /// Track fragment killed
        /// </summary>
        public void TrackFragmentKilled(string fragmentType, string orbType, float comboMultiplier, bool viaRicochet)
        {
            TrackEvent("fragment_killed", new Dictionary<string, object>
            {
                { "fragment_type", fragmentType },
                { "orb_type", orbType },
                { "combo_multiplier", comboMultiplier },
                { "via_ricochet", viaRicochet }
            });
        }

        /// <summary>
        /// Track rift closed
        /// </summary>
        public void TrackRiftClosed(string surfaceType)
        {
            TrackEvent("rift_closed", new Dictionary<string, object>
            {
                { "surface_type", surfaceType }
            });
        }

        /// <summary>
        /// Track power-up events
        /// </summary>
        public void TrackPowerUpCollected(string powerUpType)
        {
            TrackEvent("powerup_collected", new Dictionary<string, object>
            {
                { "powerup_type", powerUpType }
            });
        }

        /// <summary>
        /// Track run ended
        /// </summary>
        public void TrackRunEnded(string outcome, int score, float maxCombo, int wavesCleared, float duration)
        {
            TrackEvent("run_ended", new Dictionary<string, object>
            {
                { "outcome", outcome },
                { "score", score },
                { "max_combo", maxCombo },
                { "waves_cleared", wavesCleared },
                { "duration", duration }
            });
        }

        /// <summary>
        /// Track IAP purchase
        /// </summary>
        public void TrackIAPPurchase(string itemId, string priceTier, string currency)
        {
            TrackEvent("iap_purchase", new Dictionary<string, object>
            {
                { "item_id", itemId },
                { "price_tier", priceTier },
                { "currency", currency }
            });
        }

        /// <summary>
        /// Track ad watched
        /// </summary>
        public void TrackAdWatched(string placement)
        {
            TrackEvent("ad_watched", new Dictionary<string, object>
            {
                { "ad_placement", placement }
            });
        }

        /// <summary>
        /// Generic event tracking
        /// </summary>
        public void TrackEvent(string eventName, Dictionary<string, object> parameters = null)
        {
            if (!IsEnabled())
            {
                eventQueue.Enqueue(new AnalyticsEvent
                {
                    EventName = eventName,
                    Parameters = parameters,
                    Timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                });

                if (eventQueue.Count > maxQueueSize)
                {
                    eventQueue.Dequeue();
                }
                return;
            }

            SendEvent(eventName, parameters);
        }

        private void SendEvent(string eventName, Dictionary<string, object> parameters)
        {
            Debug.Log($"[Analytics] Event: {eventName}");
            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    Debug.Log($"  - {param.Key}: {param.Value}");
                }
            }
        }

        private void FlushEventQueue()
        {
            while (eventQueue.Count > 0)
            {
                var analyticsEvent = eventQueue.Dequeue();
                SendEvent(analyticsEvent.EventName, analyticsEvent.Parameters);
            }
        }
    }

    /// <summary>
    /// Analytics event data
    /// </summary>
    [System.Serializable]
    public class AnalyticsEvent
    {
        public string EventName;
        public Dictionary<string, object> Parameters;
        public long Timestamp;
    }
}
