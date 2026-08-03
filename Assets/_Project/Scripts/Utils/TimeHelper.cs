using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Time helper utility functions
    /// </summary>
    public static class TimeHelper
    {
        /// <summary>
        /// Format time as MM:SS
        /// </summary>
        public static string FormatMMSS(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            return $"{minutes:00}:{secs:00}";
        }

        /// <summary>
        /// Format time as MM:SS.ms
        /// </summary>
        public static string FormatMMSSMS(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            int ms = Mathf.FloorToInt((seconds * 100f) % 100f);
            return $"{minutes:00}:{secs:00}.{ms:00}";
        }

        /// <summary>
        /// Format time as HH:MM:SS
        /// </summary>
        public static string FormatHHMMSS(float seconds)
        {
            int hours = Mathf.FloorToInt(seconds / 3600f);
            int minutes = Mathf.FloorToInt((seconds % 3600f) / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            return $"{hours:00}:{minutes:00}:{secs:00}";
        }

        /// <summary>
        /// Format time as compact string
        /// </summary>
        public static string FormatCompact(float seconds)
        {
            if (seconds < 60f)
                return $"{Mathf.FloorToInt(seconds)}s";
            if (seconds < 3600f)
                return $"{Mathf.FloorToInt(seconds / 60f)}m {Mathf.FloorToInt(seconds % 60f)}s";
            return $"{Mathf.FloorToInt(seconds / 3600f)}h {Mathf.FloorToInt((seconds % 3600f) / 60f)}m";
        }

        /// <summary>
        /// Get time of day string
        /// </summary>
        public static string GetTimeOfDay()
        {
            int hour = System.DateTime.Now.Hour;
            if (hour < 6) return "Night";
            if (hour < 12) return "Morning";
            if (hour < 18) return "Afternoon";
            return "Evening";
        }

        /// <summary>
        /// Check if time has passed
        /// </summary>
        public static bool HasTimePassed(float startTime, float duration)
        {
            return Time.time - startTime >= duration;
        }

        /// <summary>
        /// Get normalized time (0-1)
        /// </summary>
        public static float GetNormalizedTime(float startTime, float duration)
        {
            return Mathf.Clamp01((Time.time - startTime) / duration);
        }

        /// <summary>
        /// Check if it's a new day
        /// </summary>
        public static bool IsNewDay()
        {
            string lastPlayDate = PlayerPrefs.GetString("LastPlayDate", "");
            string today = System.DateTime.Now.ToString("yyyy-MM-dd");
            return lastPlayDate != today;
        }

        /// <summary>
        /// Mark today as played
        /// </summary>
        public static void MarkDayPlayed()
        {
            PlayerPrefs.SetString("LastPlayDate", System.DateTime.Now.ToString("yyyy-MM-dd"));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Get days since date
        /// </summary>
        public static int GetDaysSince(string dateString)
        {
            if (System.DateTime.TryParse(dateString, out System.DateTime date))
            {
                return (System.DateTime.Now - date).Days;
            }
            return 0;
        }
    }
}
