using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.Backend
{
    /// <summary>
    /// Server-side IAP validation for anti-fraud
    /// Referência: BK-011
    /// </summary>
    public class IAPValidationService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private bool enableServerValidation = true;
        [SerializeField] private float validationTimeout = 10f;

        [Inject] private SupabaseService supabaseService;
        [Inject] private PlayerEconomyService economyService;
        [Inject] private RateLimiter rateLimiter;

        /// <summary>
        /// Event raised when validation completes
        /// </summary>
        public event Action<IAPValidationResult> OnValidationComplete;

        /// <summary>
        /// Validate and process an IAP purchase
        /// Referência: BK-011
        /// </summary>
        public void ValidatePurchase(IAPPurchase purchase)
        {
            if (!enableServerValidation)
            {
                Debug.LogWarning("[IAP] Server validation disabled - processing locally");
                ProcessPurchaseLocally(purchase);
                return;
            }

            StartCoroutine(ValidatePurchaseCoroutine(purchase));
        }

        /// <summary>
        /// Validate purchase with server
        /// </summary>
        private IEnumerator ValidatePurchaseCoroutine(IAPPurchase purchase)
        {
            Debug.Log($"[IAP] Validating purchase: {purchase.ProductId}");

            // Rate limit check
            if (rateLimiter != null && !rateLimiter.IsGeneralApiAllowed())
            {
                yield return new WaitForSeconds(rateLimiter.GetTimeUntilAllowed("general", 60));
            }

            // In production, this would call Supabase Edge Function
            // For now, simulate validation
            var result = new IAPValidationResult
            {
                IsValid = true,
                TransactionId = purchase.TransactionId,
                ProductId = purchase.ProductId,
                ValidatedAt = DateTime.UtcNow.ToString("o")
            };

            // Simulate network delay
            yield return new WaitForSeconds(0.5f);

            // Process valid purchase
            if (result.IsValid)
            {
                ProcessValidatedPurchase(purchase);
            }

            OnValidationComplete?.Invoke(result);
        }

        /// <summary>
        /// Process a validated purchase
        /// </summary>
        private void ProcessValidatedPurchase(IAPPurchase purchase)
        {
            switch (purchase.Type)
            {
                case IAPType.SoftCurrencyPack:
                    economyService?.AddHardCurrency(
                        purchase.Amount,
                        $"IAP: {purchase.ProductId}",
                        validated: true
                    );
                    break;

                case IAPType.HardCurrencyPack:
                    economyService?.AddHardCurrency(
                        purchase.Amount,
                        $"IAP: {purchase.ProductId}",
                        validated: true
                    );
                    break;

                case IAPType.BattlePass:
                    // Grant battle pass access
                    PlayerPrefs.SetInt("battle_pass_active", 1);
                    PlayerPrefs.Save();
                    break;

                case IAPType.Revive:
                    // Processed immediately by game
                    break;

                case IAPType.RemoveAds:
                    PlayerPrefs.SetInt("ads_removed", 1);
                    PlayerPrefs.Save();
                    break;
            }

            Debug.Log($"[IAP] Purchase processed: {purchase.ProductId}");
        }

        /// <summary>
        /// Process purchase locally (fallback)
        /// </summary>
        private void ProcessPurchaseLocally(IAPPurchase purchase)
        {
            Debug.LogWarning($"[IAP] Processing locally: {purchase.ProductId}");
            ProcessValidatedPurchase(purchase);
        }

        /// <summary>
        /// Restore purchases (for app reinstalls)
        /// Referência: UI-028
        /// </summary>
        public void RestorePurchases(Action<bool> onComplete)
        {
            StartCoroutine(RestorePurchasesCoroutine(onComplete));
        }

        private IEnumerator RestorePurchasesCoroutine(Action<bool> onComplete)
        {
            Debug.Log("[IAP] Restoring purchases...");

            // In production, query store for previous purchases
            yield return new WaitForSeconds(1f);

            bool restored = PlayerPrefs.GetInt("ads_removed", 0) == 1 ||
                           PlayerPrefs.GetInt("battle_pass_active", 0) == 1;

            Debug.Log($"[IAP] Restore complete: {restored}");
            onComplete?.Invoke(restored);
        }

        /// <summary>
        /// Get available IAP products
        /// </summary>
        public List<IAPProduct> GetAvailableProducts()
        {
            return new List<IAPProduct>
            {
                new IAPProduct { Id = "soft_pack_small", Name = "100 Cristais", Price = "$0.99", Amount = 100 },
                new IAPProduct { Id = "soft_pack_medium", Name = "550 Cristais", Price = "$4.99", Amount = 550 },
                new IAPProduct { Id = "soft_pack_large", Name = "1200 Cristais", Price = "$9.99", Amount = 1200 },
                new IAPProduct { Id = "battle_pass_season", Name = "Battle Pass", Price = "$4.99", Amount = 0 },
                new IAPProduct { Id = "remove_ads", Name = "Remove Ads", Price = "$2.99", Amount = 0 }
            };
        }
    }

    /// <summary>
    /// IAP purchase data
    /// </summary>
    [System.Serializable]
    public class IAPPurchase
    {
        public string TransactionId;
        public string ProductId;
        public IAPType Type;
        public int Amount;
        public float Price;
        public string Receipt;
    }

    /// <summary>
    /// IAP types
    /// </summary>
    public enum IAPType
    {
        SoftCurrencyPack,
        HardCurrencyPack,
        BattlePass,
        Revive,
        RemoveAds
    }

    /// <summary>
    /// IAP validation result
    /// </summary>
    [System.Serializable]
    public class IAPValidationResult
    {
        public bool IsValid;
        public string TransactionId;
        public string ProductId;
        public string ValidatedAt;
        public string ErrorMessage;
    }

    /// <summary>
    /// IAP product definition
    /// </summary>
    [System.Serializable]
    public class IAPProduct
    {
        public string Id;
        public string Name;
        public string Price;
        public int Amount;
    }
}
