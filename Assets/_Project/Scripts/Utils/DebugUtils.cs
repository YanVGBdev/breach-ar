using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Debug utility functions
    /// </summary>
    public static class DebugUtils
    {
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
        /// Draw debug grid
        /// </summary>
        public static void DrawGrid(Vector3 center, int size, float spacing, Color color, float duration = 0f)
        {
            for (int i = -size; i <= size; i++)
            {
                Vector3 start1 = center + new Vector3(i * spacing, 0, -size * spacing);
                Vector3 end1 = center + new Vector3(i * spacing, 0, size * spacing);
                Debug.DrawLine(start1, end1, color, duration);

                Vector3 start2 = center + new Vector3(-size * spacing, 0, i * spacing);
                Vector3 end2 = center + new Vector3(size * spacing, 0, i * spacing);
                Debug.DrawLine(start2, end2, color, duration);
            }
        }

        /// <summary>
        /// Log with color (editor only)
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
        /// Draw bounds
        /// </summary>
        public static void DrawBounds(Bounds bounds, Color color, float duration = 0f)
        {
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            // Bottom face
            Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(max.x, min.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, min.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(min.x, min.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, min.y, min.z), color, duration);

            // Top face
            Debug.DrawLine(new Vector3(min.x, max.y, min.z), new Vector3(max.x, max.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, max.y, min.z), new Vector3(max.x, max.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, max.y, max.z), new Vector3(min.x, max.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(min.x, max.y, max.z), new Vector3(min.x, max.y, min.z), color, duration);

            // Vertical edges
            Debug.DrawLine(new Vector3(min.x, min.y, min.z), new Vector3(min.x, max.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, min.y, min.z), new Vector3(max.x, max.y, min.z), color, duration);
            Debug.DrawLine(new Vector3(max.x, min.y, max.z), new Vector3(max.x, max.y, max.z), color, duration);
            Debug.DrawLine(new Vector3(min.x, min.y, max.z), new Vector3(min.x, max.y, max.z), color, duration);
        }
    }
}
