using UnityEngine;
using BreachAR.Core;
using BreachAR.ScriptableObjects;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Controls boss behavior with multi-phase FSM and weak points
    /// Referência: GP-029, GP-030
    /// </summary>
    public class BossController : MonoBehaviour, IDamageable
    {
        [Header("Configuration")]
        [SerializeField] private BossDefinitionSO bossDefinition;

        [Header("State")]
        [SerializeField] private BossState currentState = BossState.Inactive;
        [SerializeField] private int currentPhase;
        [SerializeField] private float currentHealth;
        [SerializeField] private float phaseTimer;

        [Header("Weak Points")]
        [SerializeField] private List<WeakPoint> weakPoints = new List<WeakPoint>();

        private bool isAlive = true;
        private float spawnTime;
        private int comboRequirement;

        public BossDefinitionSO BossDefinition => bossDefinition;
        public BossState CurrentState => currentState;
        public int CurrentPhase => currentPhase;
        public float CurrentHealth => currentHealth;
        public bool IsAlive => isAlive;
        public float HealthPercentage => currentHealth / (bossDefinition != null ? bossDefinition.MaxHealth : 100f);

        [Inject] private ScoreSystem scoreSystem;
        [Inject] private ComboSystem comboSystem;

        /// <summary>
        /// Initialize boss
        /// </summary>
        public void Initialize(BossDefinitionSO definition)
        {
            bossDefinition = definition;
            currentHealth = definition.MaxHealth;
            currentPhase = 0;
            spawnTime = Time.time;
            isAlive = true;

            // Setup weak points
            InitializeWeakPoints();

            // Start in intro state
            ChangeState(BossState.Intro);
        }

        /// <summary>
        /// Initialize weak points from definition
        /// </summary>
        private void InitializeWeakPoints()
        {
            weakPoints.Clear();

            if (bossDefinition.WeakPoints == null) return;

            foreach (var wpDef in bossDefinition.WeakPoints)
            {
                var weakPoint = new WeakPoint
                {
                    Definition = wpDef,
                    CurrentHealth = wpDef.MaxHealth,
                    IsDestroyed = false
                };
                weakPoints.Add(weakPoint);
            }
        }

        private void Update()
        {
            if (!isAlive || currentState == BossState.Inactive) return;

            phaseTimer += Time.deltaTime;

            switch (currentState)
            {
                case BossState.Intro:
                    UpdateIntro();
                    break;
                case BossState.Phase1:
                case BossState.Phase2:
                case BossState.Phase3:
                    UpdateCombat();
                    break;
                case BossState.Enraged:
                    UpdateEnraged();
                    break;
                case BossState.Dying:
                    UpdateDying();
                    break;
            }
        }

        /// <summary>
        /// Take damage (IDamageable implementation)
        /// Referência: GP-030 - Combo requirement for critical damage
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (!isAlive || currentState == BossState.Intro || currentState == BossState.Dying)
                return;

            // Check combo requirement for final phase
            if (currentState == BossState.Enraged && bossDefinition.RequiresComboForCritical)
            {
                float comboMultiplier = comboSystem.CurrentMultiplier;
                if (comboMultiplier < bossDefinition.RequiredCombo)
                {
                    amount *= bossDefinition.NonCriticalDamageMultiplier;
                    Debug.Log($"[Boss] Reduced damage - combo {comboMultiplier:F1} < {bossDefinition.RequiredCombo}");
                }
            }

            currentHealth = Mathf.Max(0, currentHealth - amount);

            // Check for phase transition
            CheckPhaseTransition();

            // Check for death
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// Damage a specific weak point
        /// </summary>
        public void DamageWeakPoint(int weakPointIndex, float amount)
        {
            if (weakPointIndex < 0 || weakPointIndex >= weakPoints.Count) return;

            var wp = weakPoints[weakPointIndex];
            if (wp.IsDestroyed) return;

            wp.CurrentHealth = Mathf.Max(0, wp.CurrentHealth - amount);

            if (wp.CurrentHealth <= 0)
            {
                DestroyWeakPoint(weakPointIndex);
            }
        }

        /// <summary>
        /// Destroy a weak point and potentially trigger phase transition
        /// </summary>
        private void DestroyWeakPoint(int index)
        {
            var wp = weakPoints[index];
            wp.IsDestroyed = true;

            Debug.Log($"[Boss] Weak point {index} destroyed");

            // Check if all weak points in current phase are destroyed
            bool allDestroyed = true;
            foreach (var weakPoint in weakPoints)
            {
                if (!weakPoint.IsDestroyed && weakPoint.Definition.Phase == currentPhase)
                {
                    allDestroyed = false;
                    break;
                }
            }

            if (allDestroyed)
            {
                // Trigger phase transition
                float healthPercent = currentHealth / bossDefinition.MaxHealth;
                if (healthPercent <= 0.3f)
                    ChangeState(BossState.Enraged);
                else if (healthPercent <= 0.6f)
                    ChangeState(BossState.Phase2);
            }
        }

        /// <summary>
        /// Check and handle phase transitions based on health
        /// </summary>
        private void CheckPhaseTransition()
        {
            float healthPercent = currentHealth / bossDefinition.MaxHealth;

            if (healthPercent <= 0.3f && currentState != BossState.Enraged)
            {
                ChangeState(BossState.Enraged);
            }
            else if (healthPercent <= 0.6f && currentState == BossState.Phase1)
            {
                ChangeState(BossState.Phase2);
            }
        }

        #region State Updates

        private void UpdateIntro()
        {
            // Intro animation duration
            if (phaseTimer >= bossDefinition.IntroDuration)
            {
                ChangeState(BossState.Phase1);
            }
        }

        private void UpdateCombat()
        {
            // Boss AI logic would go here
            // For now, just wait for player to deal damage
        }

        private void UpdateEnraged()
        {
            // Enraged behavior - faster attacks, new moves
        }

        private void UpdateDying()
        {
            // Death animation
            if (phaseTimer >= bossDefinition.DeathDuration)
            {
                OnDeathComplete();
            }
        }

        #endregion

        /// <summary>
        /// Change boss state
        /// </summary>
        private void ChangeState(BossState newState)
        {
            Debug.Log($"[Boss] State: {currentState} → {newState}");
            currentState = newState;
            phaseTimer = 0f;

            OnStateEnter(newState);
        }

        /// <summary>
        /// Called when entering a new state
        /// </summary>
        private void OnStateEnter(BossState state)
        {
            switch (state)
            {
                case BossState.Intro:
                    // Play intro animation
                    break;
                case BossState.Phase1:
                    comboRequirement = 0; // No combo requirement
                    break;
                case BossState.Phase2:
                    comboRequirement = 0;
                    break;
                case BossState.Phase3:
                    comboRequirement = 0;
                    break;
                case BossState.Enraged:
                    comboRequirement = bossDefinition.RequiredCombo;
                    break;
                case BossState.Dying:
                    // Disable colliders
                    foreach (var col in GetComponents<Collider>())
                    {
                        col.enabled = false;
                    }
                    break;
            }
        }

        /// <summary>
        /// Handle boss death
        /// </summary>
        private void Die()
        {
            if (!isAlive) return;

            isAlive = false;
            ChangeState(BossState.Dying);

            // Calculate time taken
            float timeTaken = Time.time - spawnTime;

            // Add score
            scoreSystem?.AddBossDefeatedScore(bossDefinition.BossId, timeTaken);

            // Emit event
            GameEvents.OnBossDefeated?.Invoke(new BossDefeatedData
            {
                BossId = bossDefinition.BossId,
                TimeTaken = timeTaken,
                FinalScore = scoreSystem.CurrentScore
            });

            Debug.Log($"[Boss] {bossDefinition.BossName} defeated!");
        }

        /// <summary>
        /// Called when death animation completes
        /// </summary>
        private void OnDeathComplete()
        {
            // Spawn loot/rewards
            // TODO: Spawn reward orbs

            // Return to pool or destroy
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Get health percentage for UI
        /// </summary>
        public float GetHealthPercentage()
        {
            return currentHealth / (bossDefinition != null ? bossDefinition.MaxHealth : 100f);
        }

        /// <summary>
        /// Check if a weak point is available to damage
        /// </summary>
        public bool HasVulnerableWeakPoint()
        {
            foreach (var wp in weakPoints)
            {
                if (!wp.IsDestroyed && wp.Definition.Phase <= currentPhase)
                {
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// Boss states
    /// </summary>
    public enum BossState
    {
        Inactive,
        Intro,
        Phase1,
        Phase2,
        Phase3,
        Enraged,
        Dying
    }

    /// <summary>
    /// Runtime weak point data
    /// </summary>
    [System.Serializable]
    public class WeakPoint
    {
        public WeakPointDefinition Definition;
        public float CurrentHealth;
        public bool IsDestroyed;
    }

    /// <summary>
    /// ScriptableObject for boss definition
    /// Referência: GP-029
    /// </summary>
    [CreateAssetMenu(fileName = "BossDefinition", menuName = "BreachAR/Bosses/BossDefinition")]
    public class BossDefinitionSO : ScriptableObject
    {
        [Header("Identity")]
        public string BossId;
        public string BossName;
        [TextArea(2, 4)]
        public string Description;

        [Header("Health")]
        public float MaxHealth = 1000f;
        public float Phase2HealthThreshold = 0.6f;
        public float Phase3HealthThreshold = 0.3f;

        [Header("Phases")]
        public float IntroDuration = 3f;
        public float DeathDuration = 2f;

        [Header("Weak Points")]
        public WeakPointDefinition[] WeakPoints;

        [Header("Combo Requirement (Final Phase)")]
        public bool RequiresComboForCritical = true;
        public float RequiredCombo = 3.0f;
        public float NonCriticalDamageMultiplier = 0.25f;

        [Header("Visual")]
        public GameObject BossPrefab;
        public GameObject DeathEffectPrefab;

        [Header("Audio")]
        public AudioClip IntroSound;
        public AudioClip DeathSound;
        public AudioClip[] PhaseTransitionSounds;
    }

    /// <summary>
    /// Definition for a boss weak point
    /// </summary>
    [System.Serializable]
    public class WeakPointDefinition
    {
        public string WeakPointId;
        public int Phase;
        public float MaxHealth;
        public Vector3 LocalOffset;
        public float Radius = 0.5f;
    }
}
