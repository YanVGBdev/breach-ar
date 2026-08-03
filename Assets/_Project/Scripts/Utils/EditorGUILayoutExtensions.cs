using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// EditorGUILayout extension methods
    /// </summary>
    public static class EditorGUILayoutExtensions
    {
        /// <summary>
        /// Draw title
        /// </summary>
        public static void DrawTitle(string title)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.LabelField(title, UnityEditor.EditorStyles.boldLabel);
            #endif
        }

        /// <summary>
        /// Draw subtitle
        /// </summary>
        public static void DrawSubtitle(string subtitle)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorGUILayout.Space();
            UnityEditor.EditorGUILayout.LabelField(subtitle, UnityEditor.EditorStyles.largeLabel);
            #endif
        }

        /// <summary>
        /// Draw horizontal line
        /// </summary>
        public static void DrawHorizontalLine()
        {
            #if UNITY_EDITOR
            Rect rect = UnityEditor.EditorGUILayout.GetControlRect(false, 1f);
            rect.height = 1f;
            UnityEditor.EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1f));
            #endif
        }

        /// <summary>
        /// Draw property field
        /// </summary>
        public static void DrawPropertyField(UnityEditor.SerializedProperty property, params GUILayoutOption[] options)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorGUILayout.PropertyField(property, options);
            #endif
        }

        /// <summary>
        /// Draw property field with label
        /// </summary>
        public static void DrawPropertyField(UnityEditor.SerializedProperty property, GUIContent label, params GUILayoutOption[] options)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorGUILayout.PropertyField(property, label, options);
            #endif
        }
    }
}
