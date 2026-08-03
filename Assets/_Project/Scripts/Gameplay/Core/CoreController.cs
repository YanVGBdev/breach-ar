using UnityEngine;
using BreachAR.Core;
using BreachAR.ScriptableObjects;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Manages the Core (object to protect) - HP, damage, and events
    /// </summary>
    public class CoreController : MonoBehaviour, IDamageable
    {
        [Header("Configuration")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float maxHealthCap = 150f;

        [Header("Events")]
        [SerializeField] private OnCoreDamagedEvent onCoreDamaged;
        [SerializeField] private GameEvent onCoreDestroyed;

        private float currentHealth;
        private bool isInvulnerable;
        private bool isDestroyed;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => !isDestroyed;
        public float HealthPercentage => currentHealth / maxHealth;

        private void Awake()
        {
            ResetHealth();
        }

        /// <summary>
        /// Reset health to maximum
        /// </summary>
        public void ResetHealth()
        {
            currentHealth = maxHealth;
            isDestroyed = false;
            isInvulnerable = false;
        }

        /// <summary>
        /// Set maximum health (with upgrade cap)
        /// </summary>
        public void SetMaxHealth(float newMax)
        {
            maxHealth = Mathf.Clamp(newMax, 50f, maxHealthCap);
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }

        /// <summary>
        /// Take damage (IDamageable implementation)
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (isDestroyed || isInvulnerable || amount <= 0) return;

            float previousHealth = currentHealth;
            currentHealth = Mathf.Max(0, currentHealth - amount);

            // Notify listeners
            if (onCoreDamaged != null)
            {
                onCoreDamaged.Raise(new CoreDamagedData
                {
                    DamageAmount = amount,
                    CurrentHealth = currentHealth,
                    MaxHealth = maxHealth,
                    SourceFragmentId = "" // Could be set by caller
                });
            }

            // Check for destruction
            if (currentHealth <= 0 && !isDestroyed)
            {
                isDestroyed = true;
                onCoreDestroyed?.Raise();
                Debug.Log("[Core] Core destroyed - Game Over");
            }
        }

        /// <summary>
        /// Heal the core
        /// </summary>
        public void Heal(float amount)
        {
            if (isDestroyed || amount <= 0) return;

            currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        }

        /// <summary>
        /// Set invulnerability state
        /// </summary>
        public void SetInvulnerable(bool invulnerable)
        {
            isInvulnerable = invulnerable;
        }

        /// <summary>
        /// Get health percentage for UI
        /// </summary>
        public float GetHealthPercentage()
        {
            return currentHealth / maxHealth;
        }

        /// <summary>
        /// Check if health is below a threshold
        /// </summary>
        public bool IsHealthBelow(float threshold)
        {
            return (currentHealth / maxHealth) < threshold;
        }

        /// <summary>
        /// Revive with partial health
        /// </summary>
        public void Revive(float healthPercentage = 0.5f)
        {
            if (!isDestroyed) return;

            isDestroyed = false;
            currentHealth = maxHealth * Mathf.Clamp01(healthPercentage);
        }
    }
}
