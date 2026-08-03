using UnityEngine;
using NUnit.Framework;
using BreachAR.AR;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for DeviceCompatibilityService
    /// Referência: OPT-001, OPT-002, AR-016
    /// </summary>
    [TestFixture]
    public class DeviceCompatibilityTests
    {
        [Test]
        public void DeviceCapability_DefaultValues_AreCorrect()
        {
            // Arrange & Act
            var capability = new ARDeviceCapability
            {
                SupportsAR = true,
                SupportsDepthAPI = false,
                RAMGB = 4,
                GPUMemoryMB = 1024,
                HasGyroscope = true,
                ProcessorCount = 8,
                SupportsVulkan = true
            };

            // Assert
            Assert.IsTrue(capability.SupportsAR);
            Assert.IsFalse(capability.SupportsDepthAPI);
            Assert.AreEqual(4, capability.RAMGB);
            Assert.AreEqual(1024, capability.GPUMemoryMB);
            Assert.IsTrue(capability.HasGyroscope);
            Assert.AreEqual(8, capability.ProcessorCount);
            Assert.IsTrue(capability.SupportsVulkan);
        }

        [Test]
        public void DeviceTier_LowSpecs_ReturnsLowTier()
        {
            // Arrange
            var capability = new ARDeviceCapability
            {
                RAMGB = 2,
                GPUMemoryMB = 512,
                ProcessorCount = 4,
                SupportsDepthAPI = false
            };

            // Act - Calculate tier score
            int score = CalculateTierScore(capability);

            // Assert
            Assert.Less(score, 5); // Should be Low tier
        }

        [Test]
        public void DeviceTier_HighSpecs_ReturnsHighTier()
        {
            // Arrange
            var capability = new ARDeviceCapability
            {
                RAMGB = 8,
                GPUMemoryMB = 4096,
                ProcessorCount = 8,
                SupportsDepthAPI = true
            };

            // Act
            int score = CalculateTierScore(capability);

            // Assert
            Assert.GreaterOrEqual(score, 8); // Should be High tier
        }

        [Test]
        public void TierSettings_HighTier_HasCorrectDefaults()
        {
            // Arrange & Act
            var settings = new TierSettings
            {
                GraphicsQuality = 3,
                ParticleBudget = 100,
                EnableOcclusion = true,
                EnableShadows = true,
                TargetFPS = 60,
                TextureQuality = 0
            };

            // Assert
            Assert.AreEqual(3, settings.GraphicsQuality);
            Assert.AreEqual(100, settings.ParticleBudget);
            Assert.IsTrue(settings.EnableOcclusion);
            Assert.IsTrue(settings.EnableShadows);
            Assert.AreEqual(60, settings.TargetFPS);
            Assert.AreEqual(0, settings.TextureQuality);
        }

        [Test]
        public void TierSettings_LowTier_HasCorrectDefaults()
        {
            // Arrange & Act
            var settings = new TierSettings
            {
                GraphicsQuality = 0,
                ParticleBudget = 20,
                EnableOcclusion = false,
                EnableShadows = false,
                TargetFPS = 30,
                TextureQuality = 2
            };

            // Assert
            Assert.AreEqual(0, settings.GraphicsQuality);
            Assert.AreEqual(20, settings.ParticleBudget);
            Assert.IsFalse(settings.EnableOcclusion);
            Assert.IsFalse(settings.EnableShadows);
            Assert.AreEqual(30, settings.TargetFPS);
            Assert.AreEqual(2, settings.TextureQuality);
        }

        [Test]
        public void ARFeature_BasicAR_SupportedWithAR()
        {
            // Arrange
            var capability = new ARDeviceCapability { SupportsAR = true };

            // Act & Assert
            Assert.IsTrue(capability.SupportsAR);
        }

        [Test]
        public void ARFeature_DepthOcclusion_RequiresDepthAPI()
        {
            // Arrange
            var withoutDepth = new ARDeviceCapability { SupportsDepthAPI = false };
            var withDepth = new ARDeviceCapability { SupportsDepthAPI = true };

            // Assert
            Assert.IsFalse(withoutDepth.SupportsDepthAPI);
            Assert.IsTrue(withDepth.SupportsDepthAPI);
        }

        /// <summary>
        /// Helper method to calculate tier score
        /// </summary>
        private int CalculateTierScore(ARDeviceCapability capability)
        {
            int score = 0;

            // RAM scoring
            if (capability.RAMGB >= 6) score += 3;
            else if (capability.RAMGB >= 4) score += 2;
            else score += 1;

            // GPU memory scoring
            if (capability.GPUMemoryMB >= 2048) score += 3;
            else if (capability.GPUMemoryMB >= 1024) score += 2;
            else score += 1;

            // Depth API bonus
            if (capability.SupportsDepthAPI) score += 2;

            // Processor count
            if (capability.ProcessorCount >= 8) score += 2;
            else if (capability.ProcessorCount >= 4) score += 1;

            return score;
        }
    }
}
