using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using VContainer;

namespace BreachAR.Utils
{
    /// <summary>
    /// Manages in-game notifications
    /// </summary>
    public class NotificationManager : MonoBehaviour
    {

        [Header("UI")]
        [SerializeField] private CanvasGroup notificationPanel;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Image iconImage;
        [SerializeField] private Animator animator;

        [Header("Settings")]
        [SerializeField] private float defaultDuration = 2f;
        [SerializeField] private float fadeDuration = 0.3f;

        private Coroutine currentNotification;

        [Inject]
        private void Initialize()
        {
            HideImmediate();
        }

        /// <summary>
        /// Show notification
        /// </summary>
        public void Show(string message, float duration = -1f)
        {
            if (duration < 0) duration = defaultDuration;

            if (currentNotification != null)
            {
                StopCoroutine(currentNotification);
            }

            currentNotification = StartCoroutine(ShowNotificationCoroutine(message, duration));
        }

        /// <summary>
        /// Show notification with icon
        /// </summary>
        public void Show(string message, Sprite icon, float duration = -1f)
        {
            if (iconImage != null)
            {
                iconImage.sprite = icon;
                iconImage.gameObject.SetActive(true);
            }

            Show(message, duration);
        }

        /// <summary>
        /// Show success notification
        /// </summary>
        public void ShowSuccess(string message)
        {
            Show($"✓ {message}", 2f);
        }

        /// <summary>
        /// Show error notification
        /// </summary>
        public void ShowError(string message)
        {
            Show($"✗ {message}", 3f);
        }

        /// <summary>
        /// Show warning notification
        /// </summary>
        public void ShowWarning(string message)
        {
            Show($"⚠ {message}", 2.5f);
        }

        private IEnumerator ShowNotificationCoroutine(string message, float duration)
        {
            // Set message
            if (messageText != null)
            {
                messageText.text = message;
            }

            // Fade in
            if (notificationPanel != null)
            {
                float elapsed = 0f;
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    notificationPanel.alpha = elapsed / fadeDuration;
                    yield return null;
                }
                notificationPanel.alpha = 1f;
            }

            // Play animation
            if (animator != null)
            {
                animator.SetTrigger("Show");
            }

            // Wait
            yield return new WaitForSecondsRealtime(duration);

            // Fade out
            if (notificationPanel != null)
            {
                float elapsed = 0f;
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    notificationPanel.alpha = 1f - (elapsed / fadeDuration);
                    yield return null;
                }
                notificationPanel.alpha = 0f;
            }

            // Hide icon
            if (iconImage != null)
            {
                iconImage.gameObject.SetActive(false);
            }

            currentNotification = null;
        }

        /// <summary>
        /// Hide notification immediately
        /// </summary>
        public void HideImmediate()
        {
            if (notificationPanel != null)
            {
                notificationPanel.alpha = 0f;
            }
            if (iconImage != null)
            {
                iconImage.gameObject.SetActive(false);
            }
        }
    }
}
