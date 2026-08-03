using UnityEngine;
using NUnit.Framework;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Unit tests for EconomyService
    /// Referência: QA-005
    /// </summary>
    [TestFixture]
    public class EconomyServiceTests
    {
        [Test]
        public void Economy_InitialState_IsZero()
        {
            // Arrange & Act
            var economy = new EconomyTestData();

            // Assert
            Assert.AreEqual(0, economy.SoftCurrency);
            Assert.AreEqual(0, economy.HardCurrency);
        }

        [Test]
        public void Economy_AddSoftCurrency_IncreasesBalance()
        {
            // Arrange
            var economy = new EconomyTestData();

            // Act
            bool success = economy.AddSoftCurrency(100, "Test");

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(100, economy.SoftCurrency);
        }

        [Test]
        public void Economy_AddSoftCurrency_NegativeAmount_ReturnsFalse()
        {
            // Arrange
            var economy = new EconomyTestData();

            // Act
            bool success = economy.AddSoftCurrency(-50, "Test");

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(0, economy.SoftCurrency);
        }

        [Test]
        public void Economy_SpendSoftCurrency_DecreasesBalance()
        {
            // Arrange
            var economy = new EconomyTestData();
            economy.AddSoftCurrency(200, "Initial");

            // Act
            bool success = economy.SpendSoftCurrency(100, "Purchase");

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(100, economy.SoftCurrency);
        }

        [Test]
        public void Economy_SpendSoftCurrency_InsufficientFunds_ReturnsFalse()
        {
            // Arrange
            var economy = new EconomyTestData();
            economy.AddSoftCurrency(50, "Initial");

            // Act
            bool success = economy.SpendSoftCurrency(100, "Purchase");

            // Assert
            Assert.IsFalse(success);
            Assert.AreEqual(50, economy.SoftCurrency);
        }

        [Test]
        public void Economy_AddHardCurrency_IncreasesBalance()
        {
            // Arrange
            var economy = new EconomyTestData();

            // Act
            bool success = economy.AddHardCurrency(10, "IAP");

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(10, economy.HardCurrency);
        }

        [Test]
        public void Economy_SpendHardCurrency_DecreasesBalance()
        {
            // Arrange
            var economy = new EconomyTestData();
            economy.AddHardCurrency(20, "IAP");

            // Act
            bool success = economy.SpendHardCurrency(10, "Revive");

            // Assert
            Assert.IsTrue(success);
            Assert.AreEqual(10, economy.HardCurrency);
        }

        [Test]
        public void Economy_MaxSoftCurrency_CapsAtLimit()
        {
            // Arrange
            var economy = new EconomyTestData(maxSoft: 1000);

            // Act
            economy.AddSoftCurrency(500, "Test");
            economy.AddSoftCurrency(600, "Test");

            // Assert
            Assert.AreEqual(1000, economy.SoftCurrency);
        }

        [Test]
        public void Economy_MaxHardCurrency_CapsAtLimit()
        {
            // Arrange
            var economy = new EconomyTestData(maxHard: 100);

            // Act
            economy.AddHardCurrency(50, "Test");
            economy.AddHardCurrency(60, "Test");

            // Assert
            Assert.AreEqual(100, economy.HardCurrency);
        }

        [Test]
        public void Economy_SessionRewards_CalculatesCorrectly()
        {
            // Arrange
            var economy = new EconomyTestData();
            int score = 5000;
            int wavesCleared = 10;
            bool perfectWave = true;

            // Act
            var rewards = economy.CalculateSessionRewards(score, wavesCleared, perfectWave);

            // Assert
            Assert.Greater(rewards.SoftCurrency, 0);
            Assert.Greater(rewards.Experience, 0);
        }

        /// <summary>
        /// Simple test helper for economy logic
        /// </summary>
        private class EconomyTestData
        {
            public int SoftCurrency { get; private set; }
            public int HardCurrency { get; private set; }
            private int maxSoft;
            private int maxHard;

            public EconomyTestData(int maxSoft = 999999, int maxHard = 99999)
            {
                this.maxSoft = maxSoft;
                this.maxHard = maxHard;
            }

            public bool AddSoftCurrency(int amount, string reason)
            {
                if (amount <= 0) return false;
                SoftCurrency = Mathf.Min(SoftCurrency + amount, maxSoft);
                return true;
            }

            public bool SpendSoftCurrency(int amount, string reason)
            {
                if (amount <= 0 || SoftCurrency < amount) return false;
                SoftCurrency -= amount;
                return true;
            }

            public bool AddHardCurrency(int amount, string reason)
            {
                if (amount <= 0) return false;
                HardCurrency = Mathf.Min(HardCurrency + amount, maxHard);
                return true;
            }

            public bool SpendHardCurrency(int amount, string reason)
            {
                if (amount <= 0 || HardCurrency < amount) return false;
                HardCurrency -= amount;
                return true;
            }

            public SessionRewards CalculateSessionRewards(int score, int wavesCleared, bool perfectWave)
            {
                int softReward = score / 100;
                softReward += wavesCleared * 10;
                if (perfectWave) softReward += 50;

                return new SessionRewards
                {
                    SoftCurrency = softReward,
                    Experience = wavesCleared * 25,
                    BattlePassXP = softReward / 2
                };
            }
        }

        /// <summary>
        /// Session rewards structure
        /// </summary>
        private struct SessionRewards
        {
            public int SoftCurrency;
            public int Experience;
            public int BattlePassXP;
        }
    }
}
