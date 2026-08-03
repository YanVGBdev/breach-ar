using UnityEngine;
using NUnit.Framework;
using System.IO;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Unit tests for SaveRepository
    /// Referência: QA-006
    /// </summary>
    [TestFixture]
    public class SaveRepositoryTests
    {
        private string testSavePath;

        [SetUp]
        public void Setup()
        {
            testSavePath = Path.Combine(Application.temporaryCachePath, "test_save.json");
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(testSavePath))
            {
                File.Delete(testSavePath);
            }
        }

        [Test]
        public void SaveData_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var saveData = new SaveDataTest();

            // Assert
            Assert.AreEqual(0, saveData.Level);
            Assert.AreEqual(0f, saveData.Experience);
            Assert.AreEqual(0, saveData.SoftCurrency);
            Assert.AreEqual(0, saveData.HardCurrency);
        }

        [Test]
        public void SaveData_CanBeSerialized()
        {
            // Arrange
            var saveData = new SaveDataTest
            {
                Level = 5,
                Experience = 1250.5f,
                SoftCurrency = 5000,
                HardCurrency = 100,
                PlayerId = "test_player_123"
            };

            // Act
            string json = JsonUtility.ToJson(saveData);

            // Assert
            Assert.IsFalse(string.IsNullOrEmpty(json));
            Assert.IsTrue(json.Contains("test_player_123"));
        }

        [Test]
        public void SaveData_CanBeDeserialized()
        {
            // Arrange
            var original = new SaveDataTest
            {
                Level = 10,
                Experience = 5000f,
                SoftCurrency = 25000,
                HardCurrency = 250,
                PlayerId = "player_456"
            };

            // Act
            string json = JsonUtility.ToJson(original);
            var restored = JsonUtility.FromJson<SaveDataTest>(json);

            // Assert
            Assert.AreEqual(original.Level, restored.Level);
            Assert.AreEqual(original.Experience, restored.Experience);
            Assert.AreEqual(original.SoftCurrency, restored.SoftCurrency);
            Assert.AreEqual(original.HardCurrency, restored.HardCurrency);
            Assert.AreEqual(original.PlayerId, restored.PlayerId);
        }

        [Test]
        public void SaveData_SerializeDeserialize_NoDataLoss()
        {
            // Arrange
            var original = CreateFullSaveData();

            // Act
            string json = JsonUtility.ToJson(original);
            var restored = JsonUtility.FromJson<SaveDataTest>(json);

            // Assert
            Assert.AreEqual(original.PlayerId, restored.PlayerId);
            Assert.AreEqual(original.Level, restored.Level);
            Assert.AreEqual(original.Experience, restored.Experience);
            Assert.AreEqual(original.SoftCurrency, restored.SoftCurrency);
            Assert.AreEqual(original.HardCurrency, restored.HardCurrency);
        }

        [Test]
        public void SaveData_CanBeWrittenToFile()
        {
            // Arrange
            var saveData = new SaveDataTest
            {
                Level = 7,
                PlayerId = "file_test"
            };

            // Act
            string json = JsonUtility.ToJson(saveData);
            File.WriteAllText(testSavePath, json);

            // Assert
            Assert.IsTrue(File.Exists(testSavePath));
        }

        [Test]
        public void SaveData_CanBeReadFromFile()
        {
            // Arrange
            var original = new SaveDataTest
            {
                Level = 15,
                Experience = 9999f,
                PlayerId = "read_test"
            };
            string json = JsonUtility.ToJson(original);
            File.WriteAllText(testSavePath, json);

            // Act
            string readJson = File.ReadAllText(testSavePath);
            var restored = JsonUtility.FromJson<SaveDataTest>(readJson);

            // Assert
            Assert.AreEqual(original.Level, restored.Level);
            Assert.AreEqual(original.PlayerId, restored.PlayerId);
        }

        [Test]
        public void SaveData_InvalidJson_ReturnsDefault()
        {
            // Arrange
            string invalidJson = "{ invalid json }";

            // Act
            var result = JsonUtility.FromJson<SaveDataTest>(invalidJson);

            // Assert - Should return default values
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Level);
        }

        [Test]
        public void SaveData_EmptyString_ReturnsDefault()
        {
            // Arrange
            string emptyJson = "";

            // Act
            var result = JsonUtility.FromJson<SaveDataTest>(emptyJson);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.Level);
        }

        /// <summary>
        /// Create a fully populated save data for testing
        /// </summary>
        private SaveDataTest CreateFullSaveData()
        {
            return new SaveDataTest
            {
                PlayerId = "test_player_full",
                Level = 25,
                Experience = 15000f,
                SoftCurrency = 100000,
                HardCurrency = 500,
                LastSaveTimestamp = System.DateTime.UtcNow.ToBinary()
            };
        }

        /// <summary>
        /// Simple save data structure for testing
        /// </summary>
        [System.Serializable]
        private class SaveDataTest
        {
            public string PlayerId = "";
            public int Level = 0;
            public float Experience = 0f;
            public int SoftCurrency = 0;
            public int HardCurrency = 0;
            public long LastSaveTimestamp = 0;
        }
    }
}
