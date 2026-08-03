using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// CanvasGroup extension methods
    /// </summary>
    public static class CanvasGroupExtensions
    {
        /// <summary>
        /// Set alpha safely
        /// </summary>
        public static void SetAlphaSafe(this CanvasGroup canvasGroup, float alpha)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = Mathf.Clamp01(alpha);
            }
        }

        /// <summary>
        /// Set interactable
        /// </summary>
        public static void SetInteractableSafe(this CanvasGroup canvasGroup, bool interactable)
        {
            if (canvasGroup != null)
            {
                canvasGroup.interactable = interactable;
            }
        }

        /// <summary>
        /// Set blocks raycasts
        /// </summary>
        public static void SetBlocksRaycastsSafe(this CanvasGroup canvasGroup, bool blocksRaycasts)
        {
            if (canvasGroup != null)
            {
                canvasGroup.blocksRaycasts = blocksRaycasts;
            }
        }

        /// <summary>
        /// Show (enable interaction and raycasts)
        /// </summary>
        public static void Show(this CanvasGroup canvasGroup, float alpha = 1f)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        /// <summary>
        /// Hide (disable interaction and raycasts)
        /// </summary>
        public static void Hide(this CanvasGroup canvasGroup)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// Fade in
        /// </summary>
        public static void FadeIn(this CanvasGroup canvasGroup, float targetAlpha = 1f)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                // Would need coroutine for actual fade
                canvasGroup.alpha = targetAlpha;
            }
        }

        /// <summary>
        /// Fade out
        /// </summary>
        public static void FadeOut(this CanvasGroup canvasGroup)
        {
            if (canvasGroup != null)
            {
                // Would need coroutine for actual fade
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
}
