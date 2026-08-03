using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Handles extension methods
    /// </summary>
    public static class HandlesExtensions
    {
        /// <summary>
        /// Draw label
        /// </summary>
        public static void DrawLabel(Vector3 position, string text, Color color)
        {
            #if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.Label(position, text);
            #endif
        }

        /// <summary>
        /// Draw wire sphere
        /// </summary>
        public static void DrawWireSphere(Vector3 center, float radius, Color color)
        {
            #if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawWireDisc(center, Vector3.up, radius);
            UnityEditor.Handles.DrawWireDisc(center, Vector3.forward, radius);
            UnityEditor.Handles.DrawWireDisc(center, Vector3.right, radius);
            #endif
        }

        /// <summary>
        /// Draw wire cube
        /// </summary>
        public static void DrawWireCube(Vector3 center, Vector3 size, Color color)
        {
            #if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawWireCube(center, size);
            #endif
        }

        /// <summary>
        /// Draw line
        /// </summary>
        public static void DrawLine(Vector3 from, Vector3 to, Color color)
        {
            #if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawLine(from, to);
            #endif
        }

        /// <summary>
        /// Draw arc
        /// </summary>
        public static void DrawArc(Vector3 center, Vector3 normal, Vector3 from, float angle, Color color)
        {
            #if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawArc(center, normal, from, angle, 1f);
            #endif
        }

        /// <summary>
        /// Draw solid disc
        /// </summary>
        public static void DrawSolidDisc(Vector3 center, Vector3 normal, float radius, Color color)
        {
            #if UNITY_EDITOR
            UnityEditor.Handles.color = color;
            UnityEditor.Handles.DrawSolidDisc(center, normal, radius);
            #endif
        }
    }
}
