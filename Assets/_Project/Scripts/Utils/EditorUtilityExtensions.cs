using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// EditorUtility extension methods
    /// </summary>
    public static class EditorUtilityExtensions
    {
        /// <summary>
        /// Display dialog
        /// </summary>
        public static bool DisplayDialog(string title, string message, string ok, string cancel = "")
        {
            #if UNITY_EDITOR
            return UnityEditor.EditorUtility.DisplayDialog(title, message, ok, cancel);
            #else
            return true;
            #endif
        }

        /// <summary>
        /// Display complex dialog
        /// </summary>
        public static int DisplayComplexDialog(string title, string message, string[] buttons)
        {
            #if UNITY_EDITOR
            return UnityEditor.EditorUtility.DisplayDialogComplex(title, message, buttons[0], buttons[1], buttons[2]);
            #else
            return 0;
            #endif
        }

        /// <summary>
        /// Display progress bar
        /// </summary>
        public static void DisplayProgressBar(string title, string info, float progress)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.DisplayProgressBar(title, info, progress);
            #endif
        }

        /// <summary>
        /// Clear progress bar
        /// </summary>
        public static void ClearProgressBar()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.ClearProgressBar();
            #endif
        }

        /// <summary>
        /// Display success dialog
        /// </summary>
        public static void DisplaySuccess(string message)
        {
            DisplayDialog("Success", message, "OK");
        }

        /// <summary>
        /// Display error dialog
        /// </summary>
        public static void DisplayError(string message)
        {
            DisplayDialog("Error", message, "OK");
        }

        /// <summary>
        /// Display warning dialog
        /// </summary>
        public static void DisplayWarning(string message)
        {
            DisplayDialog("Warning", message, "OK");
        }
    }
}
