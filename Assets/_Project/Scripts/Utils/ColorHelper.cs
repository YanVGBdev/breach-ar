using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Color helper utility functions
    /// </summary>
    public static class ColorHelper
    {
        /// <summary>
        /// Create color from hex string
        /// </summary>
        public static Color FromHex(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }
            return Color.white;
        }

        /// <summary>
        /// Convert color to hex string
        /// </summary>
        public static string ToHex(Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        /// <summary>
        /// Set alpha of color
        /// </summary>
        public static Color WithAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// Get color based on health percentage
        /// </summary>
        public static Color HealthToColor(float healthPercent)
        {
            healthPercent = Mathf.Clamp01(healthPercent);
            return Color.Lerp(Color.red, Color.green, healthPercent);
        }

        /// <summary>
        /// Get color based on combo multiplier
        /// </summary>
        public static Color ComboToColor(float multiplier, float maxMultiplier = 5f)
        {
            float t = (multiplier - 1f) / (maxMultiplier - 1f);
            t = Mathf.Clamp01(t);

            if (t < 0.5f)
            {
                return Color.Lerp(Color.white, Color.yellow, t * 2f);
            }
            else
            {
                return Color.Lerp(Color.yellow, Color.red, (t - 0.5f) * 2f);
            }
        }

        /// <summary>
        /// Pulse color effect
        /// </summary>
        public static Color Pulse(Color baseColor, Color pulseColor, float speed = 1f)
        {
            float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;
            return Color.Lerp(baseColor, pulseColor, t);
        }
    }
}
