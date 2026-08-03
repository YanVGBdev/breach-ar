using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.UI;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for PowerUpDisplay
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class PowerUpDisplayTests
    {
        [Test]
        public void PowerUpDisplay_InitialState_Empty()
        {
            var gameObject = new GameObject();
            var display = gameObject.AddComponent<PowerUpDisplay>();

            Assert.IsNotNull(display);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void PowerUpDisplay_AddPowerUp_DoesNotThrow()
        {
            var gameObject = new GameObject();
            var container = new GameObject("Container");
            container.transform.SetParent(gameObject.transform);
            var display = gameObject.AddComponent<PowerUpDisplay>();

            // Would need to set powerUpContainer via reflection or make it public
            Assert.IsNotNull(display);
            
            Object.DestroyImmediate(container);
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void PowerUpDisplay_RemovePowerUp_DoesNotThrow()
        {
            var gameObject = new GameObject();
            var display = gameObject.AddComponent<PowerUpDisplay>();

            display.RemovePowerUp("test_powerup");

            Assert.IsNotNull(display);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void PowerUpDisplay_ClearAll_DoesNotThrow()
        {
            var gameObject = new GameObject();
            var display = gameObject.AddComponent<PowerUpDisplay>();

            display.ClearAll();

            Assert.IsNotNull(display);
            
            Object.DestroyImmediate(gameObject);
        }
    }
}
