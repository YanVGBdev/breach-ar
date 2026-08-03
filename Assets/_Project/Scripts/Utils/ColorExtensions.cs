using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Color extension methods
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Set alpha
        /// </summary>
        public static Color WithAlpha(this Color color, float alpha)
        {
            return new Color(color.r, color.g, color.b, alpha);
        }

        /// <summary>
        /// Set red
        /// </summary>
        public static Color WithRed(this Color color, float red)
        {
            return new Color(red, color.g, color.b, color.a);
        }

        /// <summary>
        /// Set green
        /// </summary>
        public static Color WithGreen(this Color color, float green)
        {
            return new Color(color.r, green, color.b, color.a);
        }

        /// <summary>
        /// Set blue
        /// </summary>
        public static Color WithBlue(this Color color, float blue)
        {
            return new Color(color.r, color.g, blue, color.a);
        }

        /// <summary>
        /// Convert to hex string
        /// </summary>
        public static string ToHex(this Color color)
        {
            return ColorUtility.ToHtmlStringRGB(color);
        }

        /// <summary>
        /// Lerp with unclamped t
        /// </summary>
        public static Color LerpUnclamped(this Color a, Color b, float t)
        {
            return Color.LerpUnclamped(a, b, t);
        }

        /// <summary>
        /// Get inverted color
        /// </summary>
        public static Color Invert(this Color color)
        {
            return new Color(1f - color.r, 1f - color.g, 1f - color.b, color.a);
        }

        /// <summary>
        /// Set saturation
        /// </summary>
        public static Color WithSaturation(this Color color, float saturation)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            return Color.HSVToRGB(h, saturation, v).WithAlpha(color.a);
        }

        /// <summary>
        /// Set value/brightness
        /// </summary>
        public static Color WithValue(this Color color, float value)
        {
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            return Color.HSVToRGB(h, s, value).WithAlpha(color.a);
        }
    }
}
