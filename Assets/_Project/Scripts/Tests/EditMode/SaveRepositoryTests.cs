using UnityEngine;
using NUnit.Framework;
using BreachAR.Core;
using BreachAR.Backend;
using System.IO;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Unit tests for SaveRepository
    /// </summary>
    [TestFixture]
    public class SaveRepositoryTests
    {
        private SaveRepository saveRepository;
        private string testSavePath;

        [SetUp]
        public void SetUp()
        {
            // Use a unique test path for each test
            testSavePath = Path.Combine(Application.temporaryCachePath, $"test_save_{System.Guid.NewGuid()}.dat");
            saveRepository = new SaveRepository(testSavePath, "TestEncryptionKey");
        }

        [TearDown]
        public void TearDown()
        {
            // Clean up test save file
            if (File.Exists(testSavePath))
            {
                File.Delete(testSavePath);
            }
        }

        [Test]
        public void HasSave_NoFile_ReturnsFalse()
        {
            // Assert
            Assert.IsFalse(saveRepository.HasSave(), "Should not have save when file doesn't exist");
        }

        [Test]
        public void Load_NoFile_ReturnsDefaultSave()
        {
            // Act
            SaveData data = saveRepository.Load();

            // Assert
            Assert.IsNotNull(data, "Should return default save data");
            Assert.AreEqual(1, data.Level, "Default level should be 1");
            Assert.AreEqual(0, data.SoftCurrency, "Default soft currency should be 0");
            Assert.IsNotNull(data.Settings, "Default settings should not be null");
        }

        [Test]
        public void Save_CreatesFile()
        {
            // Arrange
            SaveData data = new SaveData
            {
                PlayerId = "test_player",
                Level = 5,
                SoftCurrency = 1000,
                HardCurrency = 50
            };

            // Act
            saveRepository.Save(data);

            // Assert
            Assert.IsTrue(saveRepository.HasSave(), "Save file should exist after saving");
        }

        [Test]
        public void SaveAndLoad_PreservesData()
        {
            // Arrange
            SaveData originalData = new SaveData
            {
                PlayerId = "test_player_123",
                Level = 10,
                Experience = 5000f,
                SoftCurrency = 2500,
                HardCurrency = 100,
                UnlockedOrbs = new string[] { "orb_basic", "orb_fire" },
                UnlockedSkins = new string[] { "skin_gold" }
            };

            // Act
            saveRepository.Save(originalData);
            SaveData loadedData = saveRepository.Load();

            // Assert
            Assert.AreEqual(originalData.PlayerId, loadedData.PlayerId, "Player ID should match");
            Assert.AreEqual(originalData.Level, loadedData.Level, "Level should match");
            Assert.AreEqual(originalData.Experience, loadedData.Experience, "Experience should match");
            Assert.AreEqual(originalData.SoftCurrency, loadedData.SoftCurrency, "Soft currency should match");
            Assert.AreEqual(originalData.HardCurrency, loadedData.HardCurrency, "Hard currency should match");
            Assert.AreEqual(originalData.UnlockedOrbs.Length, loadedData.UnlockedOrbs.Length, "Unlocked orbs count should match");
        }

        [Test]
        public void DeleteSave_RemovesFile()
        {
            // Arrange
            SaveData data = new SaveData { PlayerId = "test" };
            saveRepository.Save(data);
            Assert.IsTrue(saveRepository.HasSave(), "Save should exist");

            // Act
            saveRepository.DeleteSave();

            // Assert
            Assert.IsFalse(saveRepository.HasSave(), "Save should not exist after deletion");
        }

        [Test]
        public void Save_NullData_DoesNotThrow()
        {
            // Act & Assert - Should not throw exception
            Assert.DoesNotThrow(() => saveRepository.Save(null), "Should not throw when saving null");
        }

        [Test]
        public void Save_UpdatesTimestamp()
        {
            // Arrange
            SaveData data = new SaveData
            {
                PlayerId = "test",
                LastSaveTimestamp = 0
            };

            // Act
            saveRepository.Save(data);
            SaveData loaded = saveRepository.Load();

            // Assert
            Assert.IsTrue(loaded.LastSaveTimestamp > 0, "Timestamp should be updated");
        }

        [Test]
        public void MultipleSaves_OverwritesPrevious()
        {
            // Arrange
            SaveData data1 = new SaveData { Level = 1 };
            SaveData data2 = new SaveData { Level = 2 };

            // Act
            saveRepository.Save(data1);
            saveRepository.Save(data2);
            SaveData loaded = saveRepository.Load();

            // Assert
            Assert.AreEqual(2, loaded.Level, "Should have latest save data");
        }
    }
}
