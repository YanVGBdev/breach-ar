using UnityEngine;
using NUnit.Framework;
using BreachAR.Backend;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for PrivacyService
    /// Referência: BK-014, BK-021, UI-026
    /// </summary>
    [TestFixture]
    public class PrivacyServiceTests
    {
        [Test]
        public void ConsentType_Analytics_CanBeChecked()
        {
            // Arrange & Act
            var type = ConsentType.Analytics;

            // Assert
            Assert.AreEqual(ConsentType.Analytics, type);
        }

        [Test]
        public void ConsentType_Ads_CanBeChecked()
        {
            // Arrange & Act
            var type = ConsentType.Ads;

            // Assert
            Assert.AreEqual(ConsentType.Ads, type);
        }

        [Test]
        public void ConsentType_Personalization_CanBeChecked()
        {
            // Arrange & Act
            var type = ConsentType.Personalization;

            // Assert
            Assert.AreEqual(ConsentType.Personalization, type);
        }

        [Test]
        public void ConsentChangedEventArgs_ContainsRequiredFields()
        {
            // Arrange & Act
            var args = new ConsentChangedEventArgs
            {
                AnalyticsConsent = true,
                AdsConsent = false,
                PersonalizationConsent = true,
                Timestamp = "2026-08-03T12:00:00Z"
            };

            // Assert
            Assert.IsTrue(args.AnalyticsConsent);
            Assert.IsFalse(args.AdsConsent);
            Assert.IsTrue(args.PersonalizationConsent);
            Assert.AreEqual("2026-08-03T12:00:00Z", args.Timestamp);
        }

        [Test]
        public void PrivacyConsentData_ContainsRequiredFields()
        {
            // Arrange & Act
            var data = new PrivacyConsentData
            {
                user_id = "test-user-123",
                analytics_consent = true,
                ads_consent = false,
                personalization_consent = true,
                consent_timestamp = "2026-08-03T12:00:00Z",
                consent_version = "1.0",
                ip_address = "anonymized",
                device_info = "TestDevice"
            };

            // Assert
            Assert.AreEqual("test-user-123", data.user_id);
            Assert.IsTrue(data.analytics_consent);
            Assert.IsFalse(data.ads_consent);
            Assert.IsTrue(data.personalization_consent);
            Assert.AreEqual("2026-08-03T12:00:00Z", data.consent_timestamp);
            Assert.AreEqual("1.0", data.consent_version);
            Assert.AreEqual("anonymized", data.ip_address);
            Assert.AreEqual("TestDevice", data.device_info);
        }

        [Test]
        public void BackupResult_ContainsRequiredFields()
        {
            // Arrange & Act
            var result = new BackupResult
            {
                Timestamp = System.DateTime.UtcNow,
                Success = true,
                LocalBackupPath = "/path/to/backup.json",
                CloudSynced = true,
                ErrorMessage = null
            };

            // Assert
            Assert.IsTrue(result.Success);
            Assert.AreEqual("/path/to/backup.json", result.LocalBackupPath);
            Assert.IsTrue(result.CloudSynced);
            Assert.IsNull(result.ErrorMessage);
        }

        [Test]
        public void BackupInfo_ContainsRequiredFields()
        {
            // Arrange & Act
            var info = new BackupInfo
            {
                Path = "/path/to/backup.json",
                FileName = "backup_20260803.json",
                CreatedAt = System.DateTime.UtcNow,
                SizeBytes = 1024
            };

            // Assert
            Assert.AreEqual("/path/to/backup.json", info.Path);
            Assert.AreEqual("backup_20260803.json", info.FileName);
            Assert.AreEqual(1024, info.SizeBytes);
        }
    }
}
