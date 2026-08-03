using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BreachAR.Utils
{
    /// <summary>
    /// Text extension methods
    /// </summary>
    public static class TextExtensions
    {
        /// <summary>
        /// Set text safely
        /// </summary>
        public static void SetTextSafe(this Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        /// <summary>
        /// Set text safely (TMP)
        /// </summary>
        public static void SetTextSafe(this TextMeshProUGUI text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }

        /// <summary>
        /// Set color
        /// </summary>
        public static void SetColor(this Text text, Color color)
        {
            if (text != null)
            {
                text.color = color;
            }
        }

        /// <summary>
        /// Set color (TMP)
        /// </summary>
        public static void SetColor(this TextMeshProUGUI text, Color color)
        {
            if (text != null)
            {
                text.color = color;
            }
        }

        /// <summary>
        /// Set alpha
        /// </summary>
        public static void SetAlpha(this Text text, float alpha)
        {
            if (text != null)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }

        /// <summary>
        /// Set alpha (TMP)
        /// </summary>
        public static void SetAlpha(this TextMeshProUGUI text, float alpha)
        {
            if (text != null)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }

        /// <summary>
        /// Set font size
        /// </summary>
        public static void SetFontSize(this TextMeshProUGUI text, float size)
        {
            if (text != null)
            {
                text.fontSize = size;
            }
        }

        /// <summary>
        /// Set enabled
        /// </summary>
        public static void SetEnabled(this Text text, bool enabled)
        {
            if (text != null)
            {
                text.enabled = enabled;
            }
        }

        /// <summary>
        /// Set enabled (TMP)
        /// </summary>
        public static void SetEnabled(this TextMeshProUGUI text, bool enabled)
        {
            if (text != null)
            {
                text.enabled = enabled;
            }
        }
    }
}
