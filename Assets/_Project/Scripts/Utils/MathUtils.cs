using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Math utility functions
    /// </summary>
    public static class MathUtils
    {
        /// <summary>
        /// Remap a value from one range to another
        /// </summary>
        public static float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        }

        /// <summary>
        /// Smooth step between two values
        /// </summary>
        public static float SmoothStep(float from, float to, float t)
        {
            t = Mathf.Clamp01(t);
            t = t * t * (3f - 2f * t);
            return from + (to - from) * t;
        }

        /// <summary>
        /// Inverse lerp (get t from value between a and b)
        /// </summary>
        public static float InverseLerp(float a, float b, float value)
        {
            if (Mathf.Approximately(a, b)) return 0f;
            return Mathf.Clamp01((value - a) / (b - a));
        }

        /// <summary>
        /// Check if value is approximately equal to target
        /// </summary>
        public static bool Approximately(float a, float b, float tolerance = 0.001f)
        {
            return Mathf.Abs(a - b) < tolerance;
        }

        /// <summary>
        /// Snap value to grid
        /// </summary>
        public static float Snap(float value, float snapSize)
        {
            return Mathf.Round(value / snapSize) * snapSize;
        }

        /// <summary>
        /// Snap vector to grid
        /// </summary>
        public static Vector3 SnapVector(Vector3 vector, float snapSize)
        {
            return new Vector3(
                Snap(vector.x, snapSize),
                Snap(vector.y, snapSize),
                Snap(vector.z, snapSize)
            );
        }

        /// <summary>
        /// Get random point in ring
        /// </summary>
        public static Vector2 RandomInRing(float innerRadius, float outerRadius)
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(innerRadius, outerRadius);
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        /// <summary>
        /// Get random point in sphere
        /// </summary>
        public static Vector3 RandomInSphere(float radius)
        {
            return Random.insideUnitSphere * radius;
        }

        /// <summary>
        /// Calculate parabolic trajectory point
        /// </summary>
        public static Vector3 CalculateParabola(Vector3 start, Vector3 end, float height, float t)
        {
            float parabolicT = t * 2f - 1f; // Remap 0-1 to -1 to 1
            Vector3 arc = Vector3.Lerp(start, end, t);
            arc.y += (-parabolicT * parabolicT + 1f) * height;
            return arc;
        }

        /// <summary>
        /// Calculate launch velocity for projectile
        /// </summary>
        public static bool CalculateLaunchVelocity(Vector3 origin, Vector3 target, float speed, out Vector3 velocity)
        {
            Vector3 toTarget = target - origin;
            float g = Physics.gravity.magnitude;
            float y = toTarget.y;
            toTarget.y = 0f;
            float x = toTarget.magnitude;

            float discriminant = speed * speed * speed * speed - g * (g * x * x + 2f * y * speed * speed);

            if (discriminant < 0)
            {
                velocity = Vector3.zero;
                return false;
            }

            float tanTheta = (speed * speed + Mathf.Sqrt(discriminant)) / (g * x);
            float angle = Mathf.Atan(tanTheta);

            velocity = toTarget.normalized * (speed * Mathf.Cos(angle)) + Vector3.up * (speed * Mathf.Sin(angle));
            return true;
        }

        /// <summary>
        /// Clamp angle to -180 to 180
        /// </summary>
        public static float ClampAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }

        /// <summary>
        /// Get angle between two vectors in 2D
        /// </summary>
        public static float GetAngle2D(Vector2 from, Vector2 to)
        {
            Vector2 direction = to - from;
            return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        /// <summary>
        /// Smoothly damp angle
        /// </summary>
        public static float SmoothDampAngle(float current, float target, ref float velocity, float smoothTime)
        {
            return Mathf.SmoothDampAngle(current, target, ref velocity, smoothTime);
        }
    }
}
