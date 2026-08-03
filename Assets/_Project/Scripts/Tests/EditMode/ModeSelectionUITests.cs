using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.UI;
using BreachAR.Core;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for ModeSelectionUI
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class ModeSelectionUITests
    {
        [Test]
        public void ModeSelectionUI_InitialState_IsHidden()
        {
            var gameObject = new GameObject();
            var modeUI = gameObject.AddComponent<ModeSelectionUI>();

            Assert.IsFalse(modeUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void GameMode_Campaign_Exists()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(GameMode), GameMode.Campaign));
        }

        [Test]
        public void GameMode_Endless_Exists()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(GameMode), GameMode.Endless));
        }

        [Test]
        public void GameMode_DailyChallenge_Exists()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(GameMode), GameMode.DailyChallenge));
        }

        [Test]
        public void GameMode_Zen_Exists()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(GameMode), GameMode.Zen));
        }
    }
}
