using UnityEngine;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.Backend
{
    /// <summary>
    /// Rate limiter for API calls
    /// </summary>
    public class RateLimiter : MonoBehaviour
    {

        [Header("Rate Limits")]
        [SerializeField] private int scoreSubmissionLimit = 10; // per minute
        [SerializeField] private int leaderboardRequestLimit = 30; // per minute
        [SerializeField] private int generalApiLimit = 60; // per minute

        private Dictionary<string, List<float>> requestHistory;

        [Inject]
        private void Initialize()
        {
            requestHistory = new Dictionary<string, List<float>>();
        }

        /// <summary>
        /// Check if request is allowed
        /// </summary>
        public bool IsAllowed(string endpoint, int limit)
        {
            float currentTime = Time.realtimeSinceStartup;
            float windowSeconds = 60f;

            if (!requestHistory.ContainsKey(endpoint))
            {
                requestHistory[endpoint] = new List<float>();
            }

            var history = requestHistory[endpoint];

            // Remove old entries
            history.RemoveAll(t => currentTime - t > windowSeconds);

            // Check limit
            if (history.Count >= limit)
            {
                Debug.LogWarning($"[RateLimiter] Rate limit exceeded for {endpoint}: {history.Count}/{limit}");
                return false;
            }

            // Record request
            history.Add(currentTime);
            return true;
        }

        /// <summary>
        /// Check if score submission is allowed
        /// </summary>
        public bool IsScoreSubmissionAllowed()
        {
            return IsAllowed("score_submission", scoreSubmissionLimit);
        }

        /// <summary>
        /// Check if leaderboard request is allowed
        /// </summary>
        public bool IsLeaderboardRequestAllowed()
        {
            return IsAllowed("leaderboard", leaderboardRequestLimit);
        }

        /// <summary>
        /// Check if general API request is allowed
        /// </summary>
        public bool IsGeneralApiAllowed()
        {
            return IsAllowed("general", generalApiLimit);
        }

        /// <summary>
        /// Get time until next request is allowed
        /// </summary>
        public float GetTimeUntilAllowed(string endpoint, int limit)
        {
            float currentTime = Time.realtimeSinceStartup;
            float windowSeconds = 60f;

            if (!requestHistory.ContainsKey(endpoint))
                return 0f;

            var history = requestHistory[endpoint];

            if (history.Count < limit)
                return 0f;

            // Find oldest entry in window
            float oldestEntry = currentTime;
            foreach (float time in history)
            {
                if (currentTime - time <= windowSeconds && time < oldestEntry)
                {
                    oldestEntry = time;
                }
            }

            return Mathf.Max(0, windowSeconds - (currentTime - oldestEntry));
        }
    }
}
