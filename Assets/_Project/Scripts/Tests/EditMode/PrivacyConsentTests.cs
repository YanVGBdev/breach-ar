using UnityEngine;
using NUnit.Framework;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Privacy consent tests
    /// Referência: QA-039
    /// </summary>
    [TestFixture]
    public class PrivacyConsentTests
    {
        [Test]
        public void Consent_InitialState_IsNotGiven()
        {
            // Arrange
            var consent = new PrivacyConsentTest();

            // Assert
            Assert.IsFalse(consent.HasConsented);
        }

        [Test]
        public void Consent_GrantAll_SetsAllFlags()
        {
            // Arrange
            var consent = new PrivacyConsentTest();

            // Act
            consent.GrantAll();

            // Assert
            Assert.IsTrue(consent.HasConsented);
            Assert.IsTrue(consent.AnalyticsConsent);
            Assert.IsTrue(consent.AdsConsent);
            Assert.IsTrue(consent.PersonalizationConsent);
        }

        [Test]
        public void Consent_GrantSelected_OnlySetsSelected()
        {
            // Arrange
            var consent = new PrivacyConsentTest();

            // Act
            consent.SetConsent(analytics: true, ads: false, personalization: true);

            // Assert
            Assert.IsTrue(consent.HasConsented);
            Assert.IsTrue(consent.AnalyticsConsent);
            Assert.IsFalse(consent.AdsConsent);
            Assert.IsTrue(consent.PersonalizationConsent);
        }

        [Test]
        public void Consent_RejectAll_ClearsAllFlags()
        {
            // Arrange
            var consent = new PrivacyConsentTest();
            consent.GrantAll();

            // Act
            consent.RejectAll();

            // Assert
            Assert.IsTrue(consent.HasConsented);
            Assert.IsFalse(consent.AnalyticsConsent);
            Assert.IsFalse(consent.AdsConsent);
            Assert.IsFalse(consent.PersonalizationConsent);
        }

        [Test]
        public void Consent_AnalyticsBlocked_BeforeConsent()
        {
            // Arrange
            var consent = new PrivacyConsentTest();

            // Act
            bool canTrack = consent.CanTrackAnalytics();

            // Assert
            Assert.IsFalse(canTrack);
        }

        [Test]
        public void Consent_AnalyticsAllowed_AfterConsent()
        {
            // Arrange
            var consent = new PrivacyConsentTest();
            consent.SetConsent(analytics: true, ads: false, personalization: false);

            // Act
            bool canTrack = consent.CanTrackAnalytics();

            // Assert
            Assert.IsTrue(canTrack);
        }

        [Test]
        public void Consent_AdsBlocked_BeforeConsent()
        {
            // Arrange
            var consent = new PrivacyConsentTest();

            // Act
            bool canShowAds = consent.CanShowAds();

            // Assert
            Assert.IsFalse(canShowAds);
        }

        [Test]
        public void Consent_AdsAllowed_AfterConsent()
        {
            // Arrange
            var consent = new PrivacyConsentTest();
            consent.SetConsent(analytics: false, ads: true, personalization: false);

            // Act
            bool canShowAds = consent.CanShowAds();

            // Assert
            Assert.IsTrue(canShowAds);
        }

        [Test]
        public void Consent_CanBeRevoked()
        {
            // Arrange
            var consent = new PrivacyConsentTest();
            consent.GrantAll();

            // Act
            consent.RevokeConsent();

            // Assert
            Assert.IsFalse(consent.HasConsented);
            Assert.IsFalse(consent.AnalyticsConsent);
            Assert.IsFalse(consent.AdsConsent);
        }

        [Test]
        public void Consent_TimestampRecorded()
        {
            // Arrange
            var consent = new PrivacyConsentTest();

            // Act
            consent.GrantAll();

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(consent.ConsentTimestamp));
        }

        /// <summary>
        /// Simple privacy consent test helper
        /// </summary>
        private class PrivacyConsentTest
        {
            public bool HasConsented { get; private set; }
            public bool AnalyticsConsent { get; private set; }
            public bool AdsConsent { get; private set; }
            public bool PersonalizationConsent { get; private set; }
            public string ConsentTimestamp { get; private set; }

            public void GrantAll()
            {
                HasConsented = true;
                AnalyticsConsent = true;
                AdsConsent = true;
                PersonalizationConsent = true;
                ConsentTimestamp = System.DateTime.UtcNow.ToString("o");
            }

            public void SetConsent(bool analytics, bool ads, bool personalization)
            {
                HasConsented = true;
                AnalyticsConsent = analytics;
                AdsConsent = ads;
                PersonalizationConsent = personalization;
                ConsentTimestamp = System.DateTime.UtcNow.ToString("o");
            }

            public void RejectAll()
            {
                HasConsented = true;
                AnalyticsConsent = false;
                AdsConsent = false;
                PersonalizationConsent = false;
                ConsentTimestamp = System.DateTime.UtcNow.ToString("o");
            }

            public void RevokeConsent()
            {
                HasConsented = false;
                AnalyticsConsent = false;
                AdsConsent = false;
                PersonalizationConsent = false;
            }

            public bool CanTrackAnalytics()
            {
                return HasConsented && AnalyticsConsent;
            }

            public bool CanShowAds()
            {
                return HasConsented && AdsConsent;
            }
        }
    }
}
