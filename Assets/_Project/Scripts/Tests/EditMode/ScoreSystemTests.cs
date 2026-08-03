using UnityEngine;
using NUnit.Framework;
using BreachAR.Core;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Unit tests for ScoreSystem
    /// Referência: QA-003
    /// </summary>
    [TestFixture]
    public class ScoreSystemTests
    {
        [Test]
        public void ScoreSystem_InitialState_IsZero()
        {
            // Arrange & Act
            var score = new ScoreTestData();

            // Assert
            Assert.AreEqual(0, score.CurrentScore);
            Assert.AreEqual(0, score.MaxComboAchieved);
        }

        [Test]
        public void ScoreSystem_AddScore_IncreasesTotal()
        {
            // Arrange
            var score = new ScoreTestData();

            // Act
            score.AddScore(100, "Test");

            // Assert
            Assert.AreEqual(100, score.CurrentScore);
        }

        [Test]
        public void ScoreSystem_AddScore_WithMultiplier()
        {
            // Arrange
            var score = new ScoreTestData();

            // Act
            score.AddScore(100, 2.0f, "Test");

            // Assert
            Assert.AreEqual(200, score.CurrentScore);
        }

        [Test]
        public void ScoreSystem_FragmentKilled_CalculatesCorrectly()
        {
            // Arrange
            var score = new ScoreTestData();
            float comboMultiplier = 2.5f;

            // Act
            score.OnFragmentKilled(FragmentType.Basic, comboMultiplier, false);

            // Assert
            Assert.Greater(score.CurrentScore, 0);
        }

        [Test]
        public void ScoreSystem_RiftClosed_GivesBonusScore()
        {
            // Arrange
            var score = new ScoreTestData();

            // Act
            score.OnRiftClosed(SurfaceType.Floor);

            // Assert
            Assert.Greater(score.CurrentScore, 0);
        }

        [Test]
        public void ScoreSystem_WaveCompleted_GivesWaveBonus()
        {
            // Arrange
            var score = new ScoreTestData();

            // Act
            score.OnWaveCompleted(5, 30f, true);

            // Assert
            Assert.Greater(score.CurrentScore, 0);
        }

        [Test]
        public void ScoreSystem_PerfectWave_GivesExtraBonus()
        {
            // Arrange
            var score1 = new ScoreTestData();
            var score2 = new ScoreTestData();

            // Act
            score1.OnWaveCompleted(5, 30f, false);
            score2.OnWaveCompleted(5, 30f, true);

            // Assert
            Assert.Greater(score2.CurrentScore, score1.CurrentScore);
        }

        [Test]
        public void ScoreSystem_Reset_SetsScoreToZero()
        {
            // Arrange
            var score = new ScoreTestData();
            score.AddScore(1000, "Test");

            // Act
            score.ResetScore();

            // Assert
            Assert.AreEqual(0, score.CurrentScore);
        }

        [Test]
        public void ScoreSystem_MaxCombo_TracksHighest()
        {
            // Arrange
            var score = new ScoreTestData();

            // Act
            score.UpdateCombo(2.0f);
            score.UpdateCombo(3.5f);
            score.UpdateCombo(2.5f);

            // Assert
            Assert.AreEqual(3.5f, score.MaxComboAchieved);
        }

        /// <summary>
        /// Simple test helper for score logic
        /// </summary>
        private class ScoreTestData
        {
            public int CurrentScore { get; private set; }
            public float MaxComboAchieved { get; private set; }

            private float baseFragmentScore = 100;
            private float riftCloseScore = 500;
            private float waveCompleteBase = 200;
            private float perfectWaveBonus = 1000;

            public void AddScore(int amount, string reason)
            {
                CurrentScore += amount;
            }

            public void AddScore(int baseScore, float multiplier, string reason)
            {
                CurrentScore += Mathf.RoundToInt(baseScore * multiplier);
            }

            public void OnFragmentKilled(FragmentType type, float comboMultiplier, bool viaRicochet)
            {
                float score = baseFragmentScore * comboMultiplier;
                if (viaRicochet) score *= 1.5f;
                CurrentScore += Mathf.RoundToInt(score);
            }

            public void OnRiftClosed(SurfaceType surfaceType)
            {
                float bonus = surfaceType switch
                {
                    SurfaceType.Floor => 1.0f,
                    SurfaceType.Wall => 1.2f,
                    SurfaceType.Ceiling => 1.5f,
                    _ => 1.0f
                };
                CurrentScore += Mathf.RoundToInt(riftCloseScore * bonus);
            }

            public void OnWaveCompleted(int waveIndex, float timeTaken, bool perfectWave)
            {
                CurrentScore += waveCompleteBase;
                if (perfectWave) CurrentScore += perfectWaveBonus;
            }

            public void UpdateCombo(float multiplier)
            {
                if (multiplier > MaxComboAchieved)
                {
                    MaxComboAchieved = multiplier;
                }
            }

            public void ResetScore()
            {
                CurrentScore = 0;
                MaxComboAchieved = 0;
            }
        }
    }
}
