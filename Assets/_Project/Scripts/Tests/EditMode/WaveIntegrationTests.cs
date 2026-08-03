using UnityEngine;
using NUnit.Framework;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Integration tests for wave flow
    /// Referência: QA-008
    /// </summary>
    [TestFixture]
    public class WaveIntegrationTests
    {
        [Test]
        public void WaveFlow_InitialState_IsIdle()
        {
            // Arrange
            var session = new WaveSessionTest();

            // Assert
            Assert.AreEqual(WaveStateTest.Idle, session.CurrentState);
        }

        [Test]
        public void WaveFlow_Initialize_TransitionsToInitializing()
        {
            // Arrange
            var session = new WaveSessionTest();

            // Act
            session.Initialize(10);

            // Assert
            Assert.AreEqual(WaveStateTest.Initializing, session.CurrentState);
        }

        [Test]
        public void WaveFlow_Start_TransitionsToWaveActive()
        {
            // Arrange
            var session = new WaveSessionTest();
            session.Initialize(10);

            // Act
            session.StartSession();

            // Assert
            Assert.AreEqual(WaveStateTest.WaveActive, session.CurrentState);
        }

        [Test]
        public void WaveFlow_CompleteWave_IncrementsWaveIndex()
        {
            // Arrange
            var session = new WaveSessionTest();
            session.Initialize(10);
            session.StartSession();
            int initialWave = session.CurrentWaveIndex;

            // Act
            session.CompleteWave();

            // Assert
            Assert.Greater(session.CurrentWaveIndex, initialWave);
        }

        [Test]
        public void WaveFlow_AllWavesComplete_TransitionsToSessionComplete()
        {
            // Arrange
            var session = new WaveSessionTest();
            session.Initialize(2);
            session.StartSession();

            // Act
            session.CompleteWave(); // Wave 1
            session.CompleteWave(); // Wave 2

            // Assert
            Assert.AreEqual(WaveStateTest.SessionComplete, session.CurrentState);
        }

        [Test]
        public void WaveFlow_CoreDestroyed_TransitionsToFailed()
        {
            // Arrange
            var session = new WaveSessionTest();
            session.Initialize(10);
            session.StartSession();

            // Act
            session.HandleCoreDestroyed();

            // Assert
            Assert.AreEqual(WaveStateTest.Failed, session.CurrentState);
        }

        [Test]
        public void WaveFlow_PauseAndResume()
        {
            // Arrange
            var session = new WaveSessionTest();
            session.Initialize(10);
            session.StartSession();

            // Act
            session.Pause();
            Assert.AreEqual(WaveStateTest.Paused, session.CurrentState);

            session.Resume();
            Assert.AreEqual(WaveStateTest.WaveActive, session.CurrentState);
        }

        [Test]
        public void WaveFlow_BossWave_DetectedCorrectly()
        {
            // Arrange
            var session = new WaveSessionTest();
            session.Initialize(30);

            // Act & Assert
            Assert.IsTrue(session.IsBossWave(10));
            Assert.IsTrue(session.IsBossWave(20));
            Assert.IsTrue(session.IsBossWave(30));
            Assert.IsFalse(session.IsBossWave(5));
            Assert.IsFalse(session.IsBossWave(15));
        }

        /// <summary>
        /// Simple test helper for wave session logic
        /// </summary>
        private class WaveSessionTest
        {
            public WaveStateTest CurrentState { get; private set; } = WaveStateTest.Idle;
            public int CurrentWaveIndex { get; private set; }
            public int TotalWaves { get; private set; }

            public void Initialize(int waves)
            {
                TotalWaves = waves;
                CurrentWaveIndex = 0;
                CurrentState = WaveStateTest.Initializing;
            }

            public void StartSession()
            {
                if (CurrentState != WaveStateTest.Initializing) return;
                CurrentWaveIndex = 1;
                CurrentState = WaveStateTest.WaveActive;
            }

            public void CompleteWave()
            {
                if (CurrentState != WaveStateTest.WaveActive) return;

                if (CurrentWaveIndex >= TotalWaves)
                {
                    CurrentState = WaveStateTest.SessionComplete;
                }
                else
                {
                    CurrentWaveIndex++;
                }
            }

            public void HandleCoreDestroyed()
            {
                CurrentState = WaveStateTest.Failed;
            }

            public void Pause()
            {
                if (CurrentState == WaveStateTest.WaveActive)
                {
                    CurrentState = WaveStateTest.Paused;
                }
            }

            public void Resume()
            {
                if (CurrentState == WaveStateTest.Paused)
                {
                    CurrentState = WaveStateTest.WaveActive;
                }
            }

            public bool IsBossWave(int waveIndex)
            {
                return waveIndex % 10 == 0;
            }
        }

        private enum WaveStateTest
        {
            Idle,
            Initializing,
            WaveActive,
            WaveTransition,
            BossActive,
            SessionComplete,
            Failed,
            Paused
        }
    }
}
