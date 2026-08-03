using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// AnimationCurve extension methods
    /// </summary>
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Evaluate and clamp
        /// </summary>
        public static float EvaluateClamped(this AnimationCurve curve, float time)
        {
            return curve.Evaluate(Mathf.Clamp01(time));
        }

        /// <summary>
        /// Evaluate with ping pong
        /// </summary>
        public static float EvaluatePingPong(this AnimationCurve curve, float time, float duration)
        {
            float normalizedTime = (time % duration) / duration;
            if ((time / duration) % 2 >= 1)
            {
                normalizedTime = 1f - normalizedTime;
            }
            return curve.Evaluate(normalizedTime);
        }

        /// <summary>
        /// Evaluate with loop
        /// </summary>
        public static float EvaluateLoop(this AnimationCurve curve, float time, float duration)
        {
            float normalizedTime = (time % duration) / duration;
            return curve.Evaluate(normalizedTime);
        }

        /// <summary>
        /// Get min value
        /// </summary>
        public static float GetMinValue(this AnimationCurve curve)
        {
            float min = float.MaxValue;
            foreach (Keyframe key in curve.keys)
            {
                if (key.value < min) min = key.value;
            }
            return min;
        }

        /// <summary>
        /// Get max value
        /// </summary>
        public static float GetMaxValue(this AnimationCurve curve)
        {
            float max = float.MinValue;
            foreach (Keyframe key in curve.keys)
            {
                if (key.value > max) max = key.value;
            }
            return max;
        }

        /// <summary>
        /// Get average value
        /// </summary>
        public static float GetAverageValue(this AnimationCurve curve, int samples = 10)
        {
            float sum = 0f;
            for (int i = 0; i < samples; i++)
            {
                sum += curve.Evaluate((float)i / (samples - 1));
            }
            return sum / samples;
        }
    }
}
