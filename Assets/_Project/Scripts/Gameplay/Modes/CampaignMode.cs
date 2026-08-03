using UnityEngine;
using System;
using BreachAR.Core;
using BreachAR.AI;
using BreachAR.Backend;
using VContainer;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Campaign mode controller with fixed wave progression and bosses
    /// Referência: GP-025, specs/GameMode.md
    /// </summary>
    public class CampaignMode : MonoBehaviour, IGameMode
    {
        [Header("Campaign Configuration")]
        [SerializeField] private CampaignConfig config;
        [SerializeField] private int totalWaves = 30;
        [SerializeField] private int bossWaveInterval = 10;

        [Header("State")]
        [SerializeField] private int currentWave;
        [SerializeField] private int currentBiome;
        [SerializeField] private bool isSessionActive;

        [Inject] private SessionStateMachine sessionStateMachine;
        [Inject] private DifficultyDirector difficultyDirector;
        [Inject] private WaveGenerator waveGenerator;
        [Inject] private ScoreSystem scoreSystem;
        [Inject] private ComboSystem comboSystem;
        [Inject] private EconomyService economyService;

        public GameMode Mode => GameMode.Campaign;
        public int CurrentWave => currentWave;
        public int TotalWaves => totalWaves;
        public bool IsSessionActive => isSessionActive;
        public int CurrentBiome => currentBiome;

        /// <summary>
        /// Event raised when biome changes
        /// </summary>
        public event Action<BiomeChangedData> OnBiomeChanged;

        /// <summary>
        /// Initialize campaign mode
        /// Referência: GP-025
        /// </summary>
        public void Initialize()
        {
            currentWave = 0;
            currentBiome = 0;
            isSessionActive = true;

            // Set total waves from config
            if (config != null)
            {
                totalWaves = config.totalWaves;
                bossWaveInterval = config.bossWaveInterval;
            }

            // Initialize session
            sessionStateMachine.Initialize(totalWaves);

            Debug.Log($"[Campaign] Initialized with {totalWaves} waves");
        }

        /// <summary>
        /// Start campaign session
        /// </summary>
        public void StartSession()
        {
            if (!isSessionActive) return;

            sessionStateMachine.StartSession();
            GameEvents.OnWaveStarted?.Invoke(new WaveStartedData
            {
                WaveIndex = 1,
                TotalWaves = totalWaves,
                IsBossWave = false
            });

            Debug.Log("[Campaign] Session started");
        }

        /// <summary>
        /// Handle wave completion
        /// Referência: GP-025
        /// </summary>
        public void OnWaveCompleted(int waveIndex)
        {
            currentWave = waveIndex;

            // Check for biome transition
            CheckBiomeTransition(waveIndex);

            // Apply difficulty scaling for campaign
            ApplyCampaignDifficulty(waveIndex);

            Debug.Log($"[Campaign] Wave {waveIndex}/{totalWaves} completed");
        }

        /// <summary>
        /// Check if biome should change
        /// Referência: GP-025
        /// </summary>
        private void CheckBiomeTransition(int waveIndex)
        {
            if (config == null) return;

            int newBiome = CalculateBiome(waveIndex);
            if (newBiome != currentBiome)
            {
                int previousBiome = currentBiome;
                currentBiome = newBiome;

                OnBiomeChanged?.Invoke(new BiomeChangedData
                {
                    PreviousBiome = previousBiome,
                    NewBiome = currentBiome,
                    WaveIndex = waveIndex
                });

                Debug.Log($"[Campaign] Biome changed: {previousBiome} → {currentBiome}");
            }
        }

        /// <summary>
        /// Calculate biome from wave index
        /// </summary>
        private int CalculateBiome(int waveIndex)
        {
            if (config == null || config.biomeTransitions == null || config.biomeTransitions.Length == 0)
                return 0;

            for (int i = config.biomeTransitions.Length - 1; i >= 0; i--)
            {
                if (waveIndex >= config.biomeTransitions[i])
                {
                    return i + 1;
                }
            }

            return 0;
        }

        /// <summary>
        /// Apply campaign-specific difficulty scaling
        /// Referência: GP-025
        /// </summary>
        private void ApplyCampaignDifficulty(int waveIndex)
        {
            // Campaign has predetermined difficulty curve
            float campaignMultiplier = 1f;

            if (config != null)
            {
                // Linear scaling with slight exponential curve
                float progress = (float)waveIndex / totalWaves;
                campaignMultiplier = 1f + (progress * config.difficultyScale);
            }

            // Record metric for DDA
            difficultyDirector.RecordMetric(DifficultyMetricType.WaveTimeTaken, campaignMultiplier);
        }

        /// <summary>
        /// Check if current wave is boss wave
        /// Referência: GP-025
        /// </summary>
        public bool IsBossWave(int waveIndex)
        {
            return waveIndex % bossWaveInterval == 0 && waveIndex <= totalWaves;
        }

        /// <summary>
        /// Get boss definition for wave
        /// </summary>
        public BossDefinitionSO GetBossForWave(int waveIndex)
        {
            if (!IsBossWave(waveIndex)) return null;

            int bossIndex = (waveIndex / bossWaveInterval) - 1;

            if (config != null && config.bossDefinitions != null && bossIndex < config.bossDefinitions.Length)
            {
                return config.bossDefinitions[bossIndex];
            }

            return null;
        }

        /// <summary>
        /// Get session rewards
        /// Referência: GP-023
        /// </summary>
        public SessionRewards GetSessionRewards()
        {
            return economyService.CalculateSessionRewards(
                scoreSystem.CurrentScore,
                currentWave,
                comboSystem.ComboCount > 0
            );
        }

        /// <summary>
        /// End campaign session
        /// </summary>
        public void EndSession(bool victory)
        {
            isSessionActive = false;

            // Calculate and apply rewards
            var rewards = GetSessionRewards();
            economyService.ApplySessionRewards(rewards);

            Debug.Log($"[Campaign] Session ended: {(victory ? "Victory" : "Defeat")} | " +
                     $"Score: {scoreSystem.CurrentScore} | " +
                     $"Waves: {currentWave}/{totalWaves} | " +
                     $"Rewards: {rewards.SoftCurrency} soft currency");
        }

    }

    /// <summary>
    /// Campaign configuration
    /// </summary>
    [CreateAssetMenu(fileName = "CampaignConfig", menuName = "BreachAR/Campaign Config")]
    public class CampaignConfig : ScriptableObject
    {
        [Header("Waves")]
        public int totalWaves = 30;
        public int bossWaveInterval = 10;

        [Header("Difficulty")]
        public float difficultyScale = 0.5f;

        [Header("Biomes")]
        public int[] biomeTransitions = new int[] { 10, 20 };
        public BiomeThemeSO[] biomeThemes;

        [Header("Bosses")]
        public BossDefinitionSO[] bossDefinitions;

        [Header("Rewards")]
        public float rewardMultiplier = 1f;
    }

    /// <summary>
    /// Biome changed event data
    /// </summary>
    [System.Serializable]
    public class BiomeChangedData
    {
        public int PreviousBiome;
        public int NewBiome;
        public int WaveIndex;
    }

    /// <summary>
    /// Interface for game modes
    /// </summary>
    public interface IGameMode
    {
        GameMode Mode { get; }
        bool IsSessionActive { get; }
        void Initialize();
        void StartSession();
        void EndSession(bool victory);
    }
}
