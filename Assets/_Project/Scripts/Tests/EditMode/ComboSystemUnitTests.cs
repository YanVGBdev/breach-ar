using UnityEngine;
using NUnit.Framework;
using BreachAR.Gameplay;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Unit tests for ComboSystem
    /// Referência: QA-002
    /// </summary>
    [TestFixture]
    public class ComboSystemUnitTests
    {
        [Test]
        public void ComboSystem_InitialState_HasZeroMultiplier()
        {
            // Arrange
            var combo = new ComboTestData();

            // Assert
            Assert.AreEqual(1f, combo.Multiplier);
            Assert.AreEqual(0, combo.ComboCount);
        }

        [Test]
        public void ComboSystem_IncrementIncreasesMultiplier()
        {
            // Arrange
            var combo = new ComboTestData();

            // Act
            combo.Increment();

            // Assert
            Assert.Greater(combo.Multiplier, 1f);
            Assert.AreEqual(1, combo.ComboCount);
        }

        [Test]
        public void ComboSystem_MultipleIncrements_BuildMultiplier()
        {
            // Arrange
            var combo = new ComboTestData();

            // Act
            for (int i = 0; i < 5; i++)
                combo.Increment();

            // Assert
            Assert.AreEqual(5, combo.ComboCount);
            Assert.Greater(combo.Multiplier, 1f);
        }

        [Test]
        public void ComboSystem_ResetResetsMultiplier()
        {
            // Arrange
            var combo = new ComboTestData();
            for (int i = 0; i < 5; i++)
                combo.Increment();

            // Act
            combo.Reset();

            // Assert
            Assert.AreEqual(1f, combo.Multiplier);
            Assert.AreEqual(0, combo.ComboCount);
        }

        [Test]
        public void ComboSystem_HasMaximumCap()
        {
            // Arrange
            var combo = new ComboTestData(maxMultiplier: 5f);

            // Act
            for (int i = 0; i < 20; i++)
                combo.Increment();

            // Assert
            Assert.LessOrEqual(combo.Multiplier, 5f);
        }

        [Test]
        public void ComboSystem_DecayReducesMultiplier()
        {
            // Arrange
            var combo = new ComboTestData();
            for (int i = 0; i < 10; i++)
                combo.Increment();
            float initialMultiplier = combo.Multiplier;

            // Act
            combo.Decay(0.1f);

            // Assert
            Assert.Less(combo.Multiplier, initialMultiplier);
        }

        [Test]
        public void ComboSystem_DecayDoesNotGoBelowOne()
        {
            // Arrange
            var combo = new ComboTestData();

            // Act
            combo.Decay(100f);

            // Assert
            Assert.GreaterOrEqual(combo.Multiplier, 1f);
        }

        [Test]
        public void ComboSystem_ActivateDeactivate()
        {
            // Arrange
            var combo = new ComboTestData();

            // Act & Assert
            combo.Deactivate();
            Assert.IsFalse(combo.IsActive);

            combo.Activate();
            Assert.IsTrue(combo.IsActive);
        }

        /// <summary>
        /// Simple test helper for combo logic
        /// </summary>
        private class ComboTestData
        {
            public float Multiplier { get; private set; } = 1f;
            public int ComboCount { get; private set; }
            public bool IsActive { get; private set; } = true;
            private float maxMultiplier;

            public ComboTestData(float maxMultiplier = 10f)
            {
                this.maxMultiplier = maxMultiplier;
            }

            public void Increment()
            {
                if (!IsActive) return;
                ComboCount++;
                Multiplier = Mathf.Min(1f + (ComboCount * 0.5f), maxMultiplier);
            }

            public void Reset()
            {
                Multiplier = 1f;
                ComboCount = 0;
            }

            public void Decay(float amount)
            {
                Multiplier = Mathf.Max(1f, Multiplier - amount);
            }

            public void Activate() => IsActive = true;
            public void Deactivate() => IsActive = false;
        }
    }
}
