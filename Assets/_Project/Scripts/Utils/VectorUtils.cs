using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Vector utility functions
    /// </summary>
    public static class VectorUtils
    {
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
        /// Get random point on bounds surface
        /// </summary>
        public static Vector3 RandomPointOnBounds(Bounds bounds)
        {
            int side = Random.Range(0, 6);
            switch (side)
            {
                case 0: // Min X
                    return new Vector3(bounds.min.x, Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
                case 1: // Max X
                    return new Vector3(bounds.max.x, Random.Range(bounds.min.y, bounds.max.y), Random.Range(bounds.min.z, bounds.max.z));
                case 2: // Min Y
                    return new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.min.y, Random.Range(bounds.min.z, bounds.max.z));
                case 3: // Max Y
                    return new Vector3(Random.Range(bounds.min.x, bounds.max.x), bounds.max.y, Random.Range(bounds.min.z, bounds.max.z));
                case 4: // Min Z
                    return new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), bounds.min.z);
                case 5: // Max Z
                    return new Vector3(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y), bounds.max.z);
                default:
                    return bounds.center;
            }
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
        /// Get distance from point to line segment
        /// </summary>
        public static float DistanceToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            return Vector3.Distance(point, ClosestPointOnLineSegment(point, lineStart, lineEnd));
        }

        /// <summary>
        /// Check if point is inside bounds
        /// </summary>
        public static bool IsInsideBounds(Vector3 point, Bounds bounds)
        {
            return bounds.Contains(point);
        }

        /// <summary>
        /// Get direction from one point to another (XZ plane only)
        /// </summary>
        public static Vector3 DirectionXZ(Vector3 from, Vector3 to)
        {
            Vector3 direction = to - from;
            direction.y = 0;
            return direction.normalized;
        }

        /// <summary>
        /// Get angle between two vectors in XZ plane
        /// </summary>
        public static float AngleXZ(Vector3 from, Vector3 to)
        {
            Vector3 dir1 = from.normalized;
            Vector3 dir2 = to.normalized;
            dir1.y = 0;
            dir2.y = 0;
            return Vector3.Angle(dir1, dir2);
        }

        /// <summary>
        /// Smooth damp vector3
        /// </summary>
        public static Vector3 SmoothDamp(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime)
        {
            return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime);
        }

        /// <summary>
        /// Lerp vector3 with unclamped t
        /// </summary>
        public static Vector3 LerpUnclamped(Vector3 a, Vector3 b, float t)
        {
            return Vector3.LerpUnclamped(a, b, t);
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
