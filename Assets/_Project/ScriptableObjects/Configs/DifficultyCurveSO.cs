using UnityEngine;

namespace BreachAR.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject defining difficulty curves
    /// </summary>
    [CreateAssetMenu(fileName = "NewDifficultyCurve", menuName = "BreachAR/Balancing/DifficultyCurve")]
    public class DifficultyCurveSO : ScriptableObject
    {
        [Header("Wave Settings")]
        public AnimationCurve fragmentHealthCurve = AnimationCurve.EaseInOut(0, 1, 10, 2);
        public AnimationCurve fragmentSpeedCurve = AnimationCurve.EaseInOut(0, 1, 10, 1.5f);
        public AnimationCurve spawnRateCurve = AnimationCurve.EaseInOut(0, 1, 10, 2);

        [Header("Budget")]
        public float baseBudget = 10f;
        public AnimationCurve budgetGrowthCurve = AnimationCurve.Linear(0, 1, 10, 3);

        [Header("Boss Settings")]
        public int bossWaveInterval = 5;
        public float bossHealthMultiplier = 5f;
        public float bossDamageMultiplier = 2f;

        [Header("Endless Mode")]
        public float endlessScalingExponent = 1.1f;
        public float endlessScalingCap = 10f;

        [Header("Difficulty Director")]
        public float targetSkillScore = 0.5f;
        public float sensitivity = 1f;
        public float maxDifficultyDelta = 0.15f;
        public float maxAccumulatedDelta = 0.5f;

        [Header("Weights")]
        public float weightHitRate = 0.4f;
        public float weightReactionTime = 0.3f;
        public float weightCoreDamage = 0.3f;

        /// <summary>
        /// Get health multiplier for a wave
        /// </summary>
        public float GetHealthMultiplier(int waveIndex)
        {
            return fragmentHealthCurve.Evaluate(waveIndex);
        }

        /// <summary>
        /// Get speed multiplier for a wave
        /// </summary>
        public float GetSpeedMultiplier(int waveIndex)
        {
            return fragmentSpeedCurve.Evaluate(waveIndex);
        }

        /// <summary>
        /// Get spawn rate multiplier for a wave
        /// </summary>
        public float GetSpawnRateMultiplier(int waveIndex)
        {
            return spawnRateCurve.Evaluate(waveIndex);
        }

        /// <summary>
        /// Get budget for a wave
        /// </summary>
        public int GetWaveBudget(int waveIndex)
        {
            return Mathf.RoundToInt(baseBudget * budgetGrowthCurve.Evaluate(waveIndex));
        }

        /// <summary>
        /// Get endless mode difficulty multiplier
        /// </summary>
        public float GetEndlessMultiplier(int waveIndex)
        {
            float multiplier = Mathf.Pow(endlessScalingExponent, waveIndex);
            return Mathf.Min(multiplier, endlessScalingCap);
        }

        /// <summary>
        /// Get DifficultyDirector config
        /// </summary>
        public DifficultyDirectorConfig GetDifficultyDirectorConfig()
        {
            return new DifficultyDirectorConfig
            {
                targetSkillScore = targetSkillScore,
                sensitivity = sensitivity,
                maxDifficultyDelta = maxDifficultyDelta,
                maxAccumulatedDelta = maxAccumulatedDelta,
                weightHitRate = weightHitRate,
                weightReactionTime = weightReactionTime,
                weightCoreDamage = weightCoreDamage
            };
        }
    }

    /// <summary>
    /// DifficultyDirector configuration
    /// </summary>
    [System.Serializable]
    public struct DifficultyDirectorConfig
    {
        public float targetSkillScore;
        public float sensitivity;
        public float maxDifficultyDelta;
        public float maxAccumulatedDelta;
        public float weightHitRate;
        public float weightReactionTime;
        public float weightCoreDamage;
    }
}
