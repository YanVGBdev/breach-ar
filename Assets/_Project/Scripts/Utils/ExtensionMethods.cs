using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Utils
{
    /// <summary>
    /// Extension methods for Unity types
    /// </summary>
    public static class ExtensionMethods
    {
        // =====================================================================
        // Transform Extensions
        // =====================================================================

        /// <summary>
        /// Look at target with smooth rotation
        /// </summary>
        public static void LookAtSmooth(this Transform transform, Vector3 target, float speed)
        {
            Vector3 direction = target - transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        }

        /// <summary>
        /// Reset transform to identity
        /// </summary>
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Set position X
        /// </summary>
        public static void SetPositionX(this Transform transform, float x)
        {
            Vector3 position = transform.position;
            position.x = x;
            transform.position = position;
        }

        /// <summary>
        /// Set position Y
        /// </summary>
        public static void SetPositionY(this Transform transform, float y)
        {
            Vector3 position = transform.position;
            position.y = y;
            transform.position = position;
        }

        /// <summary>
        /// Set position Z
        /// </summary>
        public static void SetPositionZ(this Transform transform, float z)
        {
            Vector3 position = transform.position;
            position.z = z;
            transform.position = position;
        }

        // =====================================================================
        // Vector Extensions
        // =====================================================================

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

        // =====================================================================
        // Rigidbody Extensions
        // =====================================================================

        /// <summary>
        /// Add force in direction
        /// </summary>
        public static void AddForceDirection(this Rigidbody rb, Vector3 direction, float force)
        {
            rb.AddForce(direction.normalized * force, ForceMode.Impulse);
        }

        /// <summary>
        /// Set velocity
        /// </summary>
        public static void SetVelocity(this Rigidbody rb, Vector3 velocity)
        {
            rb.linearVelocity = velocity;
        }

        // =====================================================================
        // Collider Extensions
        // =====================================================================

        /// <summary>
        /// Check if point is inside collider
        /// </summary>
        public static bool Contains(this Collider collider, Vector3 point)
        {
            return collider.bounds.Contains(point);
        }

        // =====================================================================
        // List Extensions
        // =====================================================================

        /// <summary>
        /// Get random element from list
        /// </summary>
        public static T RandomElement<T>(this List<T> list)
        {
            if (list == null || list.Count == 0) return default;
            return list[Random.Range(0, list.Count)];
        }

        /// <summary>
        /// Shuffle list
        /// </summary>
        public static void Shuffle<T>(this List<T> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                T temp = list[i];
                list[i] = list[j];
                list[j] = temp;
            }
        }

        // =====================================================================
        // Float Extensions
        // =====================================================================

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
    }
}
