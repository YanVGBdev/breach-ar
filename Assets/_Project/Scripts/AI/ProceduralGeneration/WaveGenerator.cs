using UnityEngine;
using System.Collections.Generic;
using BreachAR.ScriptableObjects;

namespace BreachAR.AI
{
    /// <summary>
    /// Generates wave compositions based on budget and difficulty
    /// </summary>
    public class WaveGenerator : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private WaveGeneratorConfig config;

        [Header("Fragment Definitions")]
        [SerializeField] private FragmentDefinitionSO[] availableFragments;

        private System.Random sessionRandom;

        private void Awake()
        {
            sessionRandom = new System.Random();
        }

        /// <summary>
        /// Initialize with a specific seed (for Daily Challenge)
        /// </summary>
        public void InitializeWithSeed(int seed)
        {
            sessionRandom = new System.Random(seed);
        }

        /// <summary>
        /// Generate a wave definition based on wave index and difficulty
        /// </summary>
        public WaveDefinitionSO GenerateWave(int waveIndex, float difficultyMultiplier)
        {
            // Calculate wave budget
            int baseBudget = config != null ? config.baseBudget : 10;
            float budgetGrowth = config != null ? config.budgetGrowthPerWave : 1.2f;
            int budget = Mathf.RoundToInt(baseBudget * Mathf.Pow(budgetGrowth, waveIndex - 1));

            // Apply difficulty multiplier
            budget = Mathf.RoundToInt(budget * difficultyMultiplier);

            // Create wave definition
            WaveDefinitionSO wave = ScriptableObject.CreateInstance<WaveDefinitionSO>();
            wave.WaveIndex = waveIndex;
            wave.WaveName = $"Wave {waveIndex}";
            wave.WaveBudget = budget;
            wave.DifficultyMultiplier = difficultyMultiplier;

            // Generate fragment entries based on budget
            wave.FragmentEntries = GenerateFragmentEntries(budget, waveIndex);

            // Determine if this is a boss wave
            int bossInterval = config != null ? config.bossWaveInterval : 5;
            wave.IsBossWave = (waveIndex % bossInterval == 0);

            return wave;
        }

        /// <summary>
        /// Generate fragment entries that fit within budget
        /// </summary>
        private WaveFragmentEntry[] GenerateFragmentEntries(int budget, int waveIndex)
        {
            var entries = new List<WaveFragmentEntry>();
            int remainingBudget = budget;

            // Sort fragments by cost (cheapest first for budget filling)
            var sortedFragments = new List<FragmentDefinitionSO>(availableFragments);
            sortedFragments.Sort((a, b) => a.SpawnCost.CompareTo(b.SpawnCost));

            // Simple budget-based generation
            foreach (var fragment in sortedFragments)
            {
                if (remainingBudget <= 0) break;

                // Calculate how many of this fragment type we can afford
                int maxCount = remainingBudget / fragment.SpawnCost;
                if (maxCount <= 0) continue;

                // Randomize count based on wave index
                int count = sessionRandom.Next(1, Mathf.Min(maxCount, 10) + 1);

                if (count > 0)
                {
                    entries.Add(new WaveFragmentEntry
                    {
                        FragmentType = fragment,
                        Count = count,
                        SpawnChance = 1f
                    });

                    remainingBudget -= count * fragment.SpawnCost;
                }
            }

            return entries.ToArray();
        }

        /// <summary>
        /// Generate a swarm event wave
        /// </summary>
        public WaveDefinitionSO GenerateSwarmWave(int waveIndex, FragmentDefinitionSO swarmType, int swarmCount)
        {
            WaveDefinitionSO wave = ScriptableObject.CreateInstance<WaveDefinitionSO>();
            wave.WaveIndex = waveIndex;
            wave.WaveName = $"Swarm Wave {waveIndex}";
            wave.IsBossWave = false;
            wave.HasSwarmEvent = true;
            wave.SwarmFragmentCount = swarmCount;
            wave.SwarmFragmentType = swarmType;

            wave.FragmentEntries = new WaveFragmentEntry[]
            {
                new WaveFragmentEntry
                {
                    FragmentType = swarmType,
                    Count = swarmCount,
                    SpawnChance = 1f
                }
            };

            return wave;
        }

        /// <summary>
        /// Get available fragment types
        /// </summary>
        public FragmentDefinitionSO[] GetAvailableFragments()
        {
            return availableFragments;
        }
    }

    /// <summary>
    /// Configuration for wave generation
    /// </summary>
    [System.Serializable]
    public class WaveGeneratorConfig
    {
        public int baseBudget = 10;
        public float budgetGrowthPerWave = 1.2f;
        public int bossWaveInterval = 5;
        public int maxFragmentsPerWave = 50;
    }
}
