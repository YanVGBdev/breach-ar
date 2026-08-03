using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.Backend
{
    /// <summary>
    /// Manages player economy with server sync
    /// Referência: BK-009
    /// </summary>
    public class PlayerEconomyService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float syncInterval = 30f;
        [SerializeField] private int maxSoftCurrency = 999999;
        [SerializeField] private int maxHardCurrency = 99999;

        [Header("State")]
        [SerializeField] private int softCurrency;
        [SerializeField] private int hardCurrency;
        [SerializeField] private int totalSoftEarned;
        [SerializeField] private int totalHardEarned;
        [SerializeField] private bool isDirty;

        [Inject] private SupabaseService supabaseService;
        [Inject] private RemoteConfigService remoteConfig;

        public int SoftCurrency => softCurrency;
        public int HardCurrency => hardCurrency;
        public int TotalSoftEarned => totalSoftEarned;
        public int TotalHardEarned => totalHardEarned;

        /// <summary>
        /// Event raised when economy changes
        /// </summary>
        public event Action<EconomyChangedData> OnEconomyChanged;

        private void Start()
        {
            LoadLocal();
            StartCoroutine(SyncLoop());
        }

        private void OnDestroy()
        {
            SaveLocal();
        }

        /// <summary>
        /// Load economy from local storage
        /// </summary>
        private void LoadLocal()
        {
            softCurrency = PlayerPrefs.GetInt("economy_soft", 0);
            hardCurrency = PlayerPrefs.GetInt("economy_hard", 0);
            totalSoftEarned = PlayerPrefs.GetInt("economy_soft_total", 0);
            totalHardEarned = PlayerPrefs.GetInt("economy_hard_total", 0);

            Debug.Log($"[Economy] Loaded: Soft={softCurrency}, Hard={hardCurrency}");
        }

        /// <summary>
        /// Save economy to local storage
        /// </summary>
        private void SaveLocal()
        {
            PlayerPrefs.SetInt("economy_soft", softCurrency);
            PlayerPrefs.SetInt("economy_hard", hardCurrency);
            PlayerPrefs.SetInt("economy_soft_total", totalSoftEarned);
            PlayerPrefs.SetInt("economy_hard_total", totalHardEarned);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Add soft currency (from gameplay)
        /// Referência: GP-023
        /// </summary>
        public bool AddSoftCurrency(int amount, string reason = "")
        {
            if (amount <= 0) return false;

            // Apply reward multiplier from remote config
            float multiplier = remoteConfig != null ? remoteConfig.GetFloat("session_reward_multiplier", 1f) : 1f;
            int finalAmount = Mathf.RoundToInt(amount * multiplier);

            int previousAmount = softCurrency;
            softCurrency = Mathf.Min(softCurrency + finalAmount, maxSoftCurrency);
            totalSoftEarned += finalAmount;
            isDirty = true;

            int delta = softCurrency - previousAmount;
            if (delta > 0)
            {
                OnEconomyChanged?.Invoke(new EconomyChangedData
                {
                    CurrencyType = CurrencyType.Soft,
                    PreviousAmount = previousAmount,
                    NewAmount = softCurrency,
                    Delta = delta,
                    Reason = reason
                });

                Debug.Log($"[Economy] +{delta} Soft ({reason}). Total: {softCurrency}");
            }

            SaveLocal();
            return delta > 0;
        }

        /// <summary>
        /// Spend soft currency
        /// </summary>
        public bool SpendSoftCurrency(int amount, string reason = "")
        {
            if (amount <= 0 || softCurrency < amount) return false;

            int previousAmount = softCurrency;
            softCurrency -= amount;
            isDirty = true;

            OnEconomyChanged?.Invoke(new EconomyChangedData
            {
                CurrencyType = CurrencyType.Soft,
                PreviousAmount = previousAmount,
                NewAmount = softCurrency,
                Delta = -amount,
                Reason = reason
            });

            Debug.Log($"[Economy] -{amount} Soft ({reason}). Total: {softCurrency}");
            SaveLocal();
            return true;
        }

        /// <summary>
        /// Add hard currency (IAP or rare rewards)
        /// Referência: BK-011
        /// </summary>
        public bool AddHardCurrency(int amount, string reason = "", bool validated = false)
        {
            if (amount <= 0) return false;

            int previousAmount = hardCurrency;
            hardCurrency = Mathf.Min(hardCurrency + amount, maxHardCurrency);
            totalHardEarned += amount;
            isDirty = true;

            OnEconomyChanged?.Invoke(new EconomyChangedData
            {
                CurrencyType = CurrencyType.Hard,
                PreviousAmount = previousAmount,
                NewAmount = hardCurrency,
                Delta = amount,
                Reason = reason,
                IsValidated = validated
            });

            Debug.Log($"[Economy] +{amount} Hard ({reason}, validated={validated}). Total: {hardCurrency}");
            SaveLocal();
            return true;
        }

        /// <summary>
        /// Spend hard currency
        /// </summary>
        public bool SpendHardCurrency(int amount, string reason = "")
        {
            if (amount <= 0 || hardCurrency < amount) return false;

            int previousAmount = hardCurrency;
            hardCurrency -= amount;
            isDirty = true;

            OnEconomyChanged?.Invoke(new EconomyChangedData
            {
                CurrencyType = CurrencyType.Hard,
                PreviousAmount = previousAmount,
                NewAmount = hardCurrency,
                Delta = -amount,
                Reason = reason
            });

            Debug.Log($"[Economy] -{amount} Hard ({reason}). Total: {hardCurrency}");
            SaveLocal();
            return true;
        }

        /// <summary>
        /// Apply session rewards
        /// Referência: GP-023
        /// </summary>
        public void ApplySessionRewards(SessionRewards rewards)
        {
            AddSoftCurrency(rewards.SoftCurrency, "Session Reward");
            // Experience and BattlePassXP handled by other systems
        }

        /// <summary>
        /// Sync loop for server sync
        /// Referência: BK-009
        /// </summary>
        private IEnumerator SyncLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(syncInterval);

                if (isDirty && supabaseService != null && supabaseService.IsAuthenticated)
                {
                    yield return SyncToServer();
                }
            }
        }

        /// <summary>
        /// Sync economy to server
        /// </summary>
        private IEnumerator SyncToServer()
        {
            var data = new Dictionary<string, object>
            {
                { "user_id", supabaseService.CurrentUserId },
                { "soft_currency", softCurrency },
                { "hard_currency", hardCurrency },
                { "total_soft_earned", totalSoftEarned },
                { "total_hard_earned", totalHardEarned },
                { "last_updated", DateTime.UtcNow.ToString("o") }
            };

            yield return supabaseService.SaveData("player_economy", null, data);
            isDirty = false;

            Debug.Log("[Economy] Synced to server");
        }

        /// <summary>
        /// Get economy state for save data
        /// </summary>
        public EconomySaveData GetSaveData()
        {
            return new EconomySaveData
            {
                SoftCurrency = softCurrency,
                HardCurrency = hardCurrency
            };
        }

        /// <summary>
        /// Load economy from save data
        /// </summary>
        public void LoadFromData(EconomySaveData data)
        {
            softCurrency = data.SoftCurrency;
            hardCurrency = data.HardCurrency;
            SaveLocal();
        }
    }

    /// <summary>
    /// Currency types
    /// </summary>
    public enum CurrencyType
    {
        Soft,
        Hard
    }

    /// <summary>
    /// Economy changed event data
    /// </summary>
    [System.Serializable]
    public class EconomyChangedData
    {
        public CurrencyType CurrencyType;
        public int PreviousAmount;
        public int NewAmount;
        public int Delta;
        public string Reason;
        public bool IsValidated;
    }


}
