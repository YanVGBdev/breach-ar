using UnityEngine;
using System;
using BreachAR.Core;
using BreachAR.AI;
using BreachAR.Backend;
using VContainer;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Endless mode with infinite difficulty scaling
    /// Referência: GP-026, specs/GameMode.md
    /// </summary>
    public class EndlessMode : MonoBehaviour, IGameMode
    {
        [Header("Endless Configuration")]
        [SerializeField] private EndlessConfig config;

        [Header("State")]
        [SerializeField] private int currentWave;
        [SerializeField] private float sessionTime;
        [SerializeField] private bool isSessionActive;
        [SerializeField] private int highestWave;

        [Inject] private SessionStateMachine sessionStateMachine;
        [Inject] private DifficultyDirector difficultyDirector;
        [Inject] private WaveGenerator waveGenerator;
        [Inject] private ScoreSystem scoreSystem;
        [Inject] private EconomyService economyService;

        public GameMode Mode => GameMode.Endless;
        public int CurrentWave => currentWave;
        public float SessionTime => sessionTime;
        public bool IsSessionActive => isSessionActive;
        public int HighestWave => highestWave;

        /// <summary>
        /// Event raised when milestone is reached
        /// </summary>
        public event Action<EndlessMilestoneData> OnMilestoneReached;

        private void Update()
        {
            if (isSessionActive)
            {
                sessionTime += Time.deltaTime;
            }
        }

        /// <summary>
        /// Initialize endless mode
        /// Referência: GP-026
        /// </summary>
        public void Initialize()
        {
            currentWave = 0;
            sessionTime = 0f;
            isSessionActive = true;

            // Load highest wave from save
            highestWave = PlayerPrefs.GetInt("endless_highest_wave", 0);

            // Initialize session with infinite waves
            sessionStateMachine.Initialize(int.MaxValue);

            Debug.Log("[Endless] Initialized - Infinite waves enabled");
        }

        /// <summary>
        /// Start endless session
        /// </summary>
        public void StartSession()
        {
            if (!isSessionActive) return;

            sessionStateMachine.StartSession();
            GameEvents.OnWaveStarted?.Invoke(new WaveStartedData
            {
                WaveIndex = 1,
                TotalWaves = int.MaxValue,
                IsBossWave = false
            });

            Debug.Log("[Endless] Session started");
        }

        /// <summary>
        /// Handle wave completion
        /// Referência: GP-026, AI-004
        /// </summary>
        public void OnWaveCompleted(int waveIndex)
        {
            currentWave = waveIndex;

            // Update highest wave
            if (currentWave > highestWave)
            {
                highestWave = currentWave;
                PlayerPrefs.SetInt("endless_highest_wave", highestWave);
            }

            // Check for milestones
            CheckMilestones(waveIndex);

            // Apply exponential difficulty scaling
            ApplyEndlessDifficulty(waveIndex);

            // Update high score
            CheckHighScore();

            Debug.Log($"[Endless] Wave {waveIndex} completed | Time: {sessionTime:F1}s");
        }

        /// <summary>
        /// Apply exponential difficulty scaling
        /// Referência: AI-004
        /// </summary>
        private void ApplyEndlessDifficulty(int waveIndex)
        {
            if (config == null) return;

            // Exponential growth after soft cap
            float difficultyMultiplier = 1f;

            if (waveIndex > config.softCapWave)
            {
                int wavesOverCap = waveIndex - config.softCapWave;
                difficultyMultiplier = Mathf.Pow(1f + config.exponentialGrowthRate, wavesOverCap);
            }
            else
            {
                // Linear scaling before soft cap
                difficultyMultiplier = 1f + (waveIndex * config.linearGrowthRate);
            }

            // Apply difficulty director influence
            float ddaMultiplier = difficultyDirector.GetDifficultyMultiplier();
            float finalMultiplier = difficultyMultiplier * ddaMultiplier;

            // Record for DDA
            difficultyDirector.RecordMetric(DifficultyMetricType.WaveTimeTaken, finalMultiplier);

            Debug.Log($"[Endless] Difficulty multiplier: {finalMultiplier:F2}x (Wave {waveIndex})");
        }

        /// <summary>
        /// Check for milestone rewards
        /// Referência: GP-026
        /// </summary>
        private void CheckMilestones(int waveIndex)
        {
            if (config == null || config.milestones == null) return;

            foreach (var milestone in config.milestones)
            {
                if (waveIndex == milestone.waveThreshold)
                {
                    OnMilestoneReached?.Invoke(new EndlessMilestoneData
                    {
                        WaveThreshold = milestone.waveThreshold,
                        RewardSoftCurrency = milestone.rewardSoftCurrency,
                        RewardExperience = milestone.rewardExperience,
                        RewardTitle = milestone.rewardTitle
                    });

                    // Apply milestone rewards
                    economyService.AddSoftCurrency(milestone.rewardSoftCurrency, $"Milestone: Wave {milestone.waveThreshold}");

                    Debug.Log($"[Endless] Milestone reached: Wave {milestone.waveThreshold} | Reward: {milestone.rewardSoftCurrency} soft");
                }
            }
        }

        /// <summary>
        /// Check and update high score
        /// </summary>
        private void CheckHighScore()
        {
            int currentScore = scoreSystem.CurrentScore;
            int highScore = PlayerPrefs.GetInt("endless_high_score", 0);

            if (currentScore > highScore)
            {
                PlayerPrefs.SetInt("endless_high_score", currentScore);
                Debug.Log($"[Endless] New high score: {currentScore}");
            }
        }

        /// <summary>
        /// Get session rewards
        /// Referência: GP-023
        /// </summary>
        public SessionRewards GetSessionRewards()
        {
            // Endless mode rewards scale with waves completed
            int baseReward = currentWave * 10;
            int scoreReward = scoreSystem.CurrentScore / 100;
            int timeBonus = Mathf.RoundToInt(sessionTime / 60f) * 5; // 5 per minute

            float multiplier = config != null ? config.rewardMultiplier : 1f;

            return new SessionRewards
            {
                SoftCurrency = Mathf.RoundToInt((baseReward + scoreReward + timeBonus) * multiplier),
                Experience = currentWave * 25,
                BattlePassXP = currentWave * 15
            };
        }

        /// <summary>
        /// Check if boss should spawn
        /// Referência: GP-026
        /// </summary>
        public bool ShouldSpawnBoss(int waveIndex)
        {
            if (config == null) return false;

            // Boss every N waves after initial waves
            return waveIndex > config.initialWaves && 
                   waveIndex % config.bossInterval == 0;
        }

        /// <summary>
        /// Get boss definition for wave
        /// </summary>
        public BossDefinitionSO GetBossForWave(int waveIndex)
        {
            if (!ShouldSpawnBoss(waveIndex)) return null;

            // Cycle through available bosses
            if (config != null && config.bossPool != null && config.bossPool.Length > 0)
            {
                int bossIndex = (waveIndex / config.bossInterval - 1) % config.bossPool.Length;
                return config.bossPool[bossIndex];
            }

            return null;
        }

        /// <summary>
        /// End endless session
        /// </summary>
        public void EndSession(bool victory)
        {
            isSessionActive = false;

            // Calculate and apply rewards
            var rewards = GetSessionRewards();
            economyService.ApplySessionRewards(rewards);

            // Save stats
            PlayerPrefs.SetInt("endless_last_wave", currentWave);
            PlayerPrefs.SetFloat("endless_last_time", sessionTime);
            PlayerPrefs.Save();

            Debug.Log($"[Endless] Session ended | Waves: {currentWave} | " +
                     $"Time: {sessionTime:F1}s | Score: {scoreSystem.CurrentScore} | " +
                     $"Rewards: {rewards.SoftCurrency} soft");
        }
    }

    /// <summary>
    /// Endless mode configuration
    /// </summary>
    [CreateAssetMenu(fileName = "EndlessConfig", menuName = "BreachAR/Endless Config")]
    public class EndlessConfig : ScriptableObject
    {
        [Header("Difficulty Scaling")]
        public float linearGrowthRate = 0.05f;
        public float exponentialGrowthRate = 0.1f;
        public int softCapWave = 20;

        [Header("Bosses")]
        public int initialWaves = 10;
        public int bossInterval = 10;
        public BossDefinitionSO[] bossPool;

        [Header("Milestones")]
        public EndlessMilestone[] milestones;

        [Header("Rewards")]
        public float rewardMultiplier = 1f;
    }

    /// <summary>
    /// Endless milestone definition
    /// </summary>
    [System.Serializable]
    public class EndlessMilestone
    {
        public int waveThreshold;
        public int rewardSoftCurrency;
        public int rewardExperience;
        public string rewardTitle;
    }

    /// <summary>
    /// Endless milestone event data
    /// </summary>
    [System.Serializable]
    public class EndlessMilestoneData
    {
        public int WaveThreshold;
        public int RewardSoftCurrency;
        public int RewardExperience;
        public string RewardTitle;
    }
}
