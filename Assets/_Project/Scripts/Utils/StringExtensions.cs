using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// String extension methods
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Check if string is null or empty
        /// </summary>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }

        /// <summary>
        /// Check if string is null or whitespace
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        /// <summary>
        /// Truncate string with ellipsis
        /// </summary>
        public static string Truncate(this string str, int maxLength)
        {
            if (string.IsNullOrEmpty(str)) return str;
            if (str.Length <= maxLength) return str;
            return str.Substring(0, maxLength - 3) + "...";
        }

        /// <summary>
        /// Convert to title case
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str.ToLower());
        }

        /// <summary>
        /// Check if string contains substring (case insensitive)
        /// </summary>
        public static bool ContainsIgnoreCase(this string str, string substring)
        {
            if (string.IsNullOrEmpty(str) || string.IsNullOrEmpty(substring)) return false;
            return str.IndexOf(substring, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Remove all spaces
        /// </summary>
        public static string RemoveSpaces(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str.Replace(" ", "");
        }

        /// <summary>
        /// Convert to slug
        /// </summary>
        public static string ToSlug(this string str)
        {
            if (string.IsNullOrEmpty(str)) return str;
            return str.ToLower()
                .Replace(" ", "-")
                .Replace("_", "-")
                .Replace(".", "-");
        }
    }
}
