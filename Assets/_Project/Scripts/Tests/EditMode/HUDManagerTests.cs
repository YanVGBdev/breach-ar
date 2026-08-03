using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.UI;
using BreachAR.Core;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for HUDManager
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class HUDManagerTests
    {
        [Test]
        public void HUDManager_InitialState_IsActive()
        {
            var gameObject = new GameObject();
            var hudManager = gameObject.AddComponent<HUDManager>();

            Assert.IsTrue(hudManager.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void HUDManager_SetHUDVisible_True_ActivatesGameObject()
        {
            var gameObject = new GameObject();
            var hudManager = gameObject.AddComponent<HUDManager>();

            hudManager.SetHUDVisible(true);

            Assert.IsTrue(hudManager.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void HUDManager_SetHUDVisible_False_DeactivatesGameObject()
        {
            var gameObject = new GameObject();
            var hudManager = gameObject.AddComponent<HUDManager>();

            hudManager.SetHUDVisible(false);

            Assert.IsFalse(hudManager.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void ThreatType_HasAllExpectedValues()
        {
            Assert.AreEqual(3, System.Enum.GetValues(typeof(ThreatType)).Length);
        }

        [Test]
        public void ThreatType_Fragment_Exists()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ThreatType), ThreatType.Fragment));
        }

        [Test]
        public void ThreatType_Rift_Exists()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ThreatType), ThreatType.Rift));
        }

        [Test]
        public void ThreatType_Boss_Exists()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(ThreatType), ThreatType.Boss));
        }
    }
}
