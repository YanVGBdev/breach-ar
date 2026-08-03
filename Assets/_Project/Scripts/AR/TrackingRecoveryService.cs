using UnityEngine;
using System;
using VContainer;

namespace BreachAR.AR
{
    /// <summary>
    /// Handles AR tracking loss and recovery
    /// Referência: AR-022
    /// </summary>
    public class TrackingRecoveryService : MonoBehaviour
    {
        [Header("Tracking Settings")]
        [SerializeField] private float trackingLossTimeout = 5f;
        [SerializeField] private float relocalizationTimeout = 10f;
        [SerializeField] private int maxRecoveryAttempts = 3;
        [SerializeField] private float confidenceThreshold = 0.7f;

        [Header("State")]
        [SerializeField] private bool isTracking;
        [SerializeField] private TrackingQuality currentQuality;
        [SerializeField] private float lastTrackingTime;
        [SerializeField] private int recoveryAttempts;

        [Inject] private ARSessionService arSessionService;

        public bool IsTracking => isTracking;
        public TrackingQuality CurrentQuality => currentQuality;
        public float TrackingConfidence => GetTrackingConfidence();

        /// <summary>
        /// Event raised when tracking status changes
        /// </summary>
        public event Action<TrackingStatusChangedEventArgs> OnTrackingStatusChanged;

        private void Update()
        {
            UpdateTrackingStatus();
        }

        /// <summary>
        /// Update tracking status based on AR session state
        /// Referência: AR-022
        /// </summary>
        private void UpdateTrackingStatus()
        {
            if (arSessionService == null || !arSessionService.IsSessionActive)
            {
                SetTrackingState(false, TrackingQuality.Unknown);
                return;
            }

            // Check tracking state (simplified - real implementation uses ARCameraManager)
            bool wasTracking = isTracking;
            TrackingQuality previousQuality = currentQuality;

            // Simulate tracking quality based on session state
            // In real implementation, this would read from ARSession.state
            TrackingQuality newQuality = EstimateTrackingQuality();
            bool newIsTracking = newQuality != TrackingQuality.Lost;

            // Update state
            if (newIsTracking)
            {
                lastTrackingTime = Time.time;
                recoveryAttempts = 0;
            }

            SetTrackingState(newIsTracking, newQuality);

            // Detect changes
            if (wasTracking != newIsTracking || previousQuality != newQuality)
            {
                OnTrackingStatusChanged?.Invoke(new TrackingStatusChangedEventArgs
                {
                    IsTracking = newIsTracking,
                    Quality = newQuality,
                    PreviousWasTracking = wasTracking,
                    RecoveryAttempt = recoveryAttempts
                });

                Debug.Log($"[TrackingRecovery] Status changed: {(newIsTracking ? "Tracking" : "Lost")} | Quality: {newQuality}");
            }

            // Handle tracking loss
            if (!newIsTracking && wasTracking)
            {
                HandleTrackingLoss();
            }
        }

        /// <summary>
        /// Estimate tracking quality (simplified)
        /// In production, use ARCameraManager frameReceived event
        /// </summary>
        private TrackingQuality EstimateTrackingQuality()
        {
            // Simplified estimation
            // Real implementation would use:
            // - ARCameraManager.frameReceived
            // - ARSession.state
            // - ARCameraBackground.enabled
            // - Light estimation values stability

            float timeSinceLastTrack = Time.time - lastTrackingTime;

            if (timeSinceLastTrack < 0.5f)
                return TrackingQuality.Excellent;
            if (timeSinceLastTrack < 2f)
                return TrackingQuality.Good;
            if (timeSinceLastTrack < trackingLossTimeout)
                return TrackingQuality.Poor;
            
            return TrackingQuality.Lost;
        }

        /// <summary>
        /// Handle tracking loss event
        /// Referência: AR-022
        /// </summary>
        private void HandleTrackingLoss()
        {
            Debug.Log("[TrackingRecovery] Tracking lost - attempting recovery");

            if (recoveryAttempts < maxRecoveryAttempts)
            {
                recoveryAttempts++;
                StartCoroutine(AttemptRelocalization());
            }
            else
            {
                Debug.LogWarning("[TrackingRecovery] Max recovery attempts reached");
                NotifyTrackingFailed();
            }
        }

        /// <summary>
        /// Attempt to relocalize after tracking loss
        /// </summary>
        private System.Collections.IEnumerator AttemptRelocalization()
        {
            Debug.Log($"[TrackingRecovery] Relocalization attempt {recoveryAttempts}/{maxRecoveryAttempts}");

            // Wait for potential automatic recovery
            float elapsed = 0f;
            while (elapsed < relocalizationTimeout)
            {
                yield return new WaitForSeconds(0.5f);
                elapsed += 0.5f;

                // Check if tracking recovered
                if (isTracking)
                {
                    Debug.Log("[TrackingRecovery] Tracking recovered successfully");
                    yield break;
                }
            }

            // Recovery failed for this attempt
            Debug.LogWarning($"[TrackingRecovery] Relocalization attempt {recoveryAttempts} failed");
        }

        /// <summary>
        /// Notify that all recovery attempts failed
        /// </summary>
        private void NotifyTrackingFailed()
        {
            OnTrackingStatusChanged?.Invoke(new TrackingStatusChangedEventArgs
            {
                IsTracking = false,
                Quality = TrackingQuality.Lost,
                PreviousWasTracking = true,
                RecoveryAttempt = recoveryAttempts,
                AllAttemptsFailed = true
            });
        }

        /// <summary>
        /// Get tracking confidence score (0-1)
        /// </summary>
        public float GetTrackingConfidence()
        {
            return currentQuality switch
            {
                TrackingQuality.Excellent => 1.0f,
                TrackingQuality.Good => 0.85f,
                TrackingQuality.Poor => 0.5f,
                TrackingQuality.Lost => 0.0f,
                _ => 0.0f
            };
        }

        /// <summary>
        /// Force manual relocalization request
        /// Referência: AR-015 (Rescan)
        /// </summary>
        public void RequestRelocalization()
        {
            recoveryAttempts = 0;
            StartCoroutine(AttemptRelocalization());
        }

        /// <summary>
        /// Set tracking state
        /// </summary>
        private void SetTrackingState(bool tracking, TrackingQuality quality)
        {
            isTracking = tracking;
            currentQuality = quality;
        }

        /// <summary>
        /// Reset for new session
        /// </summary>
        public void ResetSession()
        {
            isTracking = false;
            currentQuality = TrackingQuality.Unknown;
            lastTrackingTime = Time.time;
            recoveryAttempts = 0;
        }
    }

    /// <summary>
    /// Tracking quality levels
    /// </summary>
    public enum TrackingQuality
    {
        Unknown,
        Excellent,
        Good,
        Poor,
        Lost
    }

    /// <summary>
    /// Event data for tracking status changes
    /// </summary>
    [System.Serializable]
    public class TrackingStatusChangedEventArgs : EventArgs
    {
        public bool IsTracking;
        public TrackingQuality Quality;
        public bool PreviousWasTracking;
        public int RecoveryAttempt;
        public bool AllAttemptsFailed;
    }
}
