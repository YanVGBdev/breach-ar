using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Camera extension methods
    /// </summary>
    public static class CameraExtensions
    {
        /// <summary>
        /// Get screen bounds
        /// </summary>
        public static Bounds GetScreenBounds(this Camera camera)
        {
            Vector3 bottomLeft = camera.ScreenToWorldPoint(new Vector3(0, 0, camera.nearClipPlane));
            Vector3 topRight = camera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, camera.nearClipPlane));
            Vector3 center = (bottomLeft + topRight) / 2f;
            Vector3 size = topRight - bottomLeft;
            return new Bounds(center, size);
        }

        /// <summary>
        /// Check if point is on screen
        /// </summary>
        public static bool IsPointOnScreen(this Camera camera, Vector3 worldPoint)
        {
            Vector3 screenPos = camera.WorldToScreenPoint(worldPoint);
            return screenPos.x > 0 && screenPos.x < Screen.width &&
                   screenPos.y > 0 && screenPos.y < Screen.height &&
                   screenPos.z > 0;
        }

        /// <summary>
        /// Get viewport position
        /// </summary>
        public static Vector2 GetViewportPosition(this Camera camera, Vector3 worldPoint)
        {
            Vector3 viewportPos = camera.WorldToViewportPoint(worldPoint);
            return new Vector2(viewportPos.x, viewportPos.y);
        }

        /// <summary>
        /// Check if point is in viewport
        /// </summary>
        public static bool IsPointInViewport(this Camera camera, Vector3 worldPoint)
        {
            Vector3 viewportPos = camera.WorldToViewportPoint(worldPoint);
            return viewportPos.x >= 0 && viewportPos.x <= 1 &&
                   viewportPos.y >= 0 && viewportPos.y <= 1 &&
                   viewportPos.z > 0;
        }

        /// <summary>
        /// Get direction from screen center to point
        /// </summary>
        public static Vector2 GetDirectionFromCenter(this Camera camera, Vector2 screenPosition)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            return (screenPosition - screenCenter).normalized;
        }

        /// <summary>
        /// Shake camera
        /// </summary>
        public static void Shake(this Camera camera, float duration, float magnitude)
        {
            // Would need coroutine for actual shake
            Debug.Log($"[Camera] Shake: {duration}s, {magnitude} magnitude");
        }
    }
}
