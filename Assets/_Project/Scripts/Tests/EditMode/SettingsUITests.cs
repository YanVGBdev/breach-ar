using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using BreachAR.UI;
using BreachAR.Core;

namespace BreachAR.Tests.EditMode
{
    /// <summary>
    /// Tests for SettingsUI
    /// Referência: 99_agent_rules.md §99.3.12
    /// </summary>
    public class SettingsUITests
    {
        [Test]
        public void SettingsUI_InitialState_IsHidden()
        {
            var gameObject = new GameObject();
            var settingsUI = gameObject.AddComponent<SettingsUI>();

            Assert.IsFalse(settingsUI.gameObject.activeSelf);
            
            Object.DestroyImmediate(gameObject);
        }

        [Test]
        public void GameSettings_DefaultValues_AreCorrect()
        {
            var settings = new GameSettings();

            Assert.AreEqual(1f, settings.MasterVolume);
            Assert.AreEqual(0.8f, settings.MusicVolume);
            Assert.AreEqual(1f, settings.SFXVolume);
            Assert.AreEqual(1, settings.QualityLevel);
            Assert.AreEqual(1f, settings.Sensitivity);
            Assert.AreEqual(1f, settings.FontScale);
            Assert.IsFalse(settings.HighContrast);
            Assert.IsFalse(settings.ReduceShake);
        }

        [Test]
        public void GameSettings_CanBeModified()
        {
            var settings = new GameSettings();

            settings.MasterVolume = 0.5f;
            settings.MusicVolume = 0.3f;
            settings.SFXVolume = 0.7f;
            settings.QualityLevel = 2;
            settings.Sensitivity = 1.5f;
            settings.FontScale = 1.2f;
            settings.HighContrast = true;
            settings.ReduceShake = true;

            Assert.AreEqual(0.5f, settings.MasterVolume);
            Assert.AreEqual(0.3f, settings.MusicVolume);
            Assert.AreEqual(0.7f, settings.SFXVolume);
            Assert.AreEqual(2, settings.QualityLevel);
            Assert.AreEqual(1.5f, settings.Sensitivity);
            Assert.AreEqual(1.2f, settings.FontScale);
            Assert.IsTrue(settings.HighContrast);
            Assert.IsTrue(settings.ReduceShake);
        }

        [Test]
        public void GameSettings_Serializable()
        {
            var settings = new GameSettings
            {
                MasterVolume = 0.6f,
                MusicVolume = 0.4f,
                SFXVolume = 0.8f,
                QualityLevel = 1,
                Sensitivity = 1.2f,
                FontScale = 1.1f,
                HighContrast = false,
                ReduceShake = true
            };

            string json = JsonUtility.ToJson(settings);
            var loaded = JsonUtility.FromJson<GameSettings>(json);

            Assert.AreEqual(settings.MasterVolume, loaded.MasterVolume);
            Assert.AreEqual(settings.MusicVolume, loaded.MusicVolume);
            Assert.AreEqual(settings.SFXVolume, loaded.SFXVolume);
            Assert.AreEqual(settings.QualityLevel, loaded.QualityLevel);
            Assert.AreEqual(settings.Sensitivity, loaded.Sensitivity);
            Assert.AreEqual(settings.FontScale, loaded.FontScale);
            Assert.AreEqual(settings.HighContrast, loaded.HighContrast);
            Assert.AreEqual(settings.ReduceShake, loaded.ReduceShake);
        }
    }
}
