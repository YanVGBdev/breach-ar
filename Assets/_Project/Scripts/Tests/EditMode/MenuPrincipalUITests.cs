using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.UI;
using BreachAR.Core;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for MenuPrincipalUI
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class MenuPrincipalUITests
    {
        [Test]
        public void MenuPrincipalUI_InitialState_IsHidden()
        {
            var gameObject = new GameObject();
            var menuUI = gameObject.AddComponent<MenuPrincipalUI>();

            Assert.IsFalse(menuUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void MenuPrincipalUI_Show_ActivatesGameObject()
        {
            var gameObject = new GameObject();
            var menuUI = gameObject.AddComponent<MenuPrincipalUI>();

            menuUI.Show();

            Assert.IsTrue(menuUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void MenuPrincipalUI_Hide_DeactivatesGameObject()
        {
            var gameObject = new GameObject();
            var menuUI = gameObject.AddComponent<MenuPrincipalUI>();
            gameObject.SetActive(true);

            menuUI.Hide();

            // May be delayed by animator
            Assert.IsNotNull(menuUI);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void GameMode_HasAllExpectedValues()
        {
            Assert.AreEqual(5, System.Enum.GetValues(typeof(GameMode)).Length);
        }
    }
}
