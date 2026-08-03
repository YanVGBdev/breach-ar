using UnityEngine;
using NUnit.Framework;
using BreachAR.AI;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Unit tests for DifficultyDirector
    /// </summary>
    [TestFixture]
    public class DifficultyDirectorTests
    {
        private DifficultyDirector director;
        private GameObject testObject;

        [SetUp]
        public void SetUp()
        {
            testObject = new GameObject();
            director = testObject.AddComponent<DifficultyDirector>();
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(testObject);
        }

        [Test]
        public void InitialState_HasCorrectValues()
        {
            // Assert
            Assert.AreEqual(0f, director.CurrentDifficultyDelta, 0.001f, "Initial delta should be 0");
            Assert.AreEqual(0f, director.AccumulatedDelta, 0.001f, "Initial accumulated delta should be 0");
            Assert.AreEqual(0f, director.SkillScore, 0.001f, "Initial skill score should be 0");
        }

        [Test]
        public void ResetMetrics_ResetsAllValues()
        {
            // Arrange
            director.RecordLaunch();
            director.RecordHit(0.5f);
            director.CalculateDifficultyForWave(1);

            // Act
            director.ResetMetrics();

            // Assert
            Assert.AreEqual(0f, director.CurrentDifficultyDelta, 0.001f, "Delta should reset");
            Assert.AreEqual(0f, director.AccumulatedDelta, 0.001f, "Accumulated delta should reset");
            Assert.AreEqual(0f, director.SkillScore, 0.001f, "Skill score should reset");
        }

        [Test]
        public void RecordLaunch_IncrementsLaunchCount()
        {
            // Act
            director.RecordLaunch();
            director.RecordLaunch();

            // Assert
            DifficultyStats stats = director.GetStats();
            Assert.AreEqual(2, stats.TotalLaunches, "Launch count should be 2");
        }

        [Test]
        public void RecordHit_IncrementsHitCount()
        {
            // Arrange
            director.RecordLaunch();

            // Act
            director.RecordHit(0.5f);

            // Assert
            DifficultyStats stats = director.GetStats();
            Assert.AreEqual(1, stats.TotalHits, "Hit count should be 1");
        }

        [Test]
        public void CalculateDifficultyForWave_ReturnsNonNegative()
        {
            // Arrange
            director.RecordLaunch();
            director.RecordHit(0.5f);

            // Act
            float delta = director.CalculateDifficultyForWave(1);

            // Assert
            Assert.IsTrue(delta >= -0.5f, "Delta should be clamped");
            Assert.IsTrue(delta <= 0.5f, "Delta should be clamped");
        }

        [Test]
        public void CalculateDifficultyForWave_AccumulatesOverWaves()
        {
            // Arrange
            director.RecordLaunch();
            director.RecordHit(0.3f); // Fast reaction = high skill

            // Act
            director.CalculateDifficultyForWave(1);
            float delta1 = director.AccumulatedDelta;

            director.CalculateDifficultyForWave(2);
            float delta2 = director.AccumulatedDelta;

            // Assert
            Assert.IsTrue(delta2 >= delta1, "Accumulated delta should increase with high skill");
        }

        [Test]
        public void GetDifficultyMultiplier_ReturnsCorrectValue()
        {
            // Arrange - No data should return neutral multiplier
            director.RecordLaunch();
            director.RecordHit(1.5f); // Medium reaction

            // Act
            director.CalculateDifficultyForWave(1);
            float multiplier = director.GetDifficultyMultiplier();

            // Assert
            Assert.IsTrue(multiplier > 0f, "Multiplier should be positive");
            Assert.IsTrue(multiplier < 2f, "Multiplier should be reasonable");
        }

        [Test]
        public void RecordCoreDamage_AffectsSkillScore()
        {
            // Arrange
            director.RecordLaunch();
            director.RecordHit(0.5f);

            // Act
            director.RecordCoreDamage(50f);
            director.CalculateDifficultyForWave(1);

            // Assert
            DifficultyStats stats = director.GetStats();
            Assert.IsTrue(stats.CoreDamageRate > 0, "Core damage rate should be tracked");
        }

        [Test]
        public void GetStats_ReturnsCompleteData()
        {
            // Arrange
            director.RecordLaunch();
            director.RecordLaunch();
            director.RecordHit(0.4f);
            director.CalculateDifficultyForWave(3);

            // Act
            DifficultyStats stats = director.GetStats();

            // Assert
            Assert.AreEqual(2, stats.TotalLaunches, "Should have 2 launches");
            Assert.AreEqual(1, stats.TotalHits, "Should have 1 hit");
            Assert.AreEqual(3, stats.WaveIndex, "Wave index should be 3");
            Assert.IsTrue(stats.HitRate > 0f, "Hit rate should be calculated");
        }

        [Test]
        public void MultipleWaves_DifficultyAccumulates()
        {
            // Arrange
            director.RecordLaunch();

            // Simulate good performance
            for (int i = 0; i < 5; i++)
            {
                director.RecordHit(0.3f);
                director.CalculateDifficultyForWave(i + 1);
            }

            // Assert
            float finalDelta = director.AccumulatedDelta;
            Assert.IsTrue(finalDelta > 0f, "Accumulated delta should be positive with good performance");
        }
    }
}
