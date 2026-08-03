using UnityEngine;
using BreachAR.Core;
using BreachAR.ScriptableObjects;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Manages score calculation based on events and combo multiplier
    /// </summary>
    public class ScoreSystem : MonoBehaviour
    {
        [Header("Score Values")]
        [SerializeField] private int fragmentCommonScore = 100;
        [SerializeField] private int fragmentEliteScore = 250;
        [SerializeField] private int multiKillBonus = 50;
        [SerializeField] private int riftClosedScore = 500;
        [SerializeField] private int bossDefeatedScore = 5000;
        [SerializeField] private int powerUpCollectedScore = 25;
        [SerializeField] private int perfectWaveBonus = 1000;

        [Header("References")]
        [SerializeField] private ComboSystem comboSystem;

        [Header("Events")]
        [SerializeField] private OnScoreChangedEvent onScoreChanged;

        private int currentScore;
        private int totalFragmentsKilled;
        private int totalRiftsClosed;
        private float maxComboAchieved;

        public int CurrentScore => currentScore;
        public int TotalFragmentsKilled => totalFragmentsKilled;
        public int TotalRiftsClosed => totalRiftsClosed;
        public float MaxComboAchieved => maxComboAchieved;

        private void Awake()
        {
            ResetScore();
        }

        /// <summary>
        /// Reset score to zero
        /// </summary>
        public void ResetScore()
        {
            currentScore = 0;
            totalFragmentsKilled = 0;
            totalRiftsClosed = 0;
            maxComboAchieved = 0f;
        }

        /// <summary>
        /// Add score for killing a fragment
        /// </summary>
        public void AddFragmentKillScore(FragmentType fragmentType, int extraTargetsHit = 0)
        {
            int baseScore = fragmentType == FragmentType.Elite ? fragmentEliteScore : fragmentCommonScore;
            int bonusScore = extraTargetsHit * multiKillBonus;
            int totalBase = baseScore + bonusScore;

            AddScore(totalBase, "Fragment Kill");
            totalFragmentsKilled++;

            // Track max combo
            if (comboSystem != null && comboSystem.CurrentMultiplier > maxComboAchieved)
            {
                maxComboAchieved = comboSystem.CurrentMultiplier;
            }
        }

        /// <summary>
        /// Add score for closing a rift
        /// </summary>
        public void AddRiftClosedScore(SurfaceType surfaceType)
        {
            AddScore(riftClosedScore, "Rift Closed");
            totalRiftsClosed++;
        }

        /// <summary>
        /// Add score for defeating a boss
        /// </summary>
        public void AddBossDefeatedScore(string bossId, float timeTaken)
        {
            AddScore(bossDefeatedScore, "Boss Defeated");
        }

        /// <summary>
        /// Add score for collecting a power-up
        /// </summary>
        public void AddPowerUpScore(string powerUpId)
        {
            AddScore(powerUpCollectedScore, "Power-Up Collected");
        }

        /// <summary>
        /// Add perfect wave bonus
        /// </summary>
        public void AddPerfectWaveBonus()
        {
            AddScore(perfectWaveBonus, "Perfect Wave");
        }

        /// <summary>
        /// Add score with combo multiplier
        /// </summary>
        private void AddScore(int baseScore, string reason)
        {
            float multiplier = 1f;
            if (comboSystem != null)
            {
                multiplier = comboSystem.CurrentMultiplier;
            }

            int finalScore = Mathf.RoundToInt(baseScore * multiplier);
            int previousScore = currentScore;
            currentScore += finalScore;

            // Notify listeners
            if (onScoreChanged != null)
            {
                onScoreChanged.Raise(new ScoreChangedData
                {
                    NewScore = currentScore,
                    ScoreDelta = finalScore,
                    Reason = reason
                });
            }
        }

        /// <summary>
        /// Get score breakdown for game over screen
        /// </summary>
        public ScoreBreakdown GetScoreBreakdown()
        {
            return new ScoreBreakdown
            {
                TotalScore = currentScore,
                FragmentsKilled = totalFragmentsKilled,
                RiftsClosed = totalRiftsClosed,
                MaxCombo = maxComboAchieved
            };
        }
    }

    /// <summary>
    /// Score breakdown data for UI display
    /// </summary>
    [System.Serializable]
    public struct ScoreBreakdown
    {
        public int TotalScore;
        public int FragmentsKilled;
        public int RiftsClosed;
        public float MaxCombo;
    }
}
