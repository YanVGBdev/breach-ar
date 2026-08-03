using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Math helper utility functions
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Smoothly damp angle
        /// </summary>
        public static float SmoothDampAngle(float current, float target, ref float velocity, float smoothTime)
        {
            return Mathf.SmoothDampAngle(current, target, ref velocity, smoothTime);
        }

        /// <summary>
        /// Smoothly damp vector3
        /// </summary>
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 velocity, float smoothTime)
        {
            return Vector3.SmoothDamp(current, target, ref velocity, smoothTime);
        }

        /// <summary>
        /// Inverse lerp
        /// </summary>
        public static float InverseLerp(float a, float b, float value)
        {
            if (Mathf.Approximately(a, b)) return 0f;
            return Mathf.Clamp01((value - a) / (b - a));
        }

        /// <summary>
        /// Remap value from one range to another
        /// </summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

        /// <summary>
        /// Smooth step
        /// </summary>
        public static float SmoothStep(float from, float to, float t)
        {
            t = Mathf.Clamp01(t);
            t = t * t * (3f - 2f * t);
            return from + (to - from) * t;
        }

        /// <summary>
        /// Damped spring
        /// </summary>
        public static float DampedSpring(float current, float target, ref float velocity, float damping, float frequency, float deltaTime)
        {
            float f = 1f + 2f * damping * frequency * deltaTime;
            float kk = frequency * frequency * deltaTime * deltaTime;
            float dampedK = damping * frequency * deltaTime;
            float velocityTerm = velocity + dampedK * (target - current);
            float newValue = (current + deltaTime * velocityTerm) / (1f + f + kk);
            velocity = (velocityTerm - frequency * frequency * deltaTime * (newValue - current)) / (1f + f + kk);
            return newValue;
        }

        /// <summary>
        /// Wrap angle
        /// </summary>
        public static float WrapAngle(float angle)
        {
            angle = angle % 360f;
            if (angle < 0f) angle += 360f;
            return angle;
        }

        /// <summary>
        /// Shortest distance between two angles
        /// </summary>
        public static float ShortestAngleDistance(float from, float to)
        {
            float diff = Mathf.DeltaAngle(from, to);
            return diff;
        }

        /// <summary>
        /// Check if value is approximately zero
        /// </summary>
        public static bool ApproximatelyZero(float value, float tolerance = 0.0001f)
        {
            return Mathf.Abs(value) < tolerance;
        }

        /// <summary>
        /// Check if vectors are approximately equal
        /// </summary>
        public static bool ApproximatelyEqual(Vector3 a, Vector3 b, float tolerance = 0.001f)
        {
            return Vector3.Distance(a, b) < tolerance;
        }

        /// <summary>
        /// Get random value between min and max
        /// </summary>
        public static float RandomRange(float min, float max)
        {
            return Random.Range(min, max);
        }

        /// <summary>
        /// Get random integer between min and max (inclusive)
        /// </summary>
        public static int RandomRange(int min, int max)
        {
            return Random.Range(min, max + 1);
        }

        /// <summary>
        /// Check if point is in front of transform
        /// </summary>
        public static bool IsInFront(Transform transform, Vector3 point)
        {
            return Vector3.Dot(transform.forward, point - transform.position) > 0;
        }

        /// <summary>
        /// Check if point is to the left of transform
        /// </summary>
        public static bool IsToLeft(Transform transform, Vector3 point)
        {
            return Vector3.Dot(transform.right, point - transform.position) < 0;
        }

        /// <summary>
        /// Get signed angle between vectors
        /// </summary>
        public static float SignedAngle(Vector3 from, Vector3 to, Vector3 axis)
        {
            return Vector3.SignedAngle(from, to, axis);
        }

        /// <summary>
        /// Clamp vector magnitude
        /// </summary>
        public static Vector3 ClampMagnitude(Vector3 vector, float maxLength)
        {
            return Vector3.ClampMagnitude(vector, maxLength);
        }
    }
}
