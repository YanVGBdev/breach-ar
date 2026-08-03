using UnityEngine;
using NUnit.Framework;
using BreachAR.Backend;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Unit tests for EconomyService
    /// </summary>
    [TestFixture]
    public class EconomyServiceTests
    {
        private EconomyService economyService;
        private GameObject testObject;

        [SetUp]
        public void SetUp()
        {
            testObject = new GameObject();
            economyService = testObject.AddComponent<EconomyService>();
            economyService.LoadFromData(1000, 50); // Starting amounts
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testObject);
        }

        [Test]
        public void InitialState_HasCorrectAmounts()
        {
            // Assert
            Assert.AreEqual(1000, economyService.SoftCurrency, "Initial soft currency should be 1000");
            Assert.AreEqual(50, economyService.HardCurrency, "Initial hard currency should be 50");
        }

        [Test]
        public void AddSoftCurrency_IncreasesAmount()
        {
            // Act
            economyService.AddSoftCurrency(500);

            // Assert
            Assert.AreEqual(1500, economyService.SoftCurrency, "Soft currency should increase");
        }

        [Test]
        public void AddSoftCurrency_NegativeAmount_ReturnsFalse()
        {
            // Act
            bool result = economyService.AddSoftCurrency(-100);

            // Assert
            Assert.IsFalse(result, "Should return false for negative amount");
            Assert.AreEqual(1000, economyService.SoftCurrency, "Amount should not change");
        }

        [Test]
        public void AddSoftCurrency_ZeroAmount_ReturnsFalse()
        {
            // Act
            bool result = economyService.AddSoftCurrency(0);

            // Assert
            Assert.IsFalse(result, "Should return false for zero amount");
        }

        [Test]
        public void SpendSoftCurrency_DecreasesAmount()
        {
            // Act
            bool result = economyService.SpendSoftCurrency(300);

            // Assert
            Assert.IsTrue(result, "Should return true for successful spend");
            Assert.AreEqual(700, economyService.SoftCurrency, "Soft currency should decrease");
        }

        [Test]
        public void SpendSoftCurrency_InsufficientFunds_ReturnsFalse()
        {
            // Act
            bool result = economyService.SpendSoftCurrency(2000);

            // Assert
            Assert.IsFalse(result, "Should return false for insufficient funds");
            Assert.AreEqual(1000, economyService.SoftCurrency, "Amount should not change");
        }

        [Test]
        public void AddHardCurrency_IncreasesAmount()
        {
            // Act
            economyService.AddHardCurrency(25);

            // Assert
            Assert.AreEqual(75, economyService.HardCurrency, "Hard currency should increase");
        }

        [Test]
        public void SpendHardCurrency_DecreasesAmount()
        {
            // Act
            bool result = economyService.SpendHardCurrency(30);

            // Assert
            Assert.IsTrue(result, "Should return true for successful spend");
            Assert.AreEqual(20, economyService.HardCurrency, "Hard currency should decrease");
        }

        [Test]
        public void SpendHardCurrency_InsufficientFunds_ReturnsFalse()
        {
            // Act
            bool result = economyService.SpendHardCurrency(100);

            // Assert
            Assert.IsFalse(result, "Should return false for insufficient funds");
            Assert.AreEqual(50, economyService.HardCurrency, "Amount should not change");
        }

        [Test]
        public void CalculateSessionRewards_ReturnsPositiveValues()
        {
            // Act
            SessionRewards rewards = economyService.CalculateSessionRewards(1000, 5, true);

            // Assert
            Assert.IsTrue(rewards.SoftCurrency > 0, "Soft currency reward should be positive");
            Assert.IsTrue(rewards.Experience > 0, "Experience should be positive");
            Assert.IsTrue(rewards.BattlePassXP > 0, "Battle pass XP should be positive");
        }

        [Test]
        public void ApplySessionRewards_IncreasesSoftCurrency()
        {
            // Arrange
            SessionRewards rewards = new SessionRewards
            {
                SoftCurrency = 200,
                Experience = 100,
                BattlePassXP = 50
            };

            // Act
            economyService.ApplySessionRewards(rewards);

            // Assert
            Assert.AreEqual(1200, economyService.SoftCurrency, "Soft currency should increase by reward");
        }

        [Test]
        public void GetSaveData_ReturnsCorrectData()
        {
            // Act
            EconomySaveData saveData = economyService.GetSaveData();

            // Assert
            Assert.AreEqual(1000, saveData.SoftCurrency, "Save data should have correct soft currency");
            Assert.AreEqual(50, saveData.HardCurrency, "Save data should have correct hard currency");
        }

        [Test]
        public void MultipleTransactions_MaintainCorrectBalance()
        {
            // Act
            economyService.AddSoftCurrency(500);
            economyService.SpendSoftCurrency(200);
            economyService.AddSoftCurrency(100);
            economyService.SpendSoftCurrency(300);

            // Assert
            Assert.AreEqual(1100, economyService.SoftCurrency, "Balance should be correct after transactions");
        }
    }
}
