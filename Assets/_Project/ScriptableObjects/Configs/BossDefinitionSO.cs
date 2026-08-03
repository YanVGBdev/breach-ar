using UnityEngine;

namespace BreachAR.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject defining boss properties and phases
    /// </summary>
    [CreateAssetMenu(fileName = "NewBoss", menuName = "BreachAR/Bosses/BossDefinition")]
    public class BossDefinitionSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string BossId;
        public string DisplayName;
        [TextArea(2, 5)]
        public string Description;
        public GameObject Prefab;

        [Header("Health")]
        public float MaxHealth = 1000f;
        public float HealthPerPhase = 250f;

        [Header("Phases")]
        public BossPhase[] Phases;

        [Header("Attacks")]
        public BossAttack[] AvailableAttacks;

        [Header("Weak Points")]
        public int WeakPointsPerPhase = 3;
        public float WeakPointHealth = 100f;

        [Header("Rewards")]
        public int ScoreReward = 5000;
        public int SoftCurrencyReward = 500;

        [Header("Visual")]
        public Color bossColor = Color.red;
        public GameObject DeathEffectPrefab;

        [Header("Audio")]
        public AudioClip IntroSound;
        public AudioClip AttackSound;
        public AudioClip DeathSound;
        public AudioClip PhaseChangeSound;

        /// <summary>
        /// Get health for a specific phase
        /// </summary>
        public float GetHealthForPhase(int phase)
        {
            return MaxHealth - (phase * HealthPerPhase);
        }

        /// <summary>
        /// Get attacks available in a specific phase
        /// </summary>
        public BossAttack[] GetAttacksForPhase(int phase)
        {
            var phaseAttacks = new System.Collections.Generic.List<BossAttack>();
            foreach (var attack in AvailableAttacks)
            {
                if (attack.AvailableFromPhase <= phase)
                {
                    phaseAttacks.Add(attack);
                }
            }
            return phaseAttacks.ToArray();
        }

        /// <summary>
        /// Check if boss should transition to next phase
        /// </summary>
        public bool ShouldTransitionPhase(float currentHealth, int currentPhase)
        {
            float threshold = MaxHealth - ((currentPhase + 1) * HealthPerPhase);
            return currentHealth <= threshold;
        }
    }

    /// <summary>
    /// Boss phase definition
    /// </summary>
    [System.Serializable]
    public class BossPhase
    {
        public string PhaseName;
        public float HealthThreshold; // Percentage (0-1)
        public float AttackSpeedMultiplier = 1f;
        public float MovementSpeedMultiplier = 1f;
        public bool HasSpecialAttack = false;
    }

    /// <summary>
    /// Boss attack definition
    /// </summary>
    [System.Serializable]
    public class BossAttack
    {
        public string AttackName;
        public float Damage = 20f;
        public float Cooldown = 2f;
        public float Range = 5f;
        public int AvailableFromPhase = 0;
        public AnimationCurve TrajectoryCurve;
    }
}
