using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.AR;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for CorePlacementService
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class CorePlacementServiceTests
    {
        [Test]
        public void CorePlacementService_InitialState_NotPlaced()
        {
            var gameObject = new GameObject();
            var service = gameObject.AddComponent<CorePlacementService>();

            Assert.IsFalse(service.IsPlaced);
            Assert.IsNull(service.PlacedCore);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void CorePlacementService_RemoveCore_ClearsState()
        {
            var gameObject = new GameObject();
            var service = gameObject.AddComponent<CorePlacementService>();

            service.RemoveCore();

            Assert.IsFalse(service.IsPlaced);
            Assert.IsNull(service.PlacedCore);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void CorePlacementService_GetPlacementPosition_ReturnsVector()
        {
            var gameObject = new GameObject();
            var service = gameObject.AddComponent<CorePlacementService>();

            Vector3 result = service.GetPlacementPosition(new Vector2(Screen.width / 2, Screen.height / 2));

            Assert.IsInstanceOf<Vector3>(result);
            
            Object.DestroyImmediate(gameObject);
        }
    }
}
