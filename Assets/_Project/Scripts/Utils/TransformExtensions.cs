using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Transform extension methods
    /// </summary>
    public static class TransformExtensions
    {
        /// <summary>
        /// Reset transform
        /// </summary>
        public static void Reset(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Reset local position
        /// </summary>
        public static void ResetLocalPosition(this Transform transform)
        {
            transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Reset local rotation
        /// </summary>
        public static void ResetLocalRotation(this Transform transform)
        {
            transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Reset local scale
        /// </summary>
        public static void ResetLocalScale(this Transform transform)
        {
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
        /// Get distance to target
        /// </summary>
        public static float DistanceTo(this Transform transform, Transform target)
        {
            return Vector3.Distance(transform.position, target.position);
        }

        /// <summary>
        /// Get distance to point
        /// </summary>
        public static float DistanceTo(this Transform transform, Vector3 point)
        {
            return Vector3.Distance(transform.position, point);
        }

        /// <summary>
        /// Set position from another transform
        /// </summary>
        public static void SetPositionFrom(this Transform transform, Transform other)
        {
            transform.position = other.position;
        }

        /// <summary>
        /// Set rotation from another transform
        /// </summary>
        public static void SetRotationFrom(this Transform transform, Transform other)
        {
            transform.rotation = other.rotation;
        }
    }
}
