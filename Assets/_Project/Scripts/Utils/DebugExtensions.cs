using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Debug extension methods
    /// </summary>
    public static class DebugExtensions
    {
        /// <summary>
        /// Log with color
        /// </summary>
        public static void LogColored(string message, Color color)
        {
            #if UNITY_EDITOR
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(color)}>{message}</color>");
            #else
            Debug.Log(message);
            #endif
        }

        /// <summary>
        /// Log warning with prefix
        /// </summary>
        public static void LogWarning(string message, Object context = null)
        {
            Debug.LogWarning($"[BreachAR] {message}", context);
        }

        /// <summary>
        /// Log error with prefix
        /// </summary>
        public static void LogError(string message, Object context = null)
        {
            Debug.LogError($"[BreachAR] {message}", context);
        }

        /// <summary>
        /// Draw debug sphere
        /// </summary>
        public static void DrawSphere(Vector3 position, float radius, Color color, float duration = 0f)
        {
            Debug.DrawRay(position, Vector3.up * radius, color, duration);
            Debug.DrawRay(position, Vector3.down * radius, color, duration);
            Debug.DrawRay(position, Vector3.left * radius, color, duration);
            Debug.DrawRay(position, Vector3.right * radius, color, duration);
            Debug.DrawRay(position, Vector3.forward * radius, color, duration);
            Debug.DrawRay(position, Vector3.back * radius, color, duration);
        }

        /// <summary>
        /// Draw debug arrow
        /// </summary>
        public static void DrawArrow(Vector3 start, Vector3 direction, Color color, float duration = 0f, float arrowSize = 0.5f)
        {
            Debug.DrawRay(start, direction, color, duration);
            
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized * arrowSize;
            Vector3 up = Vector3.Cross(right, direction).normalized * arrowSize;
            
            Vector3 arrowTip = start + direction;
            Debug.DrawRay(arrowTip, -direction.normalized * arrowSize + right, color, duration);
            Debug.DrawRay(arrowTip, -direction.normalized * arrowSize - right, color, duration);
            Debug.DrawRay(arrowTip, -direction.normalized * arrowSize + up, color, duration);
            Debug.DrawRay(arrowTip, -direction.normalized * arrowSize - up, color, duration);
        }

        /// <summary>
        /// Draw bounds
        /// </summary>
        public static void DrawBounds(Bounds bounds, Color color, float duration = 0f)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(min.x, min.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, min.y, min.z), color, duration);

            Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, max.y, max.z), new Vector3(min.x, max.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(min.x, max.y, max.z), new Vector3(min.x, max.y, min.z), color, duration);

            Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, max.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, max.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, max.y, max.z), color, duration);
        }
    }
}
