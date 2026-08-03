using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Core;
using BreachAR.Analytics;
using VContainer;

namespace BreachAR.UI
{
    /// <summary>
    /// Privacy consent UI for LGPD/GDPR compliance
    /// Referência: UI-026, 17_security.md
    /// </summary>
    public class PrivacyConsentUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI detailsText;

        [Header("Toggles")]
        [SerializeField] private Toggle analyticsConsentToggle;
        [SerializeField] private Toggle adsConsentToggle;
        [SerializeField] private Toggle personalizationConsentToggle;

        [Header("Buttons")]
        [SerializeField] private Button acceptAllButton;
        [SerializeField] private Button acceptSelectedButton;
        [SerializeField] private Button rejectAllButton;
        [SerializeField] private Button privacyPolicyButton;

        [Header("Settings")]
        [SerializeField] private GameObject settingsPanel;

        private bool hasMadeChoice;

        [Inject] private AnalyticsService analyticsService;

        private void Start()
        {
            SetupButtons();
            LoadConsentState();
        }

        private void SetupButtons()
        {
            acceptAllButton?.onClick.AddListener(OnAcceptAll);
            acceptSelectedButton?.onClick.AddListener(OnAcceptSelected);
            rejectAllButton?.onClick.AddListener(OnRejectAll);
            privacyPolicyButton?.onClick.AddListener(OnPrivacyPolicy);
        }

        private void LoadConsentState()
        {
            hasMadeChoice = PlayerPrefs.GetInt("PrivacyConsentMade", 0) == 1;

            if (hasMadeChoice)
            {
                if (analyticsConsentToggle != null)
                    analyticsConsentToggle.isOn = PlayerPrefs.GetInt("AnalyticsConsent", 0) == 1;
                if (adsConsentToggle != null)
                    adsConsentToggle.isOn = PlayerPrefs.GetInt("AdsConsent", 0) == 1;
                if (personalizationConsentToggle != null)
                    personalizationConsentToggle.isOn = PlayerPrefs.GetInt("PersonalizationConsent", 0) == 1;
            }
        }

        private void OnAcceptAll()
        {
            SetConsent(true, true, true);
            Hide();
        }

        private void OnAcceptSelected()
        {
            bool analytics = analyticsConsentToggle != null && analyticsConsentToggle.isOn;
            bool ads = adsConsentToggle != null && adsConsentToggle.isOn;
            bool personalization = personalizationConsentToggle != null && personalizationConsentToggle.isOn;

            SetConsent(analytics, ads, personalization);
            Hide();
        }

        private void OnRejectAll()
        {
            SetConsent(false, false, false);
            Hide();
        }

        private void OnPrivacyPolicy()
        {
            Debug.Log("[Privacy] Opening privacy policy");
            Application.OpenURL("https://breachar.com/privacy");
        }

        private void SetConsent(bool analytics, bool ads, bool personalization)
        {
            PlayerPrefs.SetInt("PrivacyConsentMade", 1);
            PlayerPrefs.SetInt("AnalyticsConsent", analytics ? 1 : 0);
            PlayerPrefs.SetInt("AdsConsent", ads ? 1 : 0);
            PlayerPrefs.SetInt("PersonalizationConsent", personalization ? 1 : 0);
            PlayerPrefs.Save();

            hasMadeChoice = true;

            // Update analytics service
            analyticsService?.SetConsent(analytics);

            Debug.Log($"[Privacy] Consent set - Analytics: {analytics}, Ads: {ads}, Personalization: {personalization}");
        }

        /// <summary>
        /// Show consent dialog
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide consent dialog
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Check if consent has been given
        /// </summary>
        public bool HasConsent()
        {
            return hasMadeChoice;
        }

        /// <summary>
        /// Get analytics consent
        /// </summary>
        public bool GetAnalyticsConsent()
        {
            return PlayerPrefs.GetInt("AnalyticsConsent", 0) == 1;
        }

        /// <summary>
        /// Get ads consent
        /// </summary>
        public bool GetAdsConsent()
        {
            return PlayerPrefs.GetInt("AdsConsent", 0) == 1;
        }
    }
}
