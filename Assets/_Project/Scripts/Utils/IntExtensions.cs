using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Int extension methods
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Clamp int
        /// </summary>
        public static int Clamp(this int value, int min, int max)
        {
            return Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// Check if even
        /// </summary>
        public static bool IsEven(this int value)
        {
            return value % 2 == 0;
        }

        /// <summary>
        /// Check if odd
        /// </summary>
        public static bool IsOdd(this int value)
        {
            return value % 2 != 0;
        }

        /// <summary>
        /// Check if prime
        /// </summary>
        public static bool IsPrime(this int value)
        {
            if (value <= 1) return false;
            if (value <= 3) return true;
            if (value % 2 == 0 || value % 3 == 0) return false;

            for (int i = 5; i * i <= value; i += 6)
            {
                if (value % i == 0 || value % (i + 2) == 0)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Format as string with commas
        /// </summary>
        public static string FormatNumber(this int value)
        {
            return value.ToString("N0");
        }

        /// <summary>
        /// Format as compact string (1K, 1M)
        /// </summary>
        public static string FormatCompact(this int value)
        {
            if (value >= 1000000)
                return $"{value / 1000000f:F1}M";
            if (value >= 1000)
                return $"{value / 1000f:F1}K";
            return value.ToString();
        }

        /// <summary>
        /// Get sign (-1, 0, or 1)
        /// </summary>
        public static int Sign(this int value)
        {
            if (value > 0) return 1;
            if (value < 0) return -1;
            return 0;
        }

        /// <summary>
        /// Get absolute value
        /// </summary>
        public static int Abs(this int value)
        {
            return Mathf.Abs(value);
        }
    }
}
