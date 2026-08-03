using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Gradient extension methods
    /// </summary>
    public static class GradientExtensions
    {
        /// <summary>
        /// Evaluate at time
        /// </summary>
        public static Color Evaluate(this Gradient gradient, float time)
        {
            return gradient.Evaluate(Mathf.Clamp01(time));
        }

        /// <summary>
        /// Get color at normalized position
        /// </summary>
        public static Color GetColorAtPosition(this Gradient gradient, float position)
        {
            return gradient.Evaluate(Mathf.Clamp01(position));
        }

        /// <summary>
        /// Create gradient from two colors
        /// </summary>
        public static Gradient CreateSimple(Color startColor, Color endColor)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[]
            {
                new GradientColorKey(startColor, 0f),
                new GradientColorKey(endColor, 1f)
            };
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey(startColor.a, 0f),
                new GradientAlphaKey(endColor.a, 1f)
            };
            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        /// <summary>
        /// Create gradient from multiple colors
        /// </summary>
        public static Gradient CreateFromColors(params Color[] colors)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[colors.Length];
            
            for (int i = 0; i < colors.Length; i++)
            {
                colorKeys[i] = new GradientColorKey(colors[i], (float)i / (colors.Length - 1));
            }
            
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[]
            {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(1f, 1f)
            };
            
            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        /// <summary>
        /// Lerp between two gradients
        /// </summary>
        public static Gradient Lerp(Gradient a, Gradient b, float t)
        {
            // Sample both gradients and blend
            Gradient result = new Gradient();
            int sampleCount = 10;
            GradientColorKey[] colorKeys = new GradientColorKey[sampleCount];
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[sampleCount];
            
            for (int i = 0; i < sampleCount; i++)
            {
                float time = (float)i / (sampleCount - 1);
                Color colorA = a.Evaluate(time);
                Color colorB = b.Evaluate(time);
                Color blended = Color.Lerp(colorA, colorB, t);
                
                colorKeys[i] = new GradientColorKey(blended, time);
                alphaKeys[i] = new GradientAlphaKey(blended.a, time);
            }
            
            result.SetKeys(colorKeys, alphaKeys);
            return result;
        }
    }
}
