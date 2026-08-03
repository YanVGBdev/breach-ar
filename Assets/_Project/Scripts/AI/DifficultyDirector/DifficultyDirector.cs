using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.AI
{
    /// <summary>
    /// Monitors player performance and adjusts difficulty dynamically
    /// </summary>
    public class DifficultyDirector : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private DifficultyConfig config;

        [Header("Monitoring Windows")]
        [SerializeField] private int hitRateWindowSize = 10;
        [SerializeField] private int reactionTimeWindowSize = 10;

        // Metrics tracking
        private Queue<HitEvent> hitEvents = new Queue<HitEvent>();
        private Queue<float> reactionTimes = new Queue<float>();
        private float coreDamageTotal;
        private int totalLaunches;
        private int totalHits;
        private float sessionStartTime;
        private int currentWaveIndex;

        // Difficulty state
        private float currentDifficultyDelta;
        private float accumulatedDelta;
        private float skillScore;

        public float CurrentDifficultyDelta => currentDifficultyDelta;
        public float AccumulatedDelta => accumulatedDelta;
        public float SkillScore => skillScore;

        private void Awake()
        {
            sessionStartTime = Time.time;
            ResetMetrics();
        }

        /// <summary>
        /// Reset all metrics for a new session
        /// </summary>
        public void ResetMetrics()
        {
            hitEvents.Clear();
            reactionTimes.Clear();
            coreDamageTotal = 0f;
            totalLaunches = 0;
            totalHits = 0;
            currentDifficultyDelta = 0f;
            accumulatedDelta = 0f;
            skillScore = 0f;
            currentWaveIndex = 0;
        }

        /// <summary>
        /// Record a launch event
        /// </summary>
        public void RecordLaunch()
        {
            totalLaunches++;
        }

        /// <summary>
        /// Record a hit event
        /// </summary>
        public void RecordHit(float reactionTime, bool wasRicochet = false)
        {
            totalHits++;

            // Add to hit events queue
            hitEvents.Enqueue(new HitEvent
            {
                Time = Time.time,
                ReactionTime = reactionTime,
                WasRicochet = wasRicochet
            });

            // Maintain window size
            while (hitEvents.Count > hitRateWindowSize)
            {
                hitEvents.Dequeue();
            }

            // Add to reaction times queue
            reactionTimes.Enqueue(reactionTime);
            while (reactionTimes.Count > reactionTimeWindowSize)
            {
                reactionTimes.Dequeue();
            }
        }

        /// <summary>
        /// Record damage taken by core
        /// </summary>
        public void RecordCoreDamage(float damage)
        {
            coreDamageTotal += damage;
        }

        /// <summary>
        /// Calculate difficulty for the next wave
        /// </summary>
        public float CalculateDifficultyForWave(int waveIndex)
        {
            currentWaveIndex = waveIndex;

            // Calculate skill score
            skillScore = CalculateSkillScore();

            // Calculate delta based on skill vs target
            float targetSkill = config != null ? config.targetSkillScore : 0.5f;
            float sensitivity = config != null ? config.sensitivity : 1f;
            float maxDelta = config != null ? config.maxDifficultyDelta : 0.15f;

            currentDifficultyDelta = Mathf.Clamp(
                (skillScore - targetSkill) * sensitivity,
                -maxDelta,
                maxDelta
            );

            // Accumulate delta over waves
            accumulatedDelta += currentDifficultyDelta;

            // Clamp accumulated delta
            float maxAccumulated = config != null ? config.maxAccumulatedDelta : 0.5f;
            accumulatedDelta = Mathf.Clamp(accumulatedDelta, -maxAccumulated, maxAccumulated);

            return accumulatedDelta;
        }

        /// <summary>
        /// Get difficulty multiplier for current state
        /// </summary>
        public float GetDifficultyMultiplier()
        {
            return 1f + accumulatedDelta;
        }

        /// <summary>
        /// Calculate skill score from metrics
        /// </summary>
        private float CalculateSkillScore()
        {
            if (totalLaunches == 0) return 0.5f;

            // Hit rate (0-1)
            float hitRate = totalHits / (float)totalLaunches;

            // Average reaction time (inverted and normalized)
            float avgReactionTime = CalculateAverageReactionTime();
            float reactionTimeScore = 1f - Mathf.Clamp01(avgReactionTime / 3f); // 3s = worst

            // Core damage rate (inverted and normalized)
            float sessionDuration = Time.time - sessionStartTime;
            float coreDamageRate = sessionDuration > 0 ? coreDamageTotal / sessionDuration : 0f;
            float coreDamageScore = 1f - Mathf.Clamp01(coreDamageRate / 20f); // 20 dmg/s = worst

            // Weighted combination
            float w1 = config != null ? config.weightHitRate : 0.4f;
            float w2 = config != null ? config.weightReactionTime : 0.3f;
            float w3 = config != null ? config.weightCoreDamage : 0.3f;

            skillScore = (w1 * hitRate) + (w2 * reactionTimeScore) + (w3 * coreDamageScore);

            return Mathf.Clamp01(skillScore);
        }

        /// <summary>
        /// Calculate average reaction time from recent events
        /// </summary>
        private float CalculateAverageReactionTime()
        {
            if (reactionTimes.Count == 0) return 2f; // Default mediocre time

            float total = 0f;
            foreach (float time in reactionTimes)
            {
                total += time;
            }
            return total / reactionTimes.Count;
        }

        /// <summary>
        /// Get stats for analytics/debug
        /// </summary>
        public DifficultyStats GetStats()
        {
            return new DifficultyStats
            {
                SkillScore = skillScore,
                CurrentDelta = currentDifficultyDelta,
                AccumulatedDelta = accumulatedDelta,
                HitRate = totalLaunches > 0 ? (float)totalHits / totalLaunches : 0f,
                AvgReactionTime = CalculateAverageReactionTime(),
                CoreDamageRate = (Time.time - sessionStartTime) > 0 
                    ? coreDamageTotal / (Time.time - sessionStartTime) 
                    : 0f,
                TotalLaunches = totalLaunches,
                TotalHits = totalHits,
                WaveIndex = currentWaveIndex
            };
        }
    }

    /// <summary>
    /// Configuration for difficulty director
    /// </summary>
    [System.Serializable]
    public class DifficultyConfig
    {
        [Header("Target")]
        public float targetSkillScore = 0.5f;
        public float sensitivity = 1f;
        public float maxDifficultyDelta = 0.15f;
        public float maxAccumulatedDelta = 0.5f;

        [Header("Weights")]
        public float weightHitRate = 0.4f;
        public float weightReactionTime = 0.3f;
        public float weightCoreDamage = 0.3f;
    }

    /// <summary>
    /// Hit event data
    /// </summary>
    [System.Serializable]
    public struct HitEvent
    {
        public float Time;
        public float ReactionTime;
        public bool WasRicochet;
    }

    /// <summary>
    /// Difficulty stats for analytics
    /// </summary>
    [System.Serializable]
    public struct DifficultyStats
    {
        public float SkillScore;
        public float CurrentDelta;
        public float AccumulatedDelta;
        public float HitRate;
        public float AvgReactionTime;
        public float CoreDamageRate;
        public int TotalLaunches;
        public int TotalHits;
        public int WaveIndex;
    }
}
