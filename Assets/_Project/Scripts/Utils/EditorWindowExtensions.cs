using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// EditorWindow extension methods
    /// </summary>
    public static class EditorWindowExtensions
    {
        /// <summary>
        /// Show window
        /// </summary>
        public static void ShowWindow<T>() where T : UnityEditor.EditorWindow
        {
            #if UNITY_EDITOR
            UnityEditor.EditorWindow.GetWindow<T>().Show();
            #endif
        }

        /// <summary>
        /// Show utility window
        /// </summary>
        public static void ShowUtility<T>() where T : UnityEditor.EditorWindow
        {
            #if UNITY_EDITOR
            var window = UnityEditor.EditorWindow.GetWindow<T>();
            window.ShowUtility();
            #endif
        }

        /// <summary>
        /// Show popup window
        /// </summary>
        public static void ShowPopup<T>() where T : UnityEditor.EditorWindow
        {
            #if UNITY_EDITOR
            var window = UnityEditor.EditorWindow.GetWindow<T>();
            window.ShowPopup();
            #endif
        }

        /// <summary>
        /// Close window
        /// </summary>
        public static void CloseWindow<T>() where T : UnityEditor.EditorWindow
        {
            #if UNITY_EDITOR
            var window = UnityEditor.EditorWindow.GetWindow<T>();
            if (window != null)
            {
                window.Close();
            }
            #endif
        }
    }
}
