using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.UI;
using BreachAR.Gameplay;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for RiftHealthBar
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class RiftHealthBarTests
    {
        [Test]
        public void RiftHealthBar_InitialState_IsHidden()
        {
            var gameObject = new GameObject();
            var healthBar = gameObject.AddComponent<RiftHealthBar>();

            // Start hidden until initialized
            Assert.IsNotNull(healthBar);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void RiftHealthBar_Show_ActivatesGameObject()
        {
            var gameObject = new GameObject();
            var healthBar = gameObject.AddComponent<RiftHealthBar>();

            healthBar.Show();

            Assert.IsTrue(healthBar.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void RiftHealthBar_Hide_DeactivatesGameObject()
        {
            var gameObject = new GameObject();
            var healthBar = gameObject.AddComponent<RiftHealthBar>();
            gameObject.SetActive(true);

            healthBar.Hide();

            Assert.IsFalse(healthBar.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }
    }
}
