using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Physics extension methods
    /// </summary>
    public static class PhysicsExtensions
    {
        /// <summary>
        /// Check if point is in bounds
        /// </summary>
        public static bool IsInBounds(this Vector3 point, Bounds bounds)
        {
            return bounds.Contains(point);
        }

        /// <summary>
        /// Get closest point on bounds
        /// </summary>
        public static Vector3 ClosestPointOnBounds(this Vector3 point, Bounds bounds)
        {
            return bounds.ClosestPoint(point);
        }

        /// <summary>
        /// Get distance to bounds
        /// </summary>
        public static float DistanceToBounds(this Vector3 point, Bounds bounds)
        {
            return Vector3.Distance(point, bounds.ClosestPoint(point));
        }

        /// <summary>
        /// Check if point is in sphere
        /// </summary>
        public static bool IsInSphere(this Vector3 point, Vector3 center, float radius)
        {
            return Vector3.Distance(point, center) <= radius;
        }

        /// <summary>
        /// Check if point is in capsule
        /// </summary>
        public static bool IsInCapsule(this Vector3 point, Vector3 point1, Vector3 point2, float radius)
        {
            Vector3 closest = ClosestPointOnLineSegment(point, point1, point2);
            return Vector3.Distance(point, closest) <= radius;
        }

        /// <summary>
        /// Get closest point on line segment
        /// </summary>
        public static Vector3 ClosestPointOnLineSegment(this Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            Vector3 line = lineEnd - lineStart;
            float t = Mathf.Clamp01(Vector3.Dot(point - lineStart, line) / Vector3.Dot(line, line));
            return lineStart + t * line;
        }

        /// <summary>
        /// Check if lines intersect
        /// </summary>
        public static bool LinesIntersect(this Vector3 a1, Vector3 a2, Vector3 b1, Vector3 b2)
        {
            Vector3 a = a2 - a1;
            Vector3 b = b2 - b1;
            float d = Vector3.Dot(a, b);
            Vector3 r = a1 - b1;
            float denominator = Vector3.Dot(b, b) * Vector3.Dot(a, a) - d * d;
            
            if (Mathf.Approximately(denominator, 0f)) return false;
            
            float t = (d * Vector3.Dot(r, b) - Vector3.Dot(b, b) * Vector3.Dot(r, a)) / denominator;
            float s = (a * d).sqrMagnitude > 0.0001f ? 
                (Vector3.Dot(a, r) * d - Vector3.Dot(a, a) * Vector3.Dot(r, b)) / denominator : 0f;
            
            return t >= 0 && t <= 1 && s >= 0 && s <= 1;
        }
    }
}
