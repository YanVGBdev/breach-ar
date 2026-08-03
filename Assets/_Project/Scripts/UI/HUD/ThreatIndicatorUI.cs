using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BreachAR.UI
{
    /// <summary>
    /// Threat indicator for off-screen enemies
    /// </summary>
    public class ThreatIndicatorUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image arrowImage;
        [SerializeField] private Image backgroundCircle;
        [SerializeField] private TextMeshProUGUI distanceText;

        [Header("Settings")]
        [SerializeField] private float indicatorDistance = 80f;
        [SerializeField] private float pulseSpeed = 2f;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color dangerColor = Color.red;
        [SerializeField] private Color bossColor = Color.magenta;

        private Transform targetTransform;
        private Camera mainCamera;
        private bool isActive;

        private void Start()
        {
            mainCamera = Camera.main;
            SetColor(normalColor);
        }

        private void Update()
        {
            if (!isActive || targetTransform == null || mainCamera == null)
                return;

            UpdateIndicatorPosition();
            UpdatePulse();
        }

        /// <summary>
        /// Initialize indicator with target
        /// </summary>
        public void Initialize(Transform target, ThreatType type = ThreatType.Fragment)
        {
            targetTransform = target;
            isActive = true;
            gameObject.SetActive(true);

            // Set color based on threat type
            switch (type)
            {
                case ThreatType.Fragment:
                    SetColor(normalColor);
                    break;
                case ThreatType.Rift:
                    SetColor(dangerColor);
                    break;
                case ThreatType.Boss:
                    SetColor(bossColor);
                    break;
            }
        }

        /// <summary>
        /// Update indicator position
        /// </summary>
        private void UpdateIndicatorPosition()
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(targetTransform.position);

            // Check if target is on screen
            bool isOnScreen = screenPos.x > 0 && screenPos.x < Screen.width &&
                              screenPos.y > 0 && screenPos.y < Screen.height &&
                              screenPos.z > 0;

            if (isOnScreen)
            {
                // Hide indicator when target is on screen
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);

            // Calculate position at screen edge
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 direction = ((Vector2)screenPos - screenCenter).normalized;

            // Position at edge
            transform.position = screenCenter + direction * indicatorDistance;

            // Rotate arrow to point toward target
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            // Update distance text
            if (distanceText != null)
            {
                float distance = Vector3.Distance(mainCamera.transform.position, targetTransform.position);
                distanceText.text = $"{distance:F0}m";
            }
        }

        /// <summary>
        /// Update pulse animation
        /// </summary>
        private void UpdatePulse()
        {
            if (arrowImage == null) return;

            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            float scale = 1f + pulse * 0.2f;
            transform.localScale = Vector3.one * scale;
        }

        /// <summary>
        /// Set indicator color
        /// </summary>
        private void SetColor(Color color)
        {
            if (arrowImage != null)
                arrowImage.color = color;
            if (backgroundCircle != null)
                backgroundCircle.color = new Color(color.r, color.g, color.b, 0.3f);
        }

        /// <summary>
        /// Deactivate indicator
        /// </summary>
        public void Deactivate()
        {
            isActive = false;
            gameObject.SetActive(false);
        }
    }
}
