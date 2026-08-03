using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Platform helper for cross-platform utilities
    /// </summary>
    public static class PlatformHelper
    {
        /// <summary>
        /// Check if running on Android
        /// </summary>
        public static bool IsAndroid()
        {
            #if UNITY_ANDROID
            return true;
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Check if running on iOS
        /// </summary>
        public static bool IsIOS()
        {
            #if UNITY_IOS
            return true;
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Check if running on mobile
        /// </summary>
        public static bool IsMobile()
        {
            return Application.isMobilePlatform || IsAndroid() || IsIOS();
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
        /// Check if running on standalone
        /// </summary>
        public static bool IsStandalone()
        {
            #if UNITY_STANDALONE
            return true;
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Get device model
        /// </summary>
        public static string GetDeviceModel()
        {
            return SystemInfo.deviceModel;
        }

        /// <summary>
        /// Get device name
        /// </summary>
        public static string GetDeviceName()
        {
            return SystemInfo.deviceName;
        }

        /// <summary>
        /// Get operating system
        /// </summary>
        public static string GetOperatingSystem()
        {
            return SystemInfo.operatingSystem;
        }

        /// <summary>
        /// Get processor type
        /// </summary>
        public static string GetProcessorType()
        {
            return SystemInfo.processorType;
        }

        /// <summary>
        /// Get system memory size
        /// </summary>
        public static int GetSystemMemoryMB()
        {
            return SystemInfo.systemMemorySize;
        }

        /// <summary>
        /// Get graphics memory size
        /// </summary>
        public static int GetGraphicsMemoryMB()
        {
            return SystemInfo.graphicsMemorySize;
        }

        /// <summary>
        /// Get graphics device name
        /// </summary>
        public static string GetGraphicsDeviceName()
        {
            return SystemInfo.graphicsDeviceName;
        }

        /// <summary>
        /// Check if gyroscope is supported
        /// </summary>
        public static bool HasGyroscope()
        {
            return SystemInfo.supportsGyroscope;
        }

        /// <summary>
        /// Check if AR is supported
        /// </summary>
        public static bool HasARSupport()
        {
            // TODO: Check ARCore/ARKit support
            return false;
        }

        /// <summary>
        /// Get battery level
        /// </summary>
        public static float GetBatteryLevel()
        {
            return SystemInfo.batteryLevel;
        }

        /// <summary>
        /// Get battery status
        /// </summary>
        public static BatteryStatus GetBatteryStatus()
        {
            return SystemInfo.batteryStatus;
        }

        /// <summary>
        /// Check if device is charging
        /// </summary>
        public static bool IsCharging()
        {
            return SystemInfo.batteryStatus == BatteryStatus.Charging;
        }

        /// <summary>
        /// Open URL in browser
        /// </summary>
        public static void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        /// <summary>
        /// Share text
        /// </summary>
        public static void ShareText(string text)
        {
            #if UNITY_ANDROID
            using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
            using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent"))
            {
                intent.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
                intent.Call<AndroidJavaObject>("setType", "text/plain");
                intent.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), text);
                
                using (AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                using (AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    currentActivity.Call("startActivity", intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, "Share via"));
                }
            }
            #elif UNITY_IOS
            // iOS sharing would use native plugin
            Debug.Log("[Platform] Share not implemented for iOS");
            #endif
        }
    }
}
