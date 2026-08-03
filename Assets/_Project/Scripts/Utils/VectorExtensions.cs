using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Vector extension methods
    /// </summary>
    public static class VectorExtensions
    {
        /// <summary>
        /// Set X component
        /// </summary>
        public static Vector3 WithX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        /// <summary>
        /// Set Y component
        /// </summary>
        public static Vector3 WithY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        /// <summary>
        /// Set Z component
        /// </summary>
        public static Vector3 WithZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        /// <summary>
        /// Flatten to XZ plane
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
        /// Set X component of Vector2
        /// </summary>
        public static Vector2 WithX(this Vector2 vector, float x)
        {
            return new Vector2(x, vector.y);
        }

        /// <summary>
        /// Set Y component of Vector2
        /// </summary>
        public static Vector2 WithY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        /// <summary>
        /// Convert Vector2 to Vector3
        /// </summary>
        public static Vector3 ToVector3(this Vector2 vector, float z = 0f)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        /// <summary>
        /// Convert Vector3 to Vector2
        /// </summary>
        public static Vector2 ToVector2(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.y);
        }
    }
}
