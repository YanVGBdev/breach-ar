using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Rigidbody extension methods
    /// </summary>
    public static class RigidbodyExtensions
    {
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

        /// <summary>
        /// Stop movement
        /// </summary>
        public static void Stop(this Rigidbody rb)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// Stop linear velocity only
        /// </summary>
        public static void StopLinear(this Rigidbody rb)
        {
            rb.linearVelocity = Vector3.zero;
        }

        /// <summary>
        /// Stop angular velocity only
        /// </summary>
        public static void StopAngular(this Rigidbody rb)
        {
            rb.angularVelocity = Vector3.zero;
        }

        /// <summary>
        /// Check if moving
        /// </summary>
        public static bool IsMoving(this Rigidbody rb, float threshold = 0.1f)
        {
            return rb.linearVelocity.sqrMagnitude > threshold * threshold;
        }

        /// <summary>
        /// Get speed
        /// </summary>
        public static float GetSpeed(this Rigidbody rb)
        {
            return rb.linearVelocity.magnitude;
        }

        /// <summary>
        /// Set speed while maintaining direction
        /// </summary>
        public static void SetSpeed(this Rigidbody rb, float speed)
        {
            if (rb.linearVelocity.sqrMagnitude > 0.001f)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * speed;
            }
        }
    }
}
