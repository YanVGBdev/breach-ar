using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// String utility functions
    /// </summary>
    public static class StringUtils
    {
        /// <summary>
        /// Format number with commas (1000 -> 1,000)
        /// </summary>
        public static string FormatNumber(int number)
        {
            return number.ToString("N0");
        }

        /// <summary>
        /// Format number with commas (1000.5 -> 1,000.5)
        /// </summary>
        public static string FormatNumber(float number, int decimals = 0)
        {
            return number.ToString($"N{decimals}");
        }

        /// <summary>
        /// Format large numbers (1000 -> 1K, 1000000 -> 1M)
        /// </summary>
        public static string FormatCompact(int number)
        {
            if (number >= 1000000)
                return $"{number / 1000000f:F1}M";
            if (number >= 1000)
                return $"{number / 1000f:F1}K";
            return number.ToString();
        }

        /// <summary>
        /// Format time (seconds -> MM:SS)
        /// </summary>
        public static string FormatTime(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            return $"{minutes:00}:{secs:00}";
        }

        /// <summary>
        /// Format time (seconds -> MM:SS.ms)
        /// </summary>
        public static string FormatTimePrecise(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            int ms = Mathf.FloorToInt((seconds * 100f) % 100f);
            return $"{minutes:00}:{secs:00}.{ms:00}";
        }

        /// <summary>
        /// Format time (seconds -> HH:MM:SS)
        /// </summary>
        public static string FormatTimeHours(float seconds)
        {
            int hours = Mathf.FloorToInt(seconds / 3600f);
            int minutes = Mathf.FloorToInt((seconds % 3600f) / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            return $"{hours:00}:{minutes:00}:{secs:00}";
        }

        /// <summary>
        /// Format percentage (0.75 -> 75%)
        /// </summary>
        public static string FormatPercent(float value, int decimals = 0)
        {
            return $"{(value * 100f).ToString($"F{decimals}")}%";
        }

        /// <summary>
        /// Format multiplier (1.5 -> x1.5)
        /// </summary>
        public static string FormatMultiplier(float multiplier)
        {
            return $"x{multiplier:F1}";
        }

        /// <summary>
        /// Truncate string with ellipsis
        /// </summary>
        public static string Truncate(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.Length <= maxLength) return text;
            return text.Substring(0, maxLength - 3) + "...";
        }

        /// <summary>
        /// Convert to title case
        /// </summary>
        public static string ToTitleCase(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(text.ToLower());
        }

        /// <summary>
        /// Generate random string
        /// </summary>
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = chars[Random.Range(0, chars.Length)];
            }
            return new string(result);
        }
    }
}
