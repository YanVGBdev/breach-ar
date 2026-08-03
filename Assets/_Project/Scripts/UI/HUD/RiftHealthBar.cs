using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Gameplay;

namespace BreachAR.UI
{
    /// <summary>
    /// World Space health bar for Rift integrity
    /// Referência: UI-008, specs/RiftSystem.md
    /// </summary>
    public class RiftHealthBar : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Canvas worldCanvas;
        [SerializeField] private Slider integrityBar;
        [SerializeField] private Image integrityFill;
        [SerializeField] private TextMeshProUGUI integrityText;

        [Header("Settings")]
        [SerializeField] private float followHeight = 1.5f;
        [SerializeField] private float lerpSpeed = 5f;
        [SerializeField] private Color healthyColor = Color.green;
        [SerializeField] private Color damagedColor = Color.yellow;
        [SerializeField] private Color criticalColor = Color.red;

        private RiftController riftController;
        private Camera mainCamera;
        private float currentDisplayValue;

        /// <summary>
        /// Initialize with rift controller
        /// </summary>
        public void Initialize(RiftController rift)
        {
            riftController = rift;
            mainCamera = Camera.main;

            if (worldCanvas != null)
            {
                worldCanvas.renderMode = RenderMode.WorldSpace;
                worldCanvas.worldCamera = mainCamera;
            }

            currentDisplayValue = rift.IntegrityPercentage;
            UpdateDisplay();
        }

        private void LateUpdate()
        {
            if (riftController == null || mainCamera == null) return;

            // Follow rift position
            transform.position = riftController.transform.position + Vector3.up * followHeight;

            // Face camera
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0);

            // Update integrity display
            UpdateDisplay();
        }

        /// <summary>
        /// Update the integrity bar display
        /// </summary>
        private void UpdateDisplay()
        {
            if (riftController == null) return;

            float targetValue = riftController.IntegrityPercentage;
            currentDisplayValue = Mathf.Lerp(currentDisplayValue, targetValue, Time.deltaTime * lerpSpeed);

            if (integrityBar != null)
            {
                integrityBar.value = currentDisplayValue;
            }

            // Update color based on integrity
            if (integrityFill != null)
            {
                if (currentDisplayValue > 0.6f)
                    integrityFill.color = healthyColor;
                else if (currentDisplayValue > 0.3f)
                    integrityFill.color = damagedColor;
                else
                    integrityFill.color = criticalColor;
            }

            // Update text
            if (integrityText != null)
            {
                integrityText.text = $"{Mathf.RoundToInt(currentDisplayValue * 100)}%";
            }
        }

        /// <summary>
        /// Show the health bar
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Hide the health bar
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
