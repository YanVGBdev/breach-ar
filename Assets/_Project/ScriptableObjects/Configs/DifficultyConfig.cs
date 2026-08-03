using UnityEngine;

namespace BreachAR.ScriptableObjects
{
    /// <summary>
    /// Configuration for Dynamic Difficulty Adjustment
    /// Referência: specs/DifficultyDirector.md, AI-002
    /// </summary>
    [CreateAssetMenu(fileName = "DifficultyConfig", menuName = "BreachAR/AI/Difficulty Config")]
    public class DifficultyConfig : ScriptableObject
    {
        [Header("Bounds")]
        public int minDifficulty = 1;
        public int maxDifficulty = 10;
        public int startDifficulty = 1;

        [Header("Adjustment")]
        public float adjustmentInterval = 60f;
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
}
