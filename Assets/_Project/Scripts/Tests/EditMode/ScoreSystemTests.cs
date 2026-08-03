using UnityEngine;
using NUnit.Framework;
using BreachAR.Gameplay;
using BreachAR.Core;
using BreachAR.ScriptableObjects;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Unit tests for ScoreSystem
    /// </summary>
    [TestFixture]
    public class ScoreSystemTests
    {
        private ScoreSystem scoreSystem;
        private ComboSystem comboSystem;
        private GameObject testObject;

        [SetUp]
        public void SetUp()
        {
            testObject = new GameObject();
            comboSystem = testObject.AddComponent<ComboSystem>();
            scoreSystem = testObject.AddComponent<ScoreSystem>();
            
            // Activate combo system for tests
            comboSystem.Activate();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testObject);
        }

        [Test]
        public void InitialState_HasZeroScore()
        {
            // Assert
            Assert.AreEqual(0, scoreSystem.CurrentScore, "Initial score should be 0");
            Assert.AreEqual(0, scoreSystem.TotalFragmentsKilled, "Initial fragments killed should be 0");
            Assert.AreEqual(0, scoreSystem.TotalRiftsClosed, "Initial rifts closed should be 0");
        }

        [Test]
        public void AddFragmentKillScore_CommonFragment_AddsCorrectScore()
        {
            // Act
            scoreSystem.AddFragmentKillScore(FragmentType.Common);

            // Assert
            Assert.AreEqual(100, scoreSystem.CurrentScore, "Score should be 100 for common fragment");
            Assert.AreEqual(1, scoreSystem.TotalFragmentsKilled, "Fragments killed should be 1");
        }

        [Test]
        public void AddFragmentKillScore_EliteFragment_AddsCorrectScore()
        {
            // Act
            scoreSystem.AddFragmentKillScore(FragmentType.Elite);

            // Assert
            Assert.AreEqual(250, scoreSystem.CurrentScore, "Score should be 250 for elite fragment");
        }

        [Test]
        public void AddFragmentKillScore_WithCombo_MultipliesScore()
        {
            // Arrange - Build up combo
            comboSystem.RegisterHit(); // 1.1x
            comboSystem.RegisterHit(); // 1.2x

            // Act
            scoreSystem.AddFragmentKillScore(FragmentType.Common);

            // Assert - 100 * 1.2 = 120
            Assert.AreEqual(120, scoreSystem.CurrentScore, "Score should be multiplied by combo");
        }

        [Test]
        public void AddFragmentKillScore_WithExtraTargets_AddsBonus()
        {
            // Act - Kill with 2 extra targets (multi-kill)
            scoreSystem.AddFragmentKillScore(FragmentType.Common, 2);

            // Assert - 100 + (2 * 50) = 200
            Assert.AreEqual(200, scoreSystem.CurrentScore, "Score should include multi-kill bonus");
        }

        [Test]
        public void AddRiftClosedScore_AddsCorrectScore()
        {
            // Act
            scoreSystem.AddRiftClosedScore(SurfaceType.Wall);

            // Assert
            Assert.AreEqual(500, scoreSystem.CurrentScore, "Score should be 500 for closing rift");
            Assert.AreEqual(1, scoreSystem.TotalRiftsClosed, "Rifts closed should be 1");
        }

        [Test]
        public void AddBossDefeatedScore_AddsCorrectScore()
        {
            // Act
            scoreSystem.AddBossDefeatedScore("boss_01", 60f);

            // Assert
            Assert.AreEqual(5000, scoreSystem.CurrentScore, "Score should be 5000 for boss defeat");
        }

        [Test]
        public void AddPowerUpScore_AddsCorrectScore()
        {
            // Act
            scoreSystem.AddPowerUpScore("powerup_shield");

            // Assert
            Assert.AreEqual(25, scoreSystem.CurrentScore, "Score should be 25 for power-up");
        }

        [Test]
        public void AddPerfectWaveBonus_AddsCorrectScore()
        {
            // Act
            scoreSystem.AddPerfectWaveBonus();

            // Assert
            Assert.AreEqual(1000, scoreSystem.CurrentScore, "Score should be 1000 for perfect wave");
        }

        [Test]
        public void ResetScore_ResetsAllValues()
        {
            // Arrange
            scoreSystem.AddFragmentKillScore(FragmentType.Common);
            scoreSystem.AddRiftClosedScore(SurfaceType.Wall);

            // Act
            scoreSystem.ResetScore();

            // Assert
            Assert.AreEqual(0, scoreSystem.CurrentScore, "Score should reset to 0");
            Assert.AreEqual(0, scoreSystem.TotalFragmentsKilled, "Fragments killed should reset to 0");
            Assert.AreEqual(0, scoreSystem.TotalRiftsClosed, "Rifts closed should reset to 0");
        }

        [Test]
        public void GetScoreBreakdown_ReturnsCorrectData()
        {
            // Arrange
            scoreSystem.AddFragmentKillScore(FragmentType.Common);
            scoreSystem.AddFragmentKillScore(FragmentType.Elite);
            scoreSystem.AddRiftClosedScore(SurfaceType.Wall);

            // Act
            ScoreBreakdown breakdown = scoreSystem.GetScoreBreakdown();

            // Assert
            Assert.AreEqual(350, breakdown.TotalScore, "Total score should be 350");
            Assert.AreEqual(2, breakdown.FragmentsKilled, "Fragments killed should be 2");
            Assert.AreEqual(1, breakdown.RiftsClosed, "Rifts closed should be 1");
        }

        [Test]
        public void MultipleKills_AccumulateScore()
        {
            // Act
            scoreSystem.AddFragmentKillScore(FragmentType.Common);
            scoreSystem.AddFragmentKillScore(FragmentType.Common);
            scoreSystem.AddFragmentKillScore(FragmentType.Elite);

            // Assert
            Assert.AreEqual(450, scoreSystem.CurrentScore, "Score should accumulate correctly");
            Assert.AreEqual(3, scoreSystem.TotalFragmentsKilled, "Fragments killed should be 3");
        }
    }
}
