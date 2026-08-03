using UnityEngine;

namespace BreachAR.ScriptableObjects
{
    /// <summary>
    /// Configuration for procedural wave generation
    /// Referência: specs/EnemySpawner.md, AI-006
    /// </summary>
    [CreateAssetMenu(fileName = "WaveGenerationConfig", menuName = "BreachAR/AI/Wave Generation Config")]
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
}
