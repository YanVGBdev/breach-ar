using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.UI;
using BreachAR.Core;
using BreachAR.Gameplay;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for GameOverUI
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class GameOverUITests
    {
        [Test]
        public void GameOverUI_InitialState_IsHidden()
        {
            // Arrange
            var gameObject = new GameObject();
            var gameOverUI = gameObject.AddComponent<GameOverUI>();

            // Act & Assert
            Assert.IsFalse(gameOverUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void GameOverUI_Show_ActivatesGameObject()
        {
            // Arrange
            var gameObject = new GameObject();
            var gameOverUI = gameObject.AddComponent<GameOverUI>();
            var breakdown = new ScoreBreakdown
            {
                TotalScore = 5000,
                WavesCleared = 10,
                MaxCombo = 3.5f,
                FragmentsKilled = 150,
                RiftsClosed = 8
            };

            // Act
            gameOverUI.Show(breakdown, false, 250);

            // Assert
            Assert.IsTrue(gameOverUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void GameOverUI_Hide_DeactivatesGameObject()
        {
            // Arrange
            var gameObject = new GameObject();
            var gameOverUI = gameObject.AddComponent<GameOverUI>();
            gameObject.SetActive(true);

            // Act
            gameOverUI.Hide();

            // Assert
            Assert.IsFalse(gameOverUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ScoreBreakdown_ContainsRequiredFields()
        {
            // Arrange & Act
            var breakdown = new ScoreBreakdown
            {
                TotalScore = 10000,
                WavesCleared = 20,
                MaxCombo = 5.0f,
                FragmentsKilled = 300,
                RiftsClosed = 15
            };

            // Assert
            Assert.AreEqual(10000, breakdown.TotalScore);
            Assert.AreEqual(20, breakdown.WavesCleared);
            Assert.AreEqual(5.0f, breakdown.MaxCombo);
            Assert.AreEqual(300, breakdown.FragmentsKilled);
            Assert.AreEqual(15, breakdown.RiftsClosed);
        }

        [Test]
        public void GameOverData_ContainsRequiredFields()
        {
            // Arrange & Act
            var data = new GameOverData
            {
                Victory = true,
                FinalScore = 8000,
                WavesCleared = 15,
                MaxCombo = 4.2f,
                FragmentsKilled = 200,
                RiftsClosed = 10
            };

            // Assert
            Assert.IsTrue(data.Victory);
            Assert.AreEqual(8000, data.FinalScore);
            Assert.AreEqual(15, data.WavesCleared);
            Assert.AreEqual(4.2f, data.MaxCombo);
            Assert.AreEqual(200, data.FragmentsKilled);
            Assert.AreEqual(10, data.RiftsClosed);
        }

        [Test]
        public void GameEvents_GameOver_CanBeAssigned()
        {
            // Arrange
            bool gameOverCalled = false;
            bool lastVictory = false;

            // Act
            GameEvents.OnGameOver += (data) =>
            {
                gameOverCalled = true;
                lastVictory = data.Victory;
            };

            // Assert
            GameEvents.OnGameOver?.Invoke(new GameOverData { Victory = true });
            
            Assert.IsTrue(gameOverCalled);
            Assert.IsTrue(lastVictory);

            // Cleanup
            GameEvents.ClearAll();
        }
    }
}
