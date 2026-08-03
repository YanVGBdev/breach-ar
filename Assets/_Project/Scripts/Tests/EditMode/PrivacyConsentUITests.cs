using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.UI;
using BreachAR.Core;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for PrivacyConsentUI
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class PrivacyConsentUITests
    {
        [SetUp]
        public void SetUp()
        {
            PlayerPrefs.DeleteKey("PrivacyConsentMade");
            PlayerPrefs.DeleteKey("AnalyticsConsent");
            PlayerPrefs.DeleteKey("AdsConsent");
            PlayerPrefs.DeleteKey("PersonalizationConsent");
        }

        [Test]
        public void PrivacyConsentUI_InitialState_IsHidden()
        {
            var gameObject = new GameObject();
            var consentUI = gameObject.AddComponent<PrivacyConsentUI>();

            Assert.IsFalse(consentUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void PrivacyConsentUI_Show_ActivatesGameObject()
        {
            var gameObject = new GameObject();
            var consentUI = gameObject.AddComponent<PrivacyConsentUI>();

            consentUI.Show();

            Assert.IsTrue(consentUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void PrivacyConsentUI_Hide_DeactivatesGameObject()
        {
            var gameObject = new GameObject();
            var consentUI = gameObject.AddComponent<PrivacyConsentUI>();
            gameObject.SetActive(true);

            consentUI.Hide();

            Assert.IsFalse(consentUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void PrivacyConsentUI_HasConsent_ReturnsFalseByDefault()
        {
            var gameObject = new GameObject();
            var consentUI = gameObject.AddComponent<PrivacyConsentUI>();

            Assert.IsFalse(consentUI.HasConsent());
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void PrivacyConsentUI_GetAnalyticsConsent_ReturnsFalseByDefault()
        {
            var gameObject = new GameObject();
            var consentUI = gameObject.AddComponent<PrivacyConsentUI>();

            Assert.IsFalse(consentUI.GetAnalyticsConsent());
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void PrivacyConsentUI_GetAdsConsent_ReturnsFalseByDefault()
        {
            var gameObject = new GameObject();
            var consentUI = gameObject.AddComponent<PrivacyConsentUI>();

            Assert.IsFalse(consentUI.GetAdsConsent());
            
            Object.DestroyImmediate(gameObject);
        }

        [TearDown]
        public void TearDown()
        {
            PlayerPrefs.DeleteKey("PrivacyConsentMade");
            PlayerPrefs.DeleteKey("AnalyticsConsent");
            PlayerPrefs.DeleteKey("AdsConsent");
            PlayerPrefs.DeleteKey("PersonalizationConsent");
        }
    }
}
