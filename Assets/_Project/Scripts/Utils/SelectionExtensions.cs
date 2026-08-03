using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Selection extension methods
    /// </summary>
    public static class SelectionExtensions
    {
        /// <summary>
        /// Select game object
        /// </summary>
        public static void Select(this GameObject gameObject)
        {
            #if UNITY_EDITOR
            UnityEditor.Selection.activeGameObject = gameObject;
            #endif
        }

        /// <summary>
        /// Select transform
        /// </summary>
        public static void Select(this Transform transform)
        {
            #if UNITY_EDITOR
            UnityEditor.Selection.activeTransform = transform;
            #endif
        }

        /// <summary>
        /// Select object
        /// </summary>
        public static void Select(this Object obj)
        {
            #if UNITY_EDITOR
            UnityEditor.Selection.activeObject = obj;
            #endif
        }

        /// <summary>
        /// Ping object
        /// </summary>
        public static void Ping(this Object obj)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorGUIUtility.PingObject(obj);
            #endif
        }

        /// <summary>
        /// Frame selected
        /// </summary>
        public static void FrameSelected()
        {
            #if UNITY_EDITOR
            UnityEditor.SceneView.lastActiveSceneView.FrameSelected();
            #endif
        }
    }
}
