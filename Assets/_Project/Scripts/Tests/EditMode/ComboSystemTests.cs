using UnityEngine;
using NUnit.Framework;
using BreachAR.Gameplay;
using BreachAR.Core;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Unit tests for ComboSystem
    /// </summary>
    [TestFixture]
    public class ComboSystemTests
    {
        private ComboSystem comboSystem;
        private GameObject testObject;

        [SetUp]
        public void SetUp()
        {
            testObject = new GameObject();
            comboSystem = testObject.AddComponent<ComboSystem>();
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
            Assert.AreEqual(1f, comboSystem.CurrentMultiplier, "Initial multiplier should be 1.0");
            Assert.AreEqual(0, comboSystem.ComboCount, "Initial combo count should be 0");
            Assert.IsFalse(comboSystem.IsActive, "Should not be active initially");
        }

        [Test]
        public void Activate_SetsActiveTrue()
        {
            // Act
            comboSystem.Activate();

            // Assert
            Assert.IsTrue(comboSystem.IsActive, "Should be active after activation");
        }

        [Test]
        public void Deactivate_SetsActiveFalse()
        {
            // Arrange
            comboSystem.Activate();

            // Act
            comboSystem.Deactivate();

            // Assert
            Assert.IsFalse(comboSystem.IsActive, "Should not be active after deactivation");
        }

        [Test]
        public void RegisterHit_IncrementsMultiplier()
        {
            // Arrange
            comboSystem.Activate();

            // Act
            comboSystem.RegisterHit();

            // Assert
            Assert.AreEqual(1.1f, comboSystem.CurrentMultiplier, 0.001f, "Multiplier should increment by 0.1");
            Assert.AreEqual(1, comboSystem.ComboCount, "Combo count should be 1");
        }

        [Test]
        public void RegisterHit_MultipleHits_IncrementsCorrectly()
        {
            // Arrange
            comboSystem.Activate();

            // Act
            comboSystem.RegisterHit();
            comboSystem.RegisterHit();
            comboSystem.RegisterHit();

            // Assert
            Assert.AreEqual(1.3f, comboSystem.CurrentMultiplier, 0.001f, "Multiplier should be 1.3 after 3 hits");
            Assert.AreEqual(3, comboSystem.ComboCount, "Combo count should be 3");
        }

        [Test]
        public void RegisterHit_DoesNotExceedMaxMultiplier()
        {
            // Arrange
            comboSystem.Activate();

            // Act - Register many hits to exceed max
            for (int i = 0; i < 60; i++)
            {
                comboSystem.RegisterHit();
            }

            // Assert
            Assert.AreEqual(5f, comboSystem.CurrentMultiplier, 0.001f, "Multiplier should not exceed max of 5.0");
        }

        [Test]
        public void RegisterHit_DoesNothingWhenInactive()
        {
            // Arrange - Not activated

            // Act
            comboSystem.RegisterHit();

            // Assert
            Assert.AreEqual(1f, comboSystem.CurrentMultiplier, 0.001f, "Multiplier should remain 1.0 when inactive");
        }

        [Test]
        public void ResetCombo_ResetsMultiplierToInitial()
        {
            // Arrange
            comboSystem.Activate();
            comboSystem.RegisterHit();
            comboSystem.RegisterHit();

            // Act
            comboSystem.ResetCombo();

            // Assert
            Assert.AreEqual(1f, comboSystem.CurrentMultiplier, 0.001f, "Multiplier should reset to 1.0");
            Assert.AreEqual(0, comboSystem.ComboCount, "Combo count should reset to 0");
        }

        [Test]
        public void IsComboActive_ReturnsTrueWhenActiveAndWithinWindow()
        {
            // Arrange
            comboSystem.Activate();
            comboSystem.RegisterHit();

            // Act
            bool isActive = comboSystem.IsComboActive();

            // Assert
            Assert.IsTrue(isActive, "Combo should be active immediately after hit");
        }

        [Test]
        public void IsComboActive_ReturnsFalseWhenInactive()
        {
            // Arrange
            comboSystem.Activate();
            comboSystem.RegisterHit();
            comboSystem.Deactivate();

            // Act
            bool isActive = comboSystem.IsComboActive();

            // Assert
            Assert.IsFalse(isActive, "Combo should not be active when system is deactivated");
        }

        [Test]
        public void GetComboWindowProgress_ReturnsCorrectValue()
        {
            // Arrange
            comboSystem.Activate();
            comboSystem.RegisterHit();

            // Act - Immediately after hit, progress should be near 0
            float progress = comboSystem.GetComboWindowProgress();

            // Assert
            Assert.IsTrue(progress >= 0f && progress <= 0.1f, "Progress should be near 0 immediately after hit");
        }
    }
}
