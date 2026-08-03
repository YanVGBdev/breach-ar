using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Undo extension methods
    /// </summary>
    public static class UndoExtensions
    {
        /// <summary>
        /// Record object
        /// </summary>
        public static void Record(this Object obj, string name)
        {
            #if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(obj, name);
            #endif
        }

        /// <summary>
        /// Register created object
        /// </summary>
        public static void RegisterCreated(this Object obj, string name)
        {
            #if UNITY_EDITOR
            UnityEditor.Undo.RegisterCreatedObjectUndo(obj, name);
            #endif
        }

        /// <summary>
        /// Destroy object
        /// </summary>
        public static void DestroyUndo(this Object obj)
        {
            #if UNITY_EDITOR
            UnityEditor.Undo.DestroyObjectImmediate(obj);
            #else
            Object.Destroy(obj);
            #endif
        }

        /// <summary>
        /// Undo
        /// </summary>
        public static void Undo()
        {
            #if UNITY_EDITOR
            UnityEditor.Undo.PerformUndo();
            #endif
        }

        /// <summary>
        /// Redo
        /// </summary>
        public static void Redo()
        {
            #if UNITY_EDITOR
            UnityEditor.Undo.PerformRedo();
            #endif
        }
    }
}
