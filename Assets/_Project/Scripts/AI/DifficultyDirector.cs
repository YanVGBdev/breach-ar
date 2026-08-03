using UnityEngine;
using BreachAR.Core;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.AI
{
    /// <summary>
    /// Dynamic Difficulty Adjustment system
    /// Referência: specs/DifficultyDirector.md, AI-001, AI-002
    /// </summary>
    public class DifficultyDirector : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private DifficultyConfig config;

        [Header("Current State")]
        [SerializeField] private int currentDifficultyLevel = 1;
        [SerializeField] private float skillScore = 0.5f;
        [SerializeField] private float lastAdjustmentTime;

        [Inject] private ComboSystem comboSystem;
        [Inject] private ScoreSystem scoreSystem;

        // Metrics tracking
        private Queue<DifficultyMetric> recentMetrics = new Queue<DifficultyMetric>();
        private float metricsWindowSize = 120f; // 2 minutes window

        public int CurrentDifficulty => currentDifficultyLevel;
        public float SkillScore => skillScore;

        private void Start()
        {
            lastAdjustmentTime = Time.time;
        }

        private void Update()
        {
            // Periodic difficulty adjustment
            if (Time.time - lastAdjustmentTime >= config.adjustmentInterval)
            {
                EvaluateAndAdjust();
                lastAdjustmentTime = Time.time;
            }
        }

        /// <summary>
        /// Record a gameplay metric for difficulty calculation
        /// </summary>
        public void RecordMetric(DifficultyMetricType type, float value)
        {
            var metric = new DifficultyMetric
            {
                Type = type,
                Value = value,
                Timestamp = Time.time
            };

            recentMetrics.Enqueue(metric);

            // Remove old metrics outside window
            while (recentMetrics.Count > 0 && 
                   Time.time - recentMetrics.Peek().Timestamp > metricsWindowSize)
            {
                recentMetrics.Dequeue();
            }
        }

        /// <summary>
        /// Evaluate performance and adjust difficulty
        /// Referência: AI-002 - skill_score → difficulty_delta
        /// </summary>
        public void EvaluateAndAdjust()
        {
            if (config == null) return;

            // Calculate skill score from metrics
            float newSkillScore = CalculateSkillScore();
            skillScore = Mathf.Lerp(skillScore, newSkillScore, config.smoothingFactor);

            // Calculate difficulty delta
            float delta = CalculateDifficultyDelta(skillScore);

            // Apply delta (only at wave start, within ±15%)
            int previousLevel = currentDifficultyLevel;
            int newLevel = Mathf.Clamp(
                currentDifficultyLevel + Mathf.RoundToInt(delta),
                config.minDifficulty,
                config.maxDifficulty
            );

            if (newLevel != previousLevel)
            {
                currentDifficultyLevel = newLevel;
                OnDifficultyChanged(previousLevel, newLevel);
            }
        }

        /// <summary>
        /// Calculate skill score from recent metrics (0-1, higher = better player)
        /// </summary>
        private float CalculateSkillScore()
        {
            if (recentMetrics.Count == 0) return 0.5f;

            float hitRate = 0f;
            float survivalRate = 0f;
            float comboPerformance = 0f;
            int metricCount = 0;

            foreach (var metric in recentMetrics)
            {
                switch (metric.Type)
                {
                    case DifficultyMetricType.HitRegistered:
                        hitRate += metric.Value;
                        metricCount++;
                        break;
                    case DifficultyMetricType.CoreDamageTaken:
                        survivalRate += metric.Value;
                        metricCount++;
                        break;
                    case DifficultyMetricType.ComboAchieved:
                        comboPerformance += metric.Value;
                        metricCount++;
                        break;
                }
            }

            if (metricCount == 0) return 0.5f;

            // Weighted average
            float score = (hitRate * config.hitRateWeight + 
                          survivalRate * config.survivalWeight + 
                          comboPerformance * config.comboWeight);

            return Mathf.Clamp01(score);
        }

        /// <summary>
        /// Calculate difficulty delta from skill score
        /// Referência: AI-002 - Delta within ±15%
        /// </summary>
        private float CalculateDifficultyDelta(float skill)
        {
            // skill > 0.5 = player doing well, increase difficulty
            // skill < 0.5 = player struggling, decrease difficulty
            float delta = (skill - 0.5f) * config.deltaMultiplier;
            
            // Clamp to ±15%
            float maxDelta = currentDifficultyLevel * 0.15f;
            return Mathf.Clamp(delta, -maxDelta, maxDelta);
        }

        /// <summary>
        /// Get difficulty multiplier for gameplay systems
        /// </summary>
        public float GetDifficultyMultiplier()
        {
            return 1f + ((currentDifficultyLevel - 1) * config.difficultyScalePerLevel);
        }

        /// <summary>
        /// Get enemy health multiplier
        /// </summary>
        public float GetEnemyHealthMultiplier()
        {
            return 1f + ((currentDifficultyLevel - 1) * config.enemyHealthScale);
        }

        /// <summary>
        /// Get enemy speed multiplier
        /// </summary>
        public float GetEnemySpeedMultiplier()
        {
            return 1f + ((currentDifficultyLevel - 1) * config.enemySpeedScale);
        }

        /// <summary>
        /// Get spawn rate multiplier
        /// </summary>
        public float GetSpawnRateMultiplier()
        {
            return 1f + ((currentDifficultyLevel - 1) * config.spawnRateScale);
        }

        /// <summary>
        /// Reset difficulty for new session
        /// </summary>
        public void ResetDifficulty()
        {
            currentDifficultyLevel = config != null ? config.startDifficulty : 1;
            skillScore = 0.5f;
            recentMetrics.Clear();
            lastAdjustmentTime = Time.time;
        }

        /// <summary>
        /// Called when difficulty changes
        /// </summary>
        private void OnDifficultyChanged(int previousLevel, int newLevel)
        {
            Debug.Log($"[DifficultyDirector] Difficulty changed: {previousLevel} → {newLevel}");
            
            GameEvents.OnDifficultyChanged?.Invoke(new DifficultyChangedData
            {
                PreviousLevel = previousLevel,
                NewLevel = newLevel,
                Reason = skillScore > 0.5f ? "Player performing well" : "Player struggling"
            });
        }
    }

    /// <summary>
    /// Configuration for difficulty system
    /// </summary>
    [CreateAssetMenu(fileName = "DifficultyConfig", menuName = "BreachAR/Difficulty Config")]
    public class DifficultyConfig : ScriptableObject
    {
        [Header("Bounds")]
        public int minDifficulty = 1;
        public int maxDifficulty = 10;
        public int startDifficulty = 1;

        [Header("Adjustment")]
        public float adjustmentInterval = 60f; // seconds
        public float smoothingFactor = 0.3f;
        public float deltaMultiplier = 2f;

        [Header("Scaling")]
        public float difficultyScalePerLevel = 0.1f;
        public float enemyHealthScale = 0.15f;
        public float enemySpeedScale = 0.08f;
        public float spawnRateScale = 0.12f;

        [Header("Skill Score Weights")]
        public float hitRateWeight = 0.4f;
        public float survivalWeight = 0.35f;
        public float comboWeight = 0.25f;
    }

    /// <summary>
    /// Types of metrics for difficulty calculation
    /// </summary>
    public enum DifficultyMetricType
    {
        HitRegistered,
        HitMissed,
        CoreDamageTaken,
        ComboAchieved,
        WaveTimeTaken,
        PowerUpCollected
    }

    /// <summary>
    /// Single metric data point
    /// </summary>
    [System.Serializable]
    public struct DifficultyMetric
    {
        public DifficultyMetricType Type;
        public float Value;
        public float Timestamp;
    }

    /// <summary>
    /// Event data for difficulty change
    /// </summary>
    [System.Serializable]
    public struct DifficultyChangedData
    {
        public int PreviousLevel;
        public int NewLevel;
        public string Reason;
    }
}
