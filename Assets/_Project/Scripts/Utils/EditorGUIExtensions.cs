using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// EditorGUI extension methods
    /// </summary>
    public static class EditorGUIExtensions
    {
        /// <summary>
        /// Draw separator
        /// </summary>
        public static void DrawSeparator()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.LabelField("", UnityEditor.EditorStyles.boldLabel);
            #endif
        }

        /// <summary>
        /// Draw header
        /// </summary>
        public static void DrawHeader(string title)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.LabelField(title, UnityEditor.EditorStyles.boldLabel);
            #endif
        }

        /// <summary>
        /// Draw foldout
        /// </summary>
        public static bool DrawFoldout(bool foldout, string title)
        {
            #if UNITY_EDITOR
            return UnityEditor.EditorGUILayout.Foldout(foldout, title, true, UnityEditor.EditorStyles.foldoutHeader);
            #else
            return foldout;
            #endif
        }

        /// <summary>
        /// Draw help box
        /// </summary>
        public static void DrawHelpBox(string message, UnityEditor.MessageType type)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorGUILayout.HelpBox(message, type);
            #endif
        }

        /// <summary>
        /// Draw progress bar
        /// </summary>
        public static void DrawProgressBar(string label, float value)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorGUILayout.LabelField(label);
            Rect rect = UnityEditor.EditorGUILayout.GetControlRect(false, 20f);
            UnityEditor.EditorGUI.ProgressBar(rect, value, $"{(value * 100f):F1}%");
            #endif
        }
    }
}
