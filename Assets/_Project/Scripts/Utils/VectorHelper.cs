using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Vector helper utility functions
    /// </summary>
    public static class VectorHelper
    {
        /// <summary>
        /// Set X component of vector
        /// </summary>
        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        /// <summary>
        /// Set Y component of vector
        /// </summary>
        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        /// <summary>
        /// Set Z component of vector
        /// </summary>
        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        /// <summary>
        /// Flatten vector to XZ plane
        /// </summary>
        public static Vector3 Flatten(this Vector3 vector)
        {
            return new Vector3(vector.x, 0, vector.z);
        }

        /// <summary>
        /// Get distance on XZ plane
        /// </summary>
        public static float DistanceXZ(this Vector3 a, Vector3 b)
        {
            return Vector3.Distance(a.Flatten(), b.Flatten());
        }

        /// <summary>
        /// Get random point in bounds
        /// </summary>
        public static Vector3 RandomPointInBounds(Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        /// <summary>
        /// Get point on circle
        /// </summary>
        public static Vector3 PointOnCircle(float radius, float angle)
        {
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
            return new Vector3(x, 0, z);
        }

        /// <summary>
        /// Get point on sphere
        /// </summary>
        public static Vector3 PointOnSphere(float radius, float theta, float phi)
        {
            float x = radius * Mathf.Sin(phi) * Mathf.Cos(theta);
            float y = radius * Mathf.Cos(phi);
            float z = radius * Mathf.Sin(phi) * Mathf.Sin(theta);
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Project point onto plane
        /// </summary>
        public static Vector3 ProjectOntoPlane(Vector3 point, Vector3 planeNormal, Vector3 planePoint)
        {
            return point - Vector3.Dot(point - planePoint, planeNormal) * planeNormal;
        }

        /// <summary>
        /// Get closest point on line segment
        /// </summary>
        public static Vector3 ClosestPointOnLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 line = lineEnd - lineStart;
            float t = Mathf.Clamp01(Vector3.Dot(point - lineStart, line) / Vector3.Dot(line, line));
            return lineStart + t * line;
        }

        /// <summary>
        /// Check if vectors are approximately equal
        /// </summary>
        public static bool Approximately(Vector3 a, Vector3 b, float tolerance = 0.001f)
        {
            return Vector3.Distance(a, b) < tolerance;
        }
    }
}
