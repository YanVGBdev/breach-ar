using UnityEngine;
using BreachAR.Core;

namespace BreachAR.Backend
{
    /// <summary>
    /// Manages player economy (soft and hard currency)
    /// </summary>
    public class EconomyService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private EconomyConfig config;

        [Header("Starting Values")]
        [SerializeField] private int startingSoftCurrency = 0;
        [SerializeField] private int startingHardCurrency = 0;

        private int softCurrency;
        private int hardCurrency;

        public int SoftCurrency => softCurrency;
        public int HardCurrency => hardCurrency;

        private void Awake()
        {
            LoadFromSave();
        }

        /// <summary>
        /// Load economy data from save
        /// </summary>
        public void LoadFromSave()
        {
            // This will be connected to SaveService
            softCurrency = startingSoftCurrency;
            hardCurrency = startingHardCurrency;
        }

        /// <summary>
        /// Load economy data from save data
        /// </summary>
        public void LoadFromData(int soft, int hard)
        {
            softCurrency = soft;
            hardCurrency = hard;
        }

        /// <summary>
        /// Add soft currency (earned from gameplay)
        /// </summary>
        public bool AddSoftCurrency(int amount, string reason = "")
        {
            if (amount <= 0) return false;

            softCurrency += amount;

            // Notify analytics
            Debug.Log($"[Economy] +{amount} Soft Currency ({reason}). Total: {softCurrency}");

            return true;
        }

        /// <summary>
        /// Spend soft currency
        /// </summary>
        public bool SpendSoftCurrency(int amount, string reason = "")
        {
            if (amount <= 0) return false;
            if (softCurrency < amount) return false;

            softCurrency -= amount;

            // Notify analytics
            Debug.Log($"[Economy] -{amount} Soft Currency ({reason}). Total: {softCurrency}");

            return true;
        }

        /// <summary>
        /// Add hard currency (from IAP or rare gameplay)
        /// </summary>
        public bool AddHardCurrency(int amount, string reason = "", bool validateServerSide = true)
        {
            if (amount <= 0) return false;

            // In production, this should validate with server
            if (validateServerSide)
            {
                // TODO: Implement server-side validation
                Debug.LogWarning("[Economy] Hard currency add should be validated server-side!");
            }

            hardCurrency += amount;

            // Notify analytics
            Debug.Log($"[Economy] +{amount} Hard Currency ({reason}). Total: {hardCurrency}");

            return true;
        }

        /// <summary>
        /// Spend hard currency
        /// </summary>
        public bool SpendHardCurrency(int amount, string reason = "")
        {
            if (amount <= 0) return false;
            if (hardCurrency < amount) return false;

            hardCurrency -= amount;

            // Notify analytics
            Debug.Log($"[Economy] -{amount} Hard Currency ({reason}). Total: {hardCurrency}");

            return true;
        }

        /// <summary>
        /// Get session rewards based on performance
        /// </summary>
        public SessionRewards CalculateSessionRewards(int score, int wavesCleared, bool perfectWave)
        {
            int softReward = 0;

            // Base reward from score
            softReward += score / 100;

            // Wave bonus
            softReward += wavesCleared * 10;

            // Perfect wave bonus
            if (perfectWave)
            {
                softReward += 50;
            }

            // Apply multiplier if configured
            float multiplier = config != null ? config.sessionRewardMultiplier : 1f;
            softReward = Mathf.RoundToInt(softReward * multiplier);

            return new SessionRewards
            {
                SoftCurrency = softReward,
                Experience = wavesCleared * 25,
                BattlePassXP = softReward / 2
            };
        }

        /// <summary>
        /// Apply session rewards
        /// </summary>
        public void ApplySessionRewards(SessionRewards rewards)
        {
            AddSoftCurrency(rewards.SoftCurrency, "Session Reward");
            // Experience and BattlePassXP handled by other systems
        }

        /// <summary>
        /// Get current economy state for save
        /// </summary>
        public EconomySaveData GetSaveData()
        {
            return new EconomySaveData
            {
                SoftCurrency = softCurrency,
                HardCurrency = hardCurrency
            };
        }
    }

    /// <summary>
    /// Economy configuration
    /// </summary>
    [System.Serializable]
    public class EconomyConfig
    {
        public float sessionRewardMultiplier = 1f;
        public int maxSoftCurrency = 999999;
        public int maxHardCurrency = 99999;
    }

    /// <summary>
    /// Session rewards data
    /// </summary>
    [System.Serializable]
    public struct SessionRewards
    {
        public int SoftCurrency;
        public int Experience;
        public int BattlePassXP;
    }

    /// <summary>
    /// Economy data for saving
    /// </summary>
    [System.Serializable]
    public struct EconomySaveData
    {
        public int SoftCurrency;
        public int HardCurrency;
    }
}
