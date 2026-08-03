using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Bounds extension methods
    /// </summary>
    public static class BoundsExtensions
    {
        /// <summary>
        /// Get random point in bounds
        /// </summary>
        public static Vector3 GetRandomPoint(this Bounds bounds)
        {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        /// <summary>
        /// Get random point on surface
        /// </summary>
        public static Vector3 GetRandomSurfacePoint(this Bounds bounds)
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
        /// Check if bounds overlap
        /// </summary>
        public static bool Overlaps(this Bounds bounds1, Bounds bounds2)
        {
            return bounds1.Intersects(bounds2);
        }

        /// <summary>
        /// Get intersection bounds
        /// </summary>
        public static Bounds GetIntersection(this Bounds bounds1, Bounds bounds2)
        {
            Vector3 min = Vector3.Max(bounds1.min, bounds2.min);
            Vector3 max = Vector3.Min(bounds1.max, bounds2.max);
            
            if (min.x > max.x || min.y > max.y || min.z > max.z)
            {
                return new Bounds(Vector3.zero, Vector3.zero);
            }
            
            return new Bounds((min + max) / 2f, max - min);
        }

        /// <summary>
        /// Expand bounds
        /// </summary>
        public static Bounds Expand(this Bounds bounds, float amount)
        {
            bounds.Expand(amount);
            return bounds;
        }

        /// <summary>
        /// Expand bounds by vector
        /// </summary>
        public static Bounds Expand(this Bounds bounds, Vector3 amount)
        {
            bounds.Expand(amount);
            return bounds;
        }

        /// <summary>
        /// Encapsulate another bounds
        /// </summary>
        public static Bounds Encapsulate(this Bounds bounds, Bounds other)
        {
            bounds.Encapsulate(other);
            return bounds;
        }

        /// <summary>
        /// Encapsulate a point
        /// </summary>
        public static Bounds Encapsulate(this Bounds bounds, Vector3 point)
        {
            bounds.Encapsulate(point);
            return bounds;
        }
    }
}
