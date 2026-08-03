using UnityEngine;
using UnityEngine.UI;

namespace BreachAR.Utils
{
    /// <summary>
    /// Image extension methods
    /// </summary>
    public static class ImageExtensions
    {
        /// <summary>
        /// Set color with alpha
        /// </summary>
        public static void SetColor(this Image image, Color color)
        {
            image.color = color;
        }

        /// <summary>
        /// Set alpha
        /// </summary>
        public static void SetAlpha(this Image image, float alpha)
        {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }

        /// <summary>
        /// Set fill amount
        /// </summary>
        public static void SetFillAmount(this Image image, float amount)
        {
            image.fillAmount = Mathf.Clamp01(amount);
        }

        /// <summary>
        /// Set sprite safely
        /// </summary>
        public static void SetSpriteSafe(this Image image, Sprite sprite)
        {
            if (image != null)
            {
                image.sprite = sprite;
                image.enabled = sprite != null;
            }
        }

        /// <summary>
        /// Fade in
        /// </summary>
        public static void FadeIn(this Image image, float duration, float targetAlpha = 1f)
        {
            if (image != null)
            {
                Color color = image.color;
                color.a = 0f;
                image.color = color;
                image.enabled = true;
                // Would need coroutine for actual fade
            }
        }

        /// <summary>
        /// Fade out
        /// </summary>
        public static void FadeOut(this Image image, float duration)
        {
            if (image != null)
            {
                // Would need coroutine for actual fade
                image.enabled = false;
            }
        }
    }
}
