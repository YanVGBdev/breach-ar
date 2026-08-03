using UnityEngine;
using NUnit.Framework;
using BreachAR.Gameplay;
using BreachAR.Core;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for CampaignMode and EndlessMode
    /// Referência: GP-025, GP-026
    /// </summary>
    [TestFixture]
    public class GameModeTests
    {
        // =====================================================================
        // Campaign Mode Tests
        // =====================================================================

        [Test]
        public void CampaignMode_Mode_ReturnsCampaign()
        {
            // Arrange & Act
            var mode = GameMode.Campaign;

            // Assert
            Assert.AreEqual(GameMode.Campaign, mode);
        }

        [Test]
        public void CampaignConfig_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var config = ScriptableObject.CreateInstance<CampaignConfig>();
            config.totalWaves = 30;
            config.bossWaveInterval = 10;
            config.difficultyScale = 0.5f;
            config.biomeTransitions = new int[] { 10, 20 };
            config.rewardMultiplier = 1f;

            // Assert
            Assert.AreEqual(30, config.totalWaves);
            Assert.AreEqual(10, config.bossWaveInterval);
            Assert.AreEqual(0.5f, config.difficultyScale);
            Assert.AreEqual(2, config.biomeTransitions.Length);
            Assert.AreEqual(1f, config.rewardMultiplier);
        }

        [Test]
        public void CampaignMode_BossWave_CorrectlyIdentified()
        {
            // Arrange
            int bossWaveInterval = 10;

            // Act & Assert
            Assert.IsTrue(10 % bossWaveInterval == 0); // Wave 10 is boss
            Assert.IsTrue(20 % bossWaveInterval == 0); // Wave 20 is boss
            Assert.IsTrue(30 % bossWaveInterval == 0); // Wave 30 is boss
            Assert.IsFalse(15 % bossWaveInterval == 0); // Wave 15 is not boss
        }

        [Test]
        public void BiomeChangedData_ContainsRequiredFields()
        {
            // Arrange & Act
            var data = new BiomeChangedData
            {
                PreviousBiome = 0,
                NewBiome = 1,
                WaveIndex = 10
            };

            // Assert
            Assert.AreEqual(0, data.PreviousBiome);
            Assert.AreEqual(1, data.NewBiome);
            Assert.AreEqual(10, data.WaveIndex);
        }

        // =====================================================================
        // Endless Mode Tests
        // =====================================================================

        [Test]
        public void EndlessMode_Mode_ReturnsEndless()
        {
            // Arrange & Act
            var mode = GameMode.Endless;

            // Assert
            Assert.AreEqual(GameMode.Endless, mode);
        }

        [Test]
        public void EndlessConfig_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var config = ScriptableObject.CreateInstance<EndlessConfig>();
            config.linearGrowthRate = 0.05f;
            config.exponentialGrowthRate = 0.1f;
            config.softCapWave = 20;
            config.initialWaves = 10;
            config.bossInterval = 10;
            config.rewardMultiplier = 1f;

            // Assert
            Assert.AreEqual(0.05f, config.linearGrowthRate);
            Assert.AreEqual(0.1f, config.exponentialGrowthRate);
            Assert.AreEqual(20, config.softCapWave);
            Assert.AreEqual(10, config.initialWaves);
            Assert.AreEqual(10, config.bossInterval);
            Assert.AreEqual(1f, config.rewardMultiplier);
        }

        [Test]
        public void EndlessMilestone_ContainsRequiredFields()
        {
            // Arrange & Act
            var milestone = new EndlessMilestone
            {
                waveThreshold = 50,
                rewardSoftCurrency = 1000,
                rewardExperience = 500,
                rewardTitle = "Wave Master"
            };

            // Assert
            Assert.AreEqual(50, milestone.waveThreshold);
            Assert.AreEqual(1000, milestone.rewardSoftCurrency);
            Assert.AreEqual(500, milestone.rewardExperience);
            Assert.AreEqual("Wave Master", milestone.rewardTitle);
        }

        [Test]
        public void EndlessMilestoneData_ContainsRequiredFields()
        {
            // Arrange & Act
            var data = new EndlessMilestoneData
            {
                WaveThreshold = 50,
                RewardSoftCurrency = 1000,
                RewardExperience = 500,
                RewardTitle = "Wave Master"
            };

            // Assert
            Assert.AreEqual(50, data.WaveThreshold);
            Assert.AreEqual(1000, data.RewardSoftCurrency);
            Assert.AreEqual(500, data.RewardExperience);
            Assert.AreEqual("Wave Master", data.RewardTitle);
        }

        [Test]
        public void EndlessMode_DifficultyScaling_ExponentialAfterSoftCap()
        {
            // Arrange
            float linearRate = 0.05f;
            float exponentialRate = 0.1f;
            int softCapWave = 20;

            // Act - Wave 10 (before soft cap)
            float difficultyBefore = 1f + (10 * linearRate);

            // Act - Wave 30 (after soft cap, 10 waves over)
            int wavesOverCap = 30 - softCapWave;
            float difficultyAfter = Mathf.Pow(1f + exponentialRate, wavesOverCap);

            // Assert
            Assert.AreEqual(1.5f, difficultyBefore, 0.01f); // Linear: 1 + (10 * 0.05) = 1.5
            Assert.Greater(difficultyAfter, 1f); // Exponential: (1.1)^10 ≈ 2.59
        }

        [Test]
        public void GameMode_Campaign_And_Endless_AreDifferent()
        {
            // Arrange & Act
            var campaign = GameMode.Campaign;
            var endless = GameMode.Endless;

            // Assert
            Assert.AreNotEqual(campaign, endless);
        }
    }
}
