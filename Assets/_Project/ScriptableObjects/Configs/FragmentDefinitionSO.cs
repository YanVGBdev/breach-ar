using UnityEngine;

namespace BreachAR.ScriptableObjects
{
    /// <summary>
    /// Configuration data for fragment types
    /// Referência: specs/EnemySpawner.md
    /// </summary>
    [CreateAssetMenu(fileName = "FragmentDefinition", menuName = "BreachAR/Fragments/Fragment Definition")]
    public class FragmentDefinitionSO : ScriptableObject
    {
        [Header("Identity")]
        public string FragmentId;
        public string DisplayName;
        [TextArea(2, 4)]
        public string Description;
        public Sprite Icon;

        [Header("Type")]
        public FragmentType Type = FragmentType.Basic;

        [Header("Health")]
        public float MaxHealth = 50f;
        public float HealthDifficultyMultiplier = 1.1f;

        [Header("Movement")]
        public float MoveSpeed = 2f;
        public float SpeedDifficultyMultiplier = 1.05f;

        [Header("Attack")]
        public float DamageToCore = 10f;
        public float AttackRange = 1.5f;
        public float AttackCooldown = 1f;

        [Header("Spawn")]
        public float SpawnDelay = 0.5f;
        public int SpawnCost = 1;
        [Range(0f, 1f)]
        public float SelectionWeight = 1f;
        public int UnlockWave = 1;

        [Header("Score")]
        public int ScoreValue = 100;

        [Header("Visual")]
        public Color FragmentColor = Color.red;
        public GameObject Prefab;
        public GameObject DeathEffectPrefab;

        [Header("Audio")]
        public AudioClip SpawnSound;
        public AudioClip DeathSound;
        public AudioClip AttackSound;

        /// <summary>
        /// Get health adjusted for difficulty
        /// </summary>
        public float GetHealthAtDifficulty(float difficultyMultiplier)
        {
            return MaxHealth * difficultyMultiplier;
        }

        /// <summary>
        /// Get speed adjusted for difficulty
        /// </summary>
        public float GetSpeedAtDifficulty(float difficultyMultiplier)
        {
            return MoveSpeed * difficultyMultiplier;
        }
    }

    /// <summary>
    /// Fragment types
    /// </summary>
    public enum FragmentType
    {
        Basic,
        Fast,
        Tanky,
        Splitter,
        Shielded,
        Healer
    }
}
