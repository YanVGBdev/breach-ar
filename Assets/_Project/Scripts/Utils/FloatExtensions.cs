using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Float extension methods
    /// </summary>
    public static class FloatExtensions
    {
        /// <summary>
        /// Clamp float
        /// </summary>
        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// Clamp01 float
        /// </summary>
        public static float Clamp01(this float value)
        {
            return Mathf.Clamp01(value);
        }

        /// <summary>
        /// Remap float
        /// </summary>
        public static float Remap(this float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return MathUtils.Remap(value, fromMin, fromMax, toMin, toMax);
        }

        /// <summary>
        /// Check if approximately zero
        /// </summary>
        public static bool ApproximatelyZero(this float value, float tolerance = 0.0001f)
        {
            return Mathf.Abs(value) < tolerance;
        }

        /// <summary>
        /// Check if approximately equal
        /// </summary>
        public static bool Approximately(this float value, float target, float tolerance = 0.0001f)
        {
            return Mathf.Abs(value - target) < tolerance;
        }

        /// <summary>
        /// Smooth step
        /// </summary>
        public static float SmoothStep(this float value, float from, float to)
        {
            return Mathf.SmoothStep(from, to, value);
        }

        /// <summary>
        /// Inverse lerp
        /// </summary>
        public static float InverseLerp(this float value, float a, float b)
        {
            return MathUtils.InverseLerp(a, b, value);
        }
    }
}
