using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Collider extension methods
    /// </summary>
    public static class ColliderExtensions
    {
        /// <summary>
        /// Check if point is inside collider
        /// </summary>
        public static bool Contains(this Collider collider, Vector3 point)
        {
            return collider.bounds.Contains(point);
        }

        /// <summary>
        /// Get closest point on collider
        /// </summary>
        public static Vector3 ClosestPoint(this Collider collider, Vector3 point)
        {
            return collider.ClosestPoint(point);
        }

        /// <summary>
        /// Get center of collider
        /// </summary>
        public static Vector3 GetCenter(this Collider collider)
        {
            return collider.bounds.center;
        }

        /// <summary>
        /// Get size of collider
        /// </summary>
        public static Vector3 GetSize(this Collider collider)
        {
            return collider.bounds.size;
        }

        /// <summary>
        /// Get extents of collider
        /// </summary>
        public static Vector3 GetExtents(this Collider collider)
        {
            return collider.bounds.extents;
        }

        /// <summary>
        /// Check if collider overlaps point with radius
        /// </summary>
        public static bool OverlapsSphere(this Collider collider, Vector3 point, float radius)
        {
            return Physics.CheckSphere(point, radius, 1 << collider.gameObject.layer);
        }
    }
}
