using UnityEngine;
using NUnit.Framework;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Integration tests for Game Over flow
    /// Referência: QA-009
    /// </summary>
    [TestFixture]
    public class GameOverIntegrationTests
    {
        [Test]
        public void GameOver_CoreDestroyed_TriggersGameOver()
        {
            // Arrange
            var core = new CoreControllerTest();
            var session = new GameSessionTest();

            // Act
            core.TakeDamage(core.MaxHealth);
            session.HandleCoreDestroyed();

            // Assert
            Assert.IsTrue(session.IsGameOver);
        }

        [Test]
        public void GameOver_FinalScore_CalculatedCorrectly()
        {
            // Arrange
            var session = new GameSessionTest();
            session.AddScore(5000);

            // Act
            session.HandleCoreDestroyed();

            // Assert
            Assert.AreEqual(5000, session.FinalScore);
        }

        [Test]
        public void GameOver_WavesCleared_TrackedCorrectly()
        {
            // Arrange
            var session = new GameSessionTest();
            session.Initialize(10);
            session.StartSession();
            session.CompleteWave();
            session.CompleteWave();
            session.CompleteWave();

            // Act
            session.HandleCoreDestroyed();

            // Assert
            Assert.AreEqual(3, session.WavesCleared);
        }

        [Test]
        public void GameOver_MaxCombo_Recorded()
        {
            // Arrange
            var session = new GameSessionTest();
            session.UpdateCombo(2.5f);
            session.UpdateCombo(4.0f);
            session.UpdateCombo(3.0f);

            // Act
            session.HandleCoreDestroyed();

            // Assert
            Assert.AreEqual(4.0f, session.MaxCombo);
        }

        [Test]
        public void GameOver_FragmentsKilled_Counted()
        {
            // Arrange
            var session = new GameSessionTest();
            session.RecordFragmentKill();
            session.RecordFragmentKill();
            session.RecordFragmentKill();

            // Act
            session.HandleCoreDestroyed();

            // Assert
            Assert.AreEqual(3, session.FragmentsKilled);
        }

        [Test]
        public void GameOver_RiftsClosed_Counted()
        {
            // Arrange
            var session = new GameSessionTest();
            session.RecordRiftClosed();
            session.RecordRiftClosed();

            // Act
            session.HandleCoreDestroyed();

            // Assert
            Assert.AreEqual(2, session.RiftsClosed);
        }

        [Test]
        public void GameOver_SessionDuration_Tracked()
        {
            // Arrange
            var session = new GameSessionTest();
            session.StartSession();

            // Act
            session.HandleCoreDestroyed();

            // Assert
            Assert.GreaterOrEqual(session.SessionDuration, 0f);
        }

        [Test]
        public void GameOver_VictoryCondition_AllWavesComplete()
        {
            // Arrange
            var session = new GameSessionTest();
            session.Initialize(2);
            session.StartSession();

            // Act
            session.CompleteWave();
            session.CompleteWave();

            // Assert
            Assert.IsTrue(session.IsVictory);
            Assert.IsFalse(session.IsGameOver);
        }

        /// <summary>
        /// Simple test helper for core controller
        /// </summary>
        private class CoreControllerTest
        {
            public float MaxHealth = 100f;
            public float CurrentHealth { get; private set; }

            public CoreControllerTest()
            {
                CurrentHealth = MaxHealth;
            }

            public void TakeDamage(float amount)
            {
                CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
            }

            public bool IsAlive => CurrentHealth > 0;
        }

        /// <summary>
        /// Simple test helper for game session
        /// </summary>
        private class GameSessionTest
        {
            public bool IsGameOver { get; private set; }
            public bool IsVictory { get; private set; }
            public int FinalScore { get; private set; }
            public int WavesCleared { get; private set; }
            public float MaxCombo { get; private set; }
            public int FragmentsKilled { get; private set; }
            public int RiftsClosed { get; private set; }
            public float SessionDuration { get; private set; }

            private int currentScore;
            private int totalWaves;
            private int currentWave;
            private float currentCombo;

            public void Initialize(int waves)
            {
                totalWaves = waves;
                currentWave = 0;
            }

            public void StartSession()
            {
                SessionDuration = 0f;
            }

            public void AddScore(int amount)
            {
                currentScore += amount;
            }

            public void UpdateCombo(float multiplier)
            {
                currentCombo = multiplier;
                if (multiplier > MaxCombo)
                {
                    MaxCombo = multiplier;
                }
            }

            public void RecordFragmentKill()
            {
                FragmentsKilled++;
            }

            public void RecordRiftClosed()
            {
                RiftsClosed++;
            }

            public void CompleteWave()
            {
                currentWave++;
                WavesCleared = currentWave;

                if (currentWave >= totalWaves)
                {
                    IsVictory = true;
                }
            }

            public void HandleCoreDestroyed()
            {
                IsGameOver = true;
                FinalScore = currentScore;
            }
        }
    }
}
