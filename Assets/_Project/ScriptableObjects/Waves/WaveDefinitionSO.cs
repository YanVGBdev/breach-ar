using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject defining a wave composition
    /// </summary>
    [CreateAssetMenu(fileName = "NewWave", menuName = "BreachAR/Waves/WaveDefinition")]
    public class WaveDefinitionSO : ScriptableObject
    {
        [Header("Wave Info")]
        public int WaveIndex;
        public string WaveName;
        [TextArea(2, 5)]
        public string Description;
        public bool IsBossWave = false;

        [Header("Budget")]
        public int WaveBudget = 10; // Total spawn cost for this wave
        public float DifficultyMultiplier = 1f;

        [Header("Fragments")]
        public WaveFragmentEntry[] FragmentEntries;

        [Header("Timing")]
        public float TimeBetweenSpawns = 0.5f;
        public float MaxSpawnTime = 30f; // Max time to spawn all fragments
        public float DelayBeforeWave = 3f;

        [Header("Special")]
        public bool HasSwarmEvent = false;
        public int SwarmFragmentCount = 20;
        public FragmentDefinitionSO SwarmFragmentType;

        /// <summary>
        /// Get total fragments in this wave
        /// </summary>
        public int GetTotalFragmentCount()
        {
            int total = 0;
            foreach (var entry in FragmentEntries)
            {
                total += entry.Count;
            }
            return total;
        }

        /// <summary>
        /// Get fragments to spawn based on current budget
        /// </summary>
        public List<FragmentSpawnInfo> GetFragmentsForBudget(int availableBudget)
        {
            var fragments = new List<FragmentSpawnInfo>();
            int remainingBudget = availableBudget;

            foreach (var entry in FragmentEntries)
            {
                int count = Mathf.Min(entry.Count, remainingBudget / entry.FragmentType.SpawnCost);
                if (count > 0)
                {
                    fragments.Add(new FragmentSpawnInfo
                    {
                        FragmentType = entry.FragmentType,
                        Count = count
                    });
                    remainingBudget -= count * entry.FragmentType.SpawnCost;
                }
            }

            return fragments;
        }
    }

    /// <summary>
    /// Entry for a fragment type in a wave
    /// </summary>
    [System.Serializable]
    public class WaveFragmentEntry
    {
        public FragmentDefinitionSO FragmentType;
        public int Count = 1;
        [Range(0f, 1f)]
        public float SpawnChance = 1f;
    }

    /// <summary>
    /// Info for spawning fragments
    /// </summary>
    [System.Serializable]
    public class FragmentSpawnInfo
    {
        public FragmentDefinitionSO FragmentType;
        public int Count;
    }
}
