using UnityEngine;
using BreachAR.Core;
using BreachAR.ScriptableObjects;
using BreachAR.Utils;
using VContainer;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Controls fragment behavior using a finite state machine
    /// Referência: GP-038, specs/EnemySpawner.md
    /// </summary>
    public class FragmentController : MonoBehaviour, IDamageable
    {
        [Header("Configuration")]
        [SerializeField] private FragmentDefinitionSO fragmentDefinition;

        [Header("Pool Settings")]
        [SerializeField] private string poolTag = "Fragment";

        [Header("State")]
        [SerializeField] private FragmentState currentState;
        [SerializeField] private float currentHealth;
        [SerializeField] private Transform targetCore;

        private float lastAttackTime;
        private float stateTimer;
        private bool isDead;
        private PoolManager poolManager;

        public FragmentDefinitionSO FragmentDefinition => fragmentDefinition;
        public float CurrentHealth => currentHealth;
        public float MaxHealth => fragmentDefinition != null ? fragmentDefinition.MaxHealth : 100f;
        public bool IsAlive => !isDead;
        public FragmentState CurrentState => currentState;

        [Inject]
        public void Construct(PoolManager pool)
        {
            poolManager = pool;
        }

        private void Start()
        {
            if (fragmentDefinition != null)
            {
                currentHealth = fragmentDefinition.MaxHealth;
            }
            ChangeState(FragmentState.Spawning);
        }

        private void Update()
        {
            if (isDead) return;

            stateTimer += Time.deltaTime;

            switch (currentState)
            {
                case FragmentState.Spawning:
                    UpdateSpawning();
                    break;
                case FragmentState.Seeking:
                    UpdateSeeking();
                    break;
                case FragmentState.Attacking:
                    UpdateAttacking();
                    break;
                case FragmentState.Staggered:
                    UpdateStaggered();
                    break;
                case FragmentState.Dying:
                    UpdateDying();
                    break;
            }
        }

        /// <summary>
        /// Initialize fragment with definition
        /// </summary>
        public void Initialize(FragmentDefinitionSO definition, Transform core, float difficultyMultiplier = 1f)
        {
            fragmentDefinition = definition;
            targetCore = core;

            if (definition != null)
            {
                currentHealth = definition.GetHealthAtDifficulty(difficultyMultiplier);
            }

            isDead = false;
            stateTimer = 0f;
            lastAttackTime = 0f;

            // Enable collider
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = true;
        }

        /// <summary>
        /// Reset state for pool reuse
        /// Referência: 99_agent_rules.md - Regra de limpeza para pooling
        /// </summary>
        public void ResetState()
        {
            isDead = false;
            currentHealth = 0f;
            stateTimer = 0f;
            lastAttackTime = 0f;
            currentState = FragmentState.Spawning;
            
            // Re-enable collider
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = true;
            
            CancelInvoke();
        }

        /// <summary>
        /// Take damage (IDamageable implementation)
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (isDead || amount <= 0) return;

            currentHealth -= amount;

            // Check for death
            if (currentHealth <= 0)
            {
                Die();
                return;
            }

            // Stagger on critical damage (more than 30% of max health)
            if (amount > MaxHealth * 0.3f && currentState != FragmentState.Staggered)
            {
                ChangeState(FragmentState.Staggered);
            }
        }

        /// <summary>
        /// Heal the fragment
        /// </summary>
        public void Heal(float amount)
        {
            if (isDead || amount <= 0) return;
            currentHealth = Mathf.Min(currentHealth + amount, MaxHealth);
        }

        #region State Machine

        private void ChangeState(FragmentState newState)
        {
            // Exit current state
            OnStateExit(currentState);

            currentState = newState;
            stateTimer = 0f;

            // Enter new state
            OnStateEnter(newState);
        }

        private void OnStateEnter(FragmentState state)
        {
            switch (state)
            {
                case FragmentState.Spawning:
                    // Play spawn animation/effect
                    break;
                case FragmentState.Seeking:
                    // Start moving toward core
                    break;
                case FragmentState.Attacking:
                    // Start attacking core
                    break;
                case FragmentState.Staggered:
                    // Stun briefly
                    break;
                case FragmentState.Dying:
                    // Start death sequence
                    break;
            }
        }

        private void OnStateExit(FragmentState state)
        {
            switch (state)
            {
                case FragmentState.Seeking:
                    break;
                case FragmentState.Attacking:
                    break;
                case FragmentState.Staggered:
                    break;
            }
        }

        private void UpdateSpawning()
        {
            // After spawn delay, start seeking
            if (stateTimer >= (fragmentDefinition != null ? fragmentDefinition.SpawnDelay : 0.5f))
            {
                ChangeState(FragmentState.Seeking);
            }
        }

        private void UpdateSeeking()
        {
            if (targetCore == null) return;

            // Move toward core
            Vector3 direction = (targetCore.position - transform.position).normalized;
            float speed = fragmentDefinition != null ? fragmentDefinition.MoveSpeed : 2f;

            transform.position += direction * speed * Time.deltaTime;

            // Look at core
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(direction);
            }

            // Check if within attack range
            float distance = Vector3.Distance(transform.position, targetCore.position);
            float attackRange = fragmentDefinition != null ? fragmentDefinition.AttackRange : 1.5f;

            if (distance <= attackRange)
            {
                ChangeState(FragmentState.Attacking);
            }
        }

        private void UpdateAttacking()
        {
            if (targetCore == null) return;

            // Check attack cooldown
            float attackCooldown = fragmentDefinition != null ? fragmentDefinition.AttackCooldown : 1f;

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                // Attack core
                IDamageable coreDamageable = targetCore.GetComponent<IDamageable>();
                if (coreDamageable != null)
                {
                    float damage = fragmentDefinition != null ? fragmentDefinition.DamageToCore : 10f;
                    coreDamageable.TakeDamage(damage);
                }

                lastAttackTime = Time.time;
            }

            // Check if core moved out of range
            float distance = Vector3.Distance(transform.position, targetCore.position);
            float attackRange = fragmentDefinition != null ? fragmentDefinition.AttackRange : 1.5f;

            if (distance > attackRange * 1.5f)
            {
                ChangeState(FragmentState.Seeking);
            }
        }

        private void UpdateStaggered()
        {
            // Stagger duration
            if (stateTimer >= 0.5f)
            {
                ChangeState(FragmentState.Seeking);
            }
        }

        private void UpdateDying()
        {
            // Death animation duration
            if (stateTimer >= 1.5f)
            {
                // Emit kill event
                GameEvents.OnFragmentKilled?.Invoke(new FragmentKilledData
                {
                    FragmentId = fragmentDefinition != null ? fragmentDefinition.FragmentId : "",
                    FragmentType = fragmentDefinition != null ? fragmentDefinition.Type : FragmentType.Basic,
                    OrbId = "",
                    ComboMultiplier = 1f,
                    ViaRicochet = false,
                    Position = transform.position
                });

                // Return to pool
                ReturnToPool();
            }
        }

        #endregion

        /// <summary>
        /// Kill the fragment immediately
        /// </summary>
        public void Die()
        {
            if (isDead) return;

            isDead = true;
            ChangeState(FragmentState.Dying);

            // Disable collider
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }

        private void ReturnToPool()
        {
            ResetState();
            
            if (poolManager != null)
            {
                poolManager.Return(poolTag, gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Get health percentage for UI
        /// </summary>
        public float GetHealthPercentage()
        {
            return currentHealth / MaxHealth;
        }
    }

    /// <summary>
    /// Fragment states
    /// </summary>
    public enum FragmentState
    {
        Spawning,
        Seeking,
        Attacking,
        Staggered,
        Dying
    }
}
