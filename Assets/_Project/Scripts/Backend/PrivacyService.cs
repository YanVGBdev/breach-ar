using UnityEngine;
using System;
using System.Collections;
using VContainer;

namespace BreachAR.Backend
{
    /// <summary>
    /// Handles privacy consent and data management for LGPD/GDPR compliance
    /// Referência: BK-014, BK-021, UI-026
    /// </summary>
    public class PrivacyService : MonoBehaviour
    {
        [Header("Privacy Configuration")]
        [SerializeField] private string privacyPolicyUrl = "https://breachar.com/privacy";
        [SerializeField] private string termsOfServiceUrl = "https://breachar.com/terms";
        [SerializeField] private bool requireConsentBeforeAnalytics = true;
        [SerializeField] private bool requireConsentBeforeAds = true;

        [Header("State")]
        [SerializeField] private bool hasConsented;
        [SerializeField] private bool analyticsConsent;
        [SerializeField] private bool adsConsent;
        [SerializeField] private bool personalizationConsent;
        [SerializeField] private string consentTimestamp;
        [SerializeField] private string consentVersion = "1.0";

        [Inject] private SupabaseService supabaseService;

        public bool HasConsented => hasConsented;
        public bool AnalyticsConsent => analyticsConsent && hasConsented;
        public bool AdsConsent => adsConsent && hasConsented;
        public bool PersonalizationConsent => personalizationConsent && hasConsented;

        /// <summary>
        /// Event raised when consent status changes
        /// </summary>
        public event Action<ConsentChangedEventArgs> OnConsentChanged;

        private void Awake()
        {
            LoadConsentState();
        }

        /// <summary>
        /// Check if consent is required before proceeding
        /// Referência: UI-026
        /// </summary>
        public bool IsConsentRequired()
        {
            return !hasConsented;
        }

        /// <summary>
        /// Grant all consent
        /// </summary>
        public void GrantAllConsent()
        {
            SetConsent(true, true, true);
        }

        /// <summary>
        /// Grant selected consent
        /// Referência: UI-026
        /// </summary>
        public void SetConsent(bool analytics, bool ads, bool personalization)
        {
            hasConsented = true;
            analyticsConsent = analytics;
            adsConsent = ads;
            personalizationConsent = personalization;
            consentTimestamp = DateTime.UtcNow.ToString("o");

            SaveConsentState();

            // Notify consent change
            OnConsentChanged?.Invoke(new ConsentChangedEventArgs
            {
                AnalyticsConsent = analytics,
                AdsConsent = ads,
                PersonalizationConsent = personalization,
                Timestamp = consentTimestamp
            });

            // Sync to server
            if (supabaseService != null && supabaseService.IsAuthenticated)
            {
                StartCoroutine(SyncConsentToServer());
            }

            Debug.Log($"[Privacy] Consent updated: Analytics={analytics}, Ads={ads}, Personalization={personalization}");
        }

        /// <summary>
        /// Reject all consent
        /// Referência: UI-026
        /// </summary>
        public void RejectAllConsent()
        {
            SetConsent(false, false, false);
        }

        /// <summary>
        /// Check if a specific feature requires consent
        /// </summary>
        public bool HasConsentFor(ConsentType type)
        {
            if (!hasConsented) return false;

            return type switch
            {
                ConsentType.Analytics => analyticsConsent,
                ConsentType.Ads => adsConsent,
                ConsentType.Personalization => personalizationConsent,
                _ => false
            };
        }

        /// <summary>
        /// Sync consent to server
        /// Referência: BK-014
        /// </summary>
        private IEnumerator SyncConsentToServer()
        {
            var consentData = new PrivacyConsentData
            {
                user_id = supabaseService.CurrentUserId,
                analytics_consent = analyticsConsent,
                ads_consent = adsConsent,
                personalization_consent = personalizationConsent,
                consent_timestamp = consentTimestamp,
                consent_version = consentVersion,
                ip_address = "anonymized", // Never store real IP
                device_info = SystemInfo.deviceModel
            };

            string json = JsonUtility.ToJson(consentData);

            // Save to Supabase
            var data = new System.Collections.Generic.Dictionary<string, object>
            {
                { "user_id", consentData.user_id },
                { "analytics_consent", consentData.analytics_consent },
                { "ads_consent", consentData.ads_consent },
                { "personalization_consent", consentData.personalization_consent },
                { "consent_timestamp", consentData.consent_timestamp },
                { "consent_version", consentData.consent_version }
            };

            yield return supabaseService.SaveData("privacy_consents", null, data);
        }

        /// <summary>
        /// Request account deletion (GDPR Article 17)
        /// Referência: BK-021
        /// </summary>
        public void RequestAccountDeletion()
        {
            Debug.Log("[Privacy] Account deletion requested");
            StartCoroutine(DeleteAccountAndData());
        }

        /// <summary>
        /// Delete all player data and account
        /// Referência: BK-021
        /// </summary>
        private IEnumerator DeleteAccountAndData()
        {
            Debug.Log("[Privacy] Starting account deletion process...");

            // Clear local data first
            ClearLocalData();

            // Delete from server
            if (supabaseService != null && supabaseService.IsAuthenticated)
            {
                yield return supabaseService.DeleteAccount();
            }

            // Reset consent state
            hasConsented = false;
            analyticsConsent = false;
            adsConsent = false;
            personalizationConsent = false;
            consentTimestamp = "";

            PlayerPrefs.DeleteKey("privacy_consent");
            PlayerPrefs.DeleteKey("privacy_analytics");
            PlayerPrefs.DeleteKey("privacy_ads");
            PlayerPrefs.DeleteKey("privacy_personalization");
            PlayerPrefs.DeleteKey("privacy_timestamp");

            Debug.Log("[Privacy] Account and data deleted");
        }

        /// <summary>
        /// Clear all local data
        /// </summary>
        private void ClearLocalData()
        {
            // Clear save data
            string savePath = Application.persistentDataPath + "/save.json";
            if (System.IO.File.Exists(savePath))
            {
                System.IO.File.Delete(savePath);
            }

            // Clear all player prefs
            PlayerPrefs.DeleteAll();

            Debug.Log("[Privacy] Local data cleared");
        }

        /// <summary>
        /// Save consent state to PlayerPrefs
        /// </summary>
        private void SaveConsentState()
        {
            PlayerPrefs.SetInt("privacy_consent", hasConsented ? 1 : 0);
            PlayerPrefs.SetInt("privacy_analytics", analyticsConsent ? 1 : 0);
            PlayerPrefs.SetInt("privacy_ads", adsConsent ? 1 : 0);
            PlayerPrefs.SetInt("privacy_personalization", personalizationConsent ? 1 : 0);
            PlayerPrefs.SetString("privacy_timestamp", consentTimestamp);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load consent state from PlayerPrefs
        /// </summary>
        private void LoadConsentState()
        {
            hasConsented = PlayerPrefs.GetInt("privacy_consent", 0) == 1;
            analyticsConsent = PlayerPrefs.GetInt("privacy_analytics", 0) == 1;
            adsConsent = PlayerPrefs.GetInt("privacy_ads", 0) == 1;
            personalizationConsent = PlayerPrefs.GetInt("privacy_personalization", 0) == 1;
            consentTimestamp = PlayerPrefs.GetString("privacy_timestamp", "");
        }

        /// <summary>
        /// Get privacy policy URL
        /// </summary>
        public string GetPrivacyPolicyUrl() => privacyPolicyUrl;

        /// <summary>
        /// Get terms of service URL
        /// </summary>
        public string GetTermsOfServiceUrl() => termsOfServiceUrl;
    }

    /// <summary>
    /// Consent types
    /// </summary>
    public enum ConsentType
    {
        Analytics,
        Ads,
        Personalization
    }

    /// <summary>
    /// Consent changed event data
    /// </summary>
    [System.Serializable]
    public class ConsentChangedEventArgs : EventArgs
    {
        public bool AnalyticsConsent;
        public bool AdsConsent;
        public bool PersonalizationConsent;
        public string Timestamp;
    }

    /// <summary>
    /// Privacy consent data for server storage
    /// Referência: BK-014
    /// </summary>
    [System.Serializable]
    public class PrivacyConsentData
    {
        public string user_id;
        public bool analytics_consent;
        public bool ads_consent;
        public bool personalization_consent;
        public string consent_timestamp;
        public string consent_version;
        public string ip_address;
        public string device_info;
    }
}
