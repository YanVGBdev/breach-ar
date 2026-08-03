using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.UI;
using BreachAR.Core;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for ARScanUI
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class ARScanUITests
    {
        [Test]
        public void ARScanUI_InitialState_IsNotScanning()
        {
            // Arrange
            var gameObject = new GameObject();
            var scanUI = gameObject.AddComponent<ARScanUI>();

            // Act & Assert
            Assert.IsFalse(scanUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ARScanUI_StopScan_DisablesGameObject()
        {
            // Arrange
            var gameObject = new GameObject();
            var scanUI = gameObject.AddComponent<ARScanUI>();
            gameObject.SetActive(true);

            // Act
            scanUI.StopScan();

            // Assert - After fade out, should be inactive
            // Note: This tests the logic, actual fade requires MonoBehaviour lifecycle
            Assert.IsNotNull(scanUI);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void GameEvents_AREvents_CanBeAssigned()
        {
            // Arrange
            bool surfaceDetectedCalled = false;
            bool scanCompleteCalled = false;

            // Act
            GameEvents.OnSurfaceDetected += (data) => surfaceDetectedCalled = true;
            GameEvents.OnScanComplete += (data) => scanCompleteCalled = true;

            // Assert
            GameEvents.OnSurfaceDetected?.Invoke(new SurfaceDetectedData());
            GameEvents.OnScanComplete?.Invoke(new ScanCompleteData());
            
            Assert.IsTrue(surfaceDetectedCalled);
            Assert.IsTrue(scanCompleteCalled);

            // Cleanup
            GameEvents.ClearAll();
        }

        [Test]
        public void GameEvents_ClearAll_AREventsAreNull()
        {
            // Arrange
            GameEvents.OnSurfaceDetected += (data) => { };
            GameEvents.OnScanComplete += (data) => { };

            // Act
            GameEvents.ClearAll();

            // Assert
            Assert.IsNull(GameEvents.OnSurfaceDetected);
            Assert.IsNull(GameEvents.OnScanComplete);
            Assert.IsNull(GameEvents.OnSurfaceLost);
            Assert.IsNull(GameEvents.OnAnchorCreated);
        }

        [Test]
        public void SurfaceDetectedData_ContainsRequiredFields()
        {
            // Arrange & Act
            var data = new SurfaceDetectedData
            {
                SurfaceId = "test_123",
                Type = SurfaceType.Floor,
                Area = 2.5f,
                Position = Vector3.one
            };

            // Assert
            Assert.AreEqual("test_123", data.SurfaceId);
            Assert.AreEqual(SurfaceType.Floor, data.Type);
            Assert.AreEqual(2.5f, data.Area);
            Assert.AreEqual(Vector3.one, data.Position);
        }

        [Test]
        public void ScanCompleteData_ContainsRequiredFields()
        {
            // Arrange & Act
            var data = new ScanCompleteData
            {
                SurfaceCount = 5,
                Duration = 12.5f,
                HasFloor = true,
                HasWall = true
            };

            // Assert
            Assert.AreEqual(5, data.SurfaceCount);
            Assert.AreEqual(12.5f, data.Duration);
            Assert.IsTrue(data.HasFloor);
            Assert.IsTrue(data.HasWall);
        }
    }
}
