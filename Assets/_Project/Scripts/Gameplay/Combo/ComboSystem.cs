using UnityEngine;
using BreachAR.Core;
using BreachAR.ScriptableObjects;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Manages combo multiplier based on consecutive hits within a time window
    /// </summary>
    public class ComboSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float comboWindow = 2.5f;
        [SerializeField] private float comboIncrement = 0.1f;
        [SerializeField] private float maxMultiplier = 5f;
        [SerializeField] private float initialMultiplier = 1f;

        [Header("Events")]
        [SerializeField] private OnComboChangedEvent onComboChanged;

        private float currentMultiplier;
        private int comboCount;
        private float lastHitTime;
        private bool isActive;

        public float CurrentMultiplier => currentMultiplier;
        public int ComboCount => comboCount;
        public bool IsActive => isActive;
        public float TimeSinceLastHit => Time.time - lastHitTime;

        private void Awake()
        {
            ResetCombo();
        }

        private void Update()
        {
            if (!isActive) return;

            // Check if combo window has expired
            if (Time.time - lastHitTime > comboWindow)
            {
                ResetCombo();
            }
        }

        /// <summary>
        /// Register a hit and increment combo
        /// </summary>
        public void RegisterHit()
        {
            if (!isActive) return;

            float previousMultiplier = currentMultiplier;
            lastHitTime = Time.time;
            comboCount++;

            // Increment multiplier
            currentMultiplier = Mathf.Min(currentMultiplier + comboIncrement, maxMultiplier);

            // Notify listeners
            if (onComboChanged != null)
            {
                onComboChanged.Raise(new ComboChangedData
                {
                    Multiplier = currentMultiplier,
                    ComboCount = comboCount,
                    WasReset = false
                });
            }
        }

        /// <summary>
        /// Reset combo to initial state
        /// </summary>
        public void ResetCombo()
        {
            float previousMultiplier = currentMultiplier;
            currentMultiplier = initialMultiplier;
            comboCount = 0;
            lastHitTime = Time.time;

            // Notify listeners if combo was actually reset
            if (previousMultiplier > initialMultiplier && onComboChanged != null)
            {
                onComboChanged.Raise(new ComboChangedData
                {
                    Multiplier = currentMultiplier,
                    ComboCount = comboCount,
                    WasReset = true
                });
            }
        }

        /// <summary>
        /// Activate the combo system
        /// </summary>
        public void Activate()
        {
            isActive = true;
            ResetCombo();
        }

        /// <summary>
        /// Deactivate the combo system
        /// </summary>
        public void Deactivate()
        {
            isActive = false;
        }

        /// <summary>
        /// Check if combo is still active (within window)
        /// </summary>
        public bool IsComboActive()
        {
            return isActive && (Time.time - lastHitTime <= comboWindow);
        }

        /// <summary>
        /// Get combo progress (0-1) through current window
        /// </summary>
        public float GetComboWindowProgress()
        {
            if (!isActive) return 0f;
            float elapsed = Time.time - lastHitTime;
            return Mathf.Clamp01(elapsed / comboWindow);
        }
    }
}
