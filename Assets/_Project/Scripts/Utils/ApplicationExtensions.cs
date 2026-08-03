using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Application extension methods
    /// </summary>
    public static class ApplicationExtensions
    {
        /// <summary>
        /// Open URL in browser
        /// </summary>
        public static void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        /// <summary>
        /// Quit application
        /// </summary>
        public static void QuitApplication()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }

        /// <summary>
        /// Get version string
        /// </summary>
        public static string GetVersion()
        {
            return Application.version;
        }

        /// <summary>
        /// Get product name
        /// </summary>
        public static string GetProductName()
        {
            return Application.productName;
        }

        /// <summary>
        /// Get company name
        /// </summary>
        public static string GetCompanyName()
        {
            return Application.companyName;
        }

        /// <summary>
        /// Check if running on mobile
        /// </summary>
        public static bool IsMobile()
        {
            return Application.isMobilePlatform;
        }

        /// <summary>
        /// Check if running in editor
        /// </summary>
        public static bool IsEditor()
        {
            #if UNITY_EDITOR
            return true;
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Get data path
        /// </summary>
        public static string GetDataPath()
        {
            return Application.dataPath;
        }

        /// <summary>
        /// Get persistent data path
        /// </summary>
        public static string GetPersistentDataPath()
        {
            return Application.persistentDataPath;
        }

        /// <summary>
        /// Get streaming assets path
        /// </summary>
        public static string GetStreamingAssetsPath()
        {
            return Application.streamingAssetsPath;
        }

        /// <summary>
        /// Get temporary cache path
        /// </summary>
        public static string GetTemporaryCachePath()
        {
            return Application.temporaryCachePath;
        }
    }
}
