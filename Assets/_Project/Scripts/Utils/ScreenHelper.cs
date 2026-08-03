using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Screen helper utility functions
    /// </summary>
    public static class ScreenHelper
    {
        /// <summary>
        /// Check if point is on screen
        /// </summary>
        public static bool IsOnScreen(Vector3 worldPosition)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            return screenPos.x > 0 && screenPos.x < Screen.width &&
                   screenPos.y > 0 && screenPos.y < Screen.height &&
                   screenPos.z > 0;
        }

        /// <summary>
        /// Get screen position of world point
        /// </summary>
        public static Vector2 GetScreenPosition(Vector3 worldPosition)
        {
            return Camera.main.WorldToScreenPoint(worldPosition);
        }

        /// <summary>
        /// Get world position from screen point
        /// </summary>
        public static Vector3 GetWorldPosition(Vector2 screenPosition, float distance = 10f)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, distance));
            return worldPos;
        }

        /// <summary>
        /// Get direction from screen center to point
        /// </summary>
        public static Vector2 GetDirectionFromCenter(Vector2 screenPosition)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            return (screenPosition - screenCenter).normalized;
        }

        /// <summary>
        /// Get screen edge position
        /// </summary>
        public static Vector2 GetScreenEdgePosition(Vector2 direction, float margin = 50f)
        {
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            return screenCenter + direction.normalized * Mathf.Min(Screen.width, Screen.height) / 2f - direction.normalized * margin;
        }

        /// <summary>
        /// Clamp position to screen
        /// </summary>
        public static Vector2 ClampToScreen(Vector2 position, float margin = 0f)
        {
            position.x = Mathf.Clamp(position.x, margin, Screen.width - margin);
            position.y = Mathf.Clamp(position.y, margin, Screen.height - margin);
            return position;
        }

        /// <summary>
        /// Get screen aspect ratio
        /// </summary>
        public static float GetAspectRatio()
        {
            return (float)Screen.width / Screen.height;
        }

        /// <summary>
        /// Check if screen is landscape
        /// </summary>
        public static bool IsLandscape()
        {
            return Screen.width > Screen.height;
        }

        /// <summary>
        /// Check if screen is portrait
        /// </summary>
        public static bool IsPortrait()
        {
            return Screen.height > Screen.width;
        }

        /// <summary>
        /// Get DPI
        /// </summary>
        public static float GetDPI()
        {
            return Screen.dpi;
        }

        /// <summary>
        /// Set fullscreen mode
        /// </summary>
        public static void SetFullscreen(bool fullscreen)
        {
            Screen.fullScreen = fullscreen;
        }

        /// <summary>
        /// Set resolution
        /// </summary>
        public static void SetResolution(int width, int height, bool fullscreen)
        {
            Screen.SetResolution(width, height, fullscreen);
        }
    }
}
