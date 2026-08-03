using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Gameplay;
using System.Collections.Generic;

namespace BreachAR.UI
{
    /// <summary>
    /// Display for active power-ups with cooldown radial
    /// Referência: UI-006, specs/HUD.md
    /// </summary>
    public class PowerUpDisplay : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Transform powerUpContainer;
        [SerializeField] private GameObject powerUpIconPrefab;
        [SerializeField] private int maxDisplayCount = 5;

        private List<ActivePowerUp> activePowerUps = new List<ActivePowerUp>();

        /// <summary>
        /// Add a power-up to display
        /// </summary>
        public void AddPowerUp(string powerUpId, Sprite icon, float duration)
        {
            if (activePowerUps.Count >= maxDisplayCount)
            {
                // Remove oldest
                RemovePowerUp(activePowerUps[0].Id);
            }

            var powerUp = new ActivePowerUp
            {
                Id = powerUpId,
                Icon = icon,
                Duration = duration,
                RemainingTime = duration
            };

            activePowerUps.Add(powerUp);
            RefreshDisplay();
        }

        /// <summary>
        /// Remove a power-up from display
        /// </summary>
        public void RemovePowerUp(string powerUpId)
        {
            activePowerUps.RemoveAll(p => p.Id == powerUpId);
            RefreshDisplay();
        }

        /// <summary>
        /// Clear all power-ups
        /// </summary>
        public void ClearAll()
        {
            activePowerUps.Clear();
            RefreshDisplay();
        }

        private void Update()
        {
            bool needsRefresh = false;

            for (int i = activePowerUps.Count - 1; i >= 0; i--)
            {
                activePowerUps[i].RemainingTime -= Time.deltaTime;

                if (activePowerUps[i].RemainingTime <= 0)
                {
                    activePowerUps.RemoveAt(i);
                    needsRefresh = true;
                }
            }

            if (needsRefresh)
            {
                RefreshDisplay();
            }
            else
            {
                UpdateCooldowns();
            }
        }

        /// <summary>
        /// Update cooldown radial fills
        /// </summary>
        private void UpdateCooldowns()
        {
            for (int i = 0; i < powerUpContainer.childCount && i < activePowerUps.Count; i++)
            {
                Transform child = powerUpContainer.GetChild(i);
                Image fillImage = child.Find("CooldownFill")?.GetComponent<Image>();

                if (fillImage != null)
                {
                    float progress = activePowerUps[i].RemainingTime / activePowerUps[i].Duration;
                    fillImage.fillAmount = progress;
                }
            }
        }

        /// <summary>
        /// Refresh the display
        /// </summary>
        private void RefreshDisplay()
        {
            // Clear existing icons
            foreach (Transform child in powerUpContainer)
            {
                Destroy(child.gameObject);
            }

            // Create new icons
            foreach (var powerUp in activePowerUps)
            {
                if (powerUpIconPrefab == null) continue;

                GameObject icon = Instantiate(powerUpIconPrefab, powerUpContainer);
                icon.name = $"PowerUp_{powerUp.Id}";

                // Set icon image
                Image iconImage = icon.transform.Find("Icon")?.GetComponent<Image>();
                if (iconImage != null && powerUp.Icon != null)
                {
                    iconImage.sprite = powerUp.Icon;
                }
            }
        }

        private class ActivePowerUp
        {
            public string Id;
            public Sprite Icon;
            public float Duration;
            public float RemainingTime;
        }
    }
}
