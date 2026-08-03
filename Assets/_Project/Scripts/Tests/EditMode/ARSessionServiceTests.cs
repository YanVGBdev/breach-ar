using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.AR;
using BreachAR.Core;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for ARSessionService
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class ARSessionServiceTests
    {
        [Test]
        public void ARSessionService_InitialState_NotActive()
        {
            var gameObject = new GameObject();
            var arService = gameObject.AddComponent<ARSessionService>();

            Assert.IsFalse(arService.IsSessionActive);
            Assert.IsFalse(arService.IsScanComplete);
            Assert.AreEqual(0f, arService.ScanProgress);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ARSessionService_StartSession_SetsActive()
        {
            var gameObject = new GameObject();
            var arService = gameObject.AddComponent<ARSessionService>();

            arService.StartSession();

            Assert.IsTrue(arService.IsSessionActive);
            Assert.IsFalse(arService.IsScanComplete);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ARSessionService_StopSession_SetsInactive()
        {
            var gameObject = new GameObject();
            var arService = gameObject.AddComponent<ARSessionService>();
            arService.StartSession();

            arService.StopSession();

            Assert.IsFalse(arService.IsSessionActive);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ARSessionService_Rescan_ResetsState()
        {
            var gameObject = new GameObject();
            var arService = gameObject.AddComponent<ARSessionService>();
            arService.StartSession();

            arService.Rescan();

            Assert.IsFalse(arService.IsScanComplete);
            Assert.AreEqual(0f, arService.ScanProgress);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ARSessionService_CheckDeviceCapability_ReturnsValid()
        {
            var gameObject = new GameObject();
            var arService = gameObject.AddComponent<ARSessionService>();

            var capability = arService.CheckDeviceCapability();

            Assert.IsTrue(capability.SupportsAR);
            Assert.Greater(capability.RAMGB, 0);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ARSessionService_ClassifySurface_FloorDetection()
        {
            var gameObject = new GameObject();
            var arService = gameObject.AddComponent<ARSessionService>();

            // Test floor detection (normal pointing up, low height)
            SurfaceType result = arService.ClassifySurface(Vector3.up, 0f, 1.5f);

            Assert.AreEqual(SurfaceType.Floor, result);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ARSessionService_ClassifySurface_WallDetection()
        {
            var gameObject = new GameObject();
            var arService = gameObject.AddComponent<ARSessionService>();

            // Test wall detection (normal pointing forward)
            SurfaceType result = arService.ClassifySurface(Vector3.forward, 1f, 1.5f);

            Assert.AreEqual(SurfaceType.Wall, result);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ARSessionService_GetRandomValidSurface_ReturnsNullWhenEmpty()
        {
            var gameObject = new GameObject();
            var arService = gameObject.AddComponent<ARSessionService>();

            var surface = arService.GetRandomValidSurface();

            Assert.IsNull(surface);
            
            Object.DestroyImmediate(gameObject);
        }
    }
}
