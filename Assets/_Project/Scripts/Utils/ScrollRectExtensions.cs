using UnityEngine;
using UnityEngine.UI;

namespace BreachAR.Utils
{
    /// <summary>
    /// ScrollRect extension methods
    /// </summary>
    public static class ScrollRectExtensions
    {
        /// <summary>
        /// Scroll to top
        /// </summary>
        public static void ScrollToTop(this ScrollRect scrollRect)
        {
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }

        /// <summary>
        /// Scroll to bottom
        /// </summary>
        public static void ScrollToBottom(this ScrollRect scrollRect)
        {
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f;
            }
        }

        /// <summary>
        /// Scroll to left
        /// </summary>
        public static void ScrollToLeft(this ScrollRect scrollRect)
        {
            if (scrollRect != null)
            {
                scrollRect.horizontalNormalizedPosition = 0f;
            }
        }

        /// <summary>
        /// Scroll to right
        /// </summary>
        public static void ScrollToRight(this ScrollRect scrollRect)
        {
            if (scrollRect != null)
            {
                scrollRect.horizontalNormalizedPosition = 1f;
            }
        }

        /// <summary>
        /// Scroll to position
        /// </summary>
        public static void ScrollTo(this ScrollRect scrollRect, float horizontal, float vertical)
        {
            if (scrollRect != null)
            {
                scrollRect.horizontalNormalizedPosition = Mathf.Clamp01(horizontal);
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(vertical);
            }
        }

        /// <summary>
        /// Set enabled
        /// </summary>
        public static void SetEnabled(this ScrollRect scrollRect, bool enabled)
        {
            if (scrollRect != null)
            {
                scrollRect.enabled = enabled;
                scrollRect.horizontal = enabled;
                scrollRect.vertical = enabled;
            }
        }
    }
}
