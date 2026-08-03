using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Color utility functions
    /// </summary>
    public static class ColorUtils
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
        /// Lerp between two colors
        /// </summary>
        public static Color Lerp(Color a, Color b, float t)
        {
            return Color.Lerp(a, b, Mathf.Clamp01(t));
        }

        /// <summary>
        /// Set alpha of color
        /// </summary>
        public static Color WithAlpha(Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// Get color based on health percentage (green to red)
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

            // White -> Yellow -> Orange -> Red
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
        /// Get color for threat type
        /// </summary>
        public static Color ThreatTypeToColor(ThreatType type)
        {
            switch (type)
            {
                case ThreatType.Fragment:
                    return Color.white;
                case ThreatType.Rift:
                    return Color.red;
                case ThreatType.Boss:
                    return Color.magenta;
                default:
                    return Color.white;
            }
        }

        /// <summary>
        /// Get color for element type
        /// </summary>
        public static Color ElementTypeToColor(string elementType)
        {
            switch (elementType.ToLower())
            {
                case "fire":
                    return new Color(1f, 0.5f, 0f); // Orange
                case "ice":
                    return new Color(0.5f, 0.8f, 1f); // Light blue
                case "poison":
                    return new Color(0.5f, 1f, 0.5f); // Light green
                case "energy":
                    return new Color(0.8f, 0.5f, 1f); // Purple
                default:
                    return Color.white;
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

        /// <summary>
        /// Flash color effect
        /// </summary>
        public static bool ShouldFlash(float speed = 2f)
        {
            return Mathf.Sin(Time.time * speed) > 0f;
        }
    }
}
