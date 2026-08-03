using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Backend
{
    /// <summary>
    /// Manages leaderboard operations
    /// Injected via VContainer DI
    /// </summary>
    public class LeaderboardService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private string apiEndpoint;
        [SerializeField] private float refreshInterval = 60f;

        private Dictionary<string, List<LeaderboardEntry>> cachedLeaderboards;
        private float lastRefreshTime;

        private void Awake()
        {
            cachedLeaderboards = new Dictionary<string, List<LeaderboardEntry>>();
        }

        /// <summary>
        /// Submit score to leaderboard
        /// </summary>
        public void SubmitScore(string leaderboardId, int score, int waveReached, float maxCombo)
        {
            Debug.Log($"[Leaderboard] Submitting score: {score} to {leaderboardId}");
        }

        /// <summary>
        /// Get leaderboard entries
        /// </summary>
        public List<LeaderboardEntry> GetLeaderboard(string leaderboardId, int limit = 100)
        {
            if (cachedLeaderboards.ContainsKey(leaderboardId))
            {
                return cachedLeaderboards[leaderboardId];
            }

            return new List<LeaderboardEntry>();
        }

        /// <summary>
        /// Get player rank
        /// </summary>
        public int GetPlayerRank(string leaderboardId)
        {
            return -1;
        }

        /// <summary>
        /// Get daily challenge leaderboard
        /// </summary>
        public List<LeaderboardEntry> GetDailyLeaderboard()
        {
            string date = System.DateTime.UtcNow.ToString("yyyy-MM-dd");
            return GetLeaderboard($"daily_{date}");
        }

        /// <summary>
        /// Refresh leaderboard cache
        /// </summary>
        public void RefreshCache()
        {
            Debug.Log("[Leaderboard] Refreshing cache");
            lastRefreshTime = Time.time;
        }
    }

    /// <summary>
    /// Leaderboard entry data
    /// </summary>
    [System.Serializable]
    public class LeaderboardEntry
    {
        public string PlayerId;
        public string PlayerName;
        public int Score;
        public int WaveReached;
        public float MaxCombo;
        public long Timestamp;
        public int Rank;
    }
}
