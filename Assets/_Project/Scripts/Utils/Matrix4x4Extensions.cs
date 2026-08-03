using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Matrix4x4 extension methods
    /// </summary>
    public static class Matrix4x4Extensions
    {
        /// <summary>
        /// Get position from matrix
        /// </summary>
        public static Vector3 GetPosition(this Matrix4x4 matrix)
        {
            return matrix.GetColumn(3);
        }

        /// <summary>
        /// Get rotation from matrix
        /// </summary>
        public static Quaternion GetRotation(this Matrix4x4 matrix)
        {
            return matrix.rotation;
        }

        /// <summary>
        /// Get scale from matrix
        /// </summary>
        public static Vector3 GetScale(this Matrix4x4 matrix)
        {
            return matrix.lossyScale;
        }

        /// <summary>
        /// Transform point
        /// </summary>
        public static Vector3 TransformPoint(this Matrix4x4 matrix, Vector3 point)
        {
            return matrix.MultiplyPoint(point);
        }

        /// <summary>
        /// Transform direction
        /// </summary>
        public static Vector3 TransformDirection(this Matrix4x4 matrix, Vector3 direction)
        {
            return matrix.MultiplyVector(direction);
        }

        /// <summary>
        /// Inverse transform point
        /// </summary>
        public static Vector3 InverseTransformPoint(this Matrix4x4 matrix, Vector3 point)
        {
            return matrix.inverse.MultiplyPoint(point);
        }

        /// <summary>
        /// Inverse transform direction
        /// </summary>
        public static Vector3 InverseTransformDirection(this Matrix4x4 matrix, Vector3 direction)
        {
            return matrix.inverse.MultiplyVector(direction);
        }
    }
}
