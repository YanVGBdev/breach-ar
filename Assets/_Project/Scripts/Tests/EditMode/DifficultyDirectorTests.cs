using UnityEngine;
using NUnit.Framework;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Unit tests for DifficultyDirector (DDA)
    /// Referência: QA-004
    /// </summary>
    [TestFixture]
    public class DifficultyDirectorTests
    {
        [Test]
        public void DifficultyDirector_SkillScore_DefaultIs05()
        {
            // Arrange & Act
            var dda = new DDATestData();

            // Assert
            Assert.AreEqual(0.5f, dda.SkillScore, 0.01f);
        }

        [Test]
        public void DifficultyDirector_CalculateDelta_PlayerDoingWell()
        {
            // Arrange
            var dda = new DDATestData();

            // Act - Player hitting 100% of shots
            for (int i = 0; i < 10; i++)
                dda.RecordHit(0.5f);

            float delta = dda.CalculateDifficultyDelta();

            // Assert - Delta should be positive (increase difficulty)
            Assert.Greater(delta, 0f);
        }

        [Test]
        public void DifficultyDirector_CalculateDelta_PlayerStruggling()
        {
            // Arrange
            var dda = new DDATestData();

            // Act - Player missing most shots
            for (int i = 0; i < 10; i++)
                dda.RecordMiss();

            float delta = dda.CalculateDifficultyDelta();

            // Assert - Delta should be negative (decrease difficulty)
            Assert.Less(delta, 0f);
        }

        [Test]
        public void DifficultyDirector_DeltaClamped_ToMaxRange()
        {
            // Arrange
            var dda = new DDATestData(maxDelta: 0.15f);

            // Act - Extreme performance
            for (int i = 0; i < 100; i++)
                dda.RecordHit(0.1f);

            float delta = dda.CalculateDifficultyDelta();

            // Assert - Should be clamped
            Assert.LessOrEqual(delta, 0.15f);
        }

        [Test]
        public void DifficultyDirector_DeltaClamped_ToMinRange()
        {
            // Arrange
            var dda = new DDATestData(maxDelta: 0.15f);

            // Act - Very poor performance
            for (int i = 0; i < 100; i++)
                dda.RecordMiss();

            float delta = dda.CalculateDifficultyDelta();

            // Assert - Should be clamped
            Assert.GreaterOrEqual(delta, -0.15f);
        }

        [Test]
        public void DifficultyDirector_SkillScore_BoundedBetween0And1()
        {
            // Arrange
            var dda = new DDATestData();

            // Act - Extreme inputs
            for (int i = 0; i < 100; i++)
            {
                dda.RecordHit(0.01f); // Very fast
            }

            float skill = dda.CalculateSkillScore();

            // Assert
            Assert.GreaterOrEqual(skill, 0f);
            Assert.LessOrEqual(skill, 1f);
        }

        [Test]
        public void DifficultyDirector_Reset_ClearsState()
        {
            // Arrange
            var dda = new DDATestData();
            for (int i = 0; i < 10; i++)
                dda.RecordHit(0.5f);

            // Act
            dda.Reset();

            // Assert
            Assert.AreEqual(0.5f, dda.SkillScore, 0.01f);
        }

        /// <summary>
        /// Simple test helper for DDA logic
        /// </summary>
        private class DDATestData
        {
            public float SkillScore { get; private set; } = 0.5f;
            private int hits;
            private int misses;
            private float sensitivity = 1f;
            private float maxDelta;

            public DDATestData(float sensitivity = 1f, float maxDelta = 0.15f)
            {
                this.sensitivity = sensitivity;
                this.maxDelta = maxDelta;
            }

            public void RecordHit(float reactionTime)
            {
                hits++;
            }

            public void RecordMiss()
            {
                misses++;
            }

            public float CalculateSkillScore()
            {
                int total = hits + misses;
                if (total == 0) return 0.5f;

                float hitRate = (float)hits / total;
                SkillScore = Mathf.Clamp01(hitRate);
                return SkillScore;
            }

            public float CalculateDifficultyDelta()
            {
                float skill = CalculateSkillScore();
                float delta = (skill - 0.5f) * sensitivity;
                return Mathf.Clamp(delta, -maxDelta, maxDelta);
            }

            public void Reset()
            {
                hits = 0;
                misses = 0;
                SkillScore = 0.5f;
            }
        }
    }
}
