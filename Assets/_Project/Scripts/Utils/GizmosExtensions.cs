using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Gizmos extension methods
    /// </summary>
    public static class GizmosExtensions
    {
        /// <summary>
        /// Draw wire sphere
        /// </summary>
        public static void DrawWireSphere(Vector3 center, float radius, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(center, radius);
        }

        /// <summary>
        /// Draw wire cube
        /// </summary>
        public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(center, size);
        }

        /// <summary>
        /// Draw arrow
        /// </summary>
        public static void DrawArrow(Vector3 start, Vector3 direction, Color color, float arrowSize = 0.5f)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(start, direction);
            
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized * arrowSize;
            Vector3 up = Vector3.Cross(right, direction).normalized * arrowSize;
            
            Vector3 arrowTip = start + direction;
            Gizmos.DrawRay(arrowTip, -direction.normalized * arrowSize + right);
            Gizmos.DrawRay(arrowTip, -direction.normalized * arrowSize - right);
            Gizmos.DrawRay(arrowTip, -direction.normalized * arrowSize + up);
            Gizmos.DrawRay(arrowTip, -direction.normalized * arrowSize - up);
        }

        /// <summary>
        /// Draw bounds
        /// </summary>
        public static void DrawBounds(Bounds bounds, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        /// <summary>
        /// Draw line
        /// </summary>
        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(from, to);
        }

        /// <summary>
        /// Draw ray
        /// </summary>
        public static void DrawRay(Vector3 from, Vector3 direction, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawRay(from, direction);
        }

        /// <summary>
        /// Draw label
        /// </summary>
        public static void DrawLabel(Vector3 position, string text, Color color)
        {
            Gizmos.color = color;
            #if UNITY_EDITOR
            UnityEditor.Handles.Label(position, text);
            #endif
        }
    }
}
