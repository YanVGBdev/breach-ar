using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// CustomEditor extension methods
    /// </summary>
    public static class CustomEditorExtensions
    {
        /// <summary>
        /// Get target
        /// </summary>
        public static T GetTarget<T>(this UnityEditor.Editor editor) where T : Object
        {
            return (T)editor.target;
        }

        /// <summary>
        /// Get targets
        /// </summary>
        public static T[] GetTargets<T>(this UnityEditor.Editor editor) where T : Object
        {
            T[] targets = new T[editor.targets.Length];
            for (int i = 0; i < editor.targets.Length; i++)
            {
                targets[i] = (T)editor.targets[i];
            }
            return targets;
        }

        /// <summary>
        /// Draw default inspector
        /// </summary>
        public static void DrawDefaultInspector(this UnityEditor.Editor editor)
        {
            editor.DrawDefaultInspector();
        }

        /// <summary>
        /// Apply modified properties
        /// </summary>
        public static void ApplyModifiedProperties(this UnityEditor.Editor editor, bool undoRedo = true)
        {
            editor.serializedObject.ApplyModifiedProperties();
        }
    }
}
