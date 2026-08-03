using UnityEngine;
using BreachAR.Core;
using BreachAR.ScriptableObjects;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.AI
{
    /// <summary>
    /// Generates procedural wave compositions based on difficulty budget
    /// Referência: specs/EnemySpawner.md, AI-006, AI-018
    /// </summary>
    public class WaveGenerator : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private WaveGenerationConfig config;

        [Inject] private DifficultyDirector difficultyDirector;

        private System.Random sessionRandom;

        /// <summary>
        /// Initialize with session seed for deterministic generation
        /// </summary>
        public void Initialize(int seed = -1)
        {
            if (seed < 0)
                seed = System.Environment.TickCount;
            
            sessionRandom = new System.Random(seed);
            Debug.Log($"[WaveGenerator] Initialized with seed: {seed}");
        }

        /// <summary>
        /// Generate wave composition for a given wave index
        /// Referência: AI-006 - Wave Budget system
        /// </summary>
        public WaveDefinitionSO GenerateWave(int waveIndex, bool isBossWave = false)
        {
            if (config == null)
            {
                Debug.LogWarning("[WaveGenerator] No config assigned");
                return null;
            }

            // Create wave definition
            var wave = ScriptableObject.CreateInstance<WaveDefinitionSO>();
            wave.waveIndex = waveIndex;
            wave.isBossWave = isBossWave;

            // Calculate budget for this wave
            float budget = CalculateWaveBudget(waveIndex);

            // Generate fragment composition
            wave.fragments = GenerateFragmentComposition(budget, waveIndex);

            // Set spawn timing
            wave.spawnInterval = config.baseSpawnInterval / difficultyDirector.GetSpawnRateMultiplier();
            wave.initialDelay = config.initialDelay;

            // Boss waves get special treatment
            if (isBossWave)
            {
                wave.bossDefinition = GetBossForWave(waveIndex);
                wave.fragments = new FragmentWithCount[0]; // No fragments during boss
            }

            Debug.Log($"[WaveGenerator] Generated wave {waveIndex} with budget {budget}");
            return wave;
        }

        /// <summary>
        /// Calculate wave budget based on wave index and difficulty
        /// Referência: AI-018 - Budget-based spawning
        /// </summary>
        private float CalculateWaveBudget(int waveIndex)
        {
            float baseBudget = config.baseBudget;
            float growthRate = config.budgetGrowthRate;
            float difficultyMultiplier = difficultyDirector.GetDifficultyMultiplier();

            // Exponential growth with difficulty scaling
            float budget = baseBudget * Mathf.Pow(1f + growthRate, waveIndex) * difficultyMultiplier;

            // Apply wave modifiers
            if (waveIndex % 10 == 0) // Every 10 waves, bonus budget
                budget *= config.milestoneBudgetMultiplier;

            return Mathf.Min(budget, config.maxBudget);
        }

        /// <summary>
        /// Generate fragment composition from budget
        /// Referência: AI-006 - Wave composition
        /// </summary>
        private FragmentWithCount[] GenerateFragmentComposition(float budget, int waveIndex)
        {
            var composition = new List<FragmentWithCount>();
            float remainingBudget = budget;

            // Get available fragment types based on wave progression
            var availableFragments = GetAvailableFragments(waveIndex);

            // Sort by cost (cheapest first for better distribution)
            availableFragments.Sort((a, b) => a.Cost.CompareTo(b.Cost));

            // Fill budget with fragments
            while (remainingBudget > 0 && availableFragments.Count > 0)
            {
                // Select fragment type (weighted random)
                FragmentDefinitionSO selected = SelectFragmentType(availableFragments);
                if (selected == null) break;

                // Calculate how many we can afford
                int count = Mathf.FloorToInt(remainingBudget / selected.Cost);
                if (count <= 0) break;

                // Apply max per wave limit
                count = Mathf.Min(count, config.maxFragmentsPerType);

                // Add to composition
                composition.Add(new FragmentWithCount
                {
                    FragmentDefinition = selected,
                    Count = count
                });

                remainingBudget -= count * selected.Cost;

                // Remove this type from available (or reduce its weight)
                availableFragments.Remove(selected);
            }

            // Shuffle for varied spawn order
            ShuffleList(composition, sessionRandom);

            return composition.ToArray();
        }

        /// <summary>
        /// Get available fragment types based on wave progression
        /// </summary>
        private List<FragmentDefinitionSO> GetAvailableFragments(int waveIndex)
        {
            var available = new List<FragmentDefinitionSO>();

            foreach (var fragment in config.allFragments)
            {
                if (fragment == null) continue;

                // Check unlock wave
                if (waveIndex >= fragment.unlockWave)
                {
                    available.Add(fragment);
                }
            }

            // Always have at least basic fragments
            if (available.Count == 0 && config.fallbackFragment != null)
            {
                available.Add(config.fallbackFragment);
            }

            return available;
        }

        /// <summary>
        /// Select fragment type using weighted random
        /// </summary>
        private FragmentDefinitionSO SelectFragmentType(List<FragmentDefinitionSO> fragments)
        {
            if (fragments.Count == 0) return null;

            // Calculate total weight
            float totalWeight = 0f;
            foreach (var frag in fragments)
            {
                totalWeight += frag.SelectionWeight;
            }

            // Random selection
            float random = (float)(sessionRandom.NextDouble() * totalWeight);
            float cumulative = 0f;

            foreach (var frag in fragments)
            {
                cumulative += frag.SelectionWeight;
                if (random <= cumulative)
                {
                    return frag;
                }
            }

            return fragments[fragments.Count - 1];
        }

        /// <summary>
        /// Get boss definition for boss wave
        /// </summary>
        private BossDefinitionSO GetBossForWave(int waveIndex)
        {
            int bossIndex = (waveIndex / 10) - 1; // Boss every 10 waves
            bossIndex = Mathf.Clamp(bossIndex, 0, config.bossDefinitions.Length - 1);

            if (config.bossDefinitions.Length > bossIndex)
            {
                return config.bossDefinitions[bossIndex];
            }

            return null;
        }

        /// <summary>
        /// Shuffle list using Fisher-Yates algorithm
        /// </summary>
        private void ShuffleList<T>(List<T> list, System.Random random)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }
    }

    /// <summary>
    /// Configuration for wave generation
    /// </summary>
    [CreateAssetMenu(fileName = "WaveGenerationConfig", menuName = "BreachAR/Wave Generation Config")]
    public class WaveGenerationConfig : ScriptableObject
    {
        [Header("Budget")]
        public float baseBudget = 10f;
        public float budgetGrowthRate = 0.15f;
        public float maxBudget = 200f;
        public float milestoneBudgetMultiplier = 1.5f;

        [Header("Timing")]
        public float baseSpawnInterval = 2f;
        public float initialDelay = 3f;

        [Header("Limits")]
        public int maxFragmentsPerType = 10;

        [Header("Fragments")]
        public FragmentDefinitionSO[] allFragments;
        public FragmentDefinitionSO fallbackFragment;

        [Header("Bosses")]
        public BossDefinitionSO[] bossDefinitions;
    }

    /// <summary>
    /// Fragment with count for wave composition
    /// </summary>
    [System.Serializable]
    public class FragmentWithCount
    {
        public FragmentDefinitionSO FragmentDefinition;
        public int Count;
    }
}
