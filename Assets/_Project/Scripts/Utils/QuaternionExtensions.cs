using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Quaternion extension methods
    /// </summary>
    public static class QuaternionExtensions
    {
        /// <summary>
        /// Set X rotation
        /// </summary>
        public static Quaternion WithX(this Quaternion rotation, float x)
        {
            return Quaternion.Euler(x, rotation.eulerAngles.y, rotation.eulerAngles.z);
        }

        /// <summary>
        /// Set Y rotation
        /// </summary>
        public static Quaternion WithY(this Quaternion rotation, float y)
        {
            return Quaternion.Euler(rotation.eulerAngles.x, y, rotation.eulerAngles.z);
        }

        /// <summary>
        /// Set Z rotation
        /// </summary>
        public static Quaternion WithZ(this Quaternion rotation, float z)
        {
            return Quaternion.Euler(rotation.eulerAngles.x, rotation.eulerAngles.y, z);
        }

        /// <summary>
        /// Add euler angles
        /// </summary>
        public static Quaternion AddEuler(this Quaternion rotation, Vector3 euler)
        {
            return rotation * Quaternion.Euler(euler);
        }

        /// <summary>
        /// Get angle to target
        /// </summary>
        public static float AngleTo(this Quaternion rotation, Quaternion target)
        {
            return Quaternion.Angle(rotation, target);
        }

        /// <summary>
        /// Smoothly rotate to target
        /// </summary>
        public static Quaternion SmoothRotateTo(this Quaternion rotation, Quaternion target, float speed)
        {
            return Quaternion.Slerp(rotation, target, speed * Time.deltaTime);
        }

        /// <summary>
        /// Get forward direction
        /// </summary>
        public static Vector3 Forward(this Quaternion rotation)
        {
            return rotation * Vector3.forward;
        }

        /// <summary>
        /// Get right direction
        /// </summary>
        public static Vector3 Right(this Quaternion rotation)
        {
            return rotation * Vector3.right;
        }

        /// <summary>
        /// Get up direction
        /// </summary>
        public static Vector3 Up(this Quaternion rotation)
        {
            return rotation * Vector3.up;
        }
    }
}
