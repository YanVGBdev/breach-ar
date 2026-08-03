namespace BreachAR.Utils
{
    /// <summary>
    /// Bool extension methods
    /// </summary>
    public static class BoolExtensions
    {
        /// <summary>
        /// Convert bool to int (true=1, false=0)
        /// </summary>
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        /// <summary>
        /// Convert bool to string ("true" or "false")
        /// </summary>
        public static string ToLowerString(this bool value)
        {
            return value.ToString().ToLower();
        }

        /// <summary>
        /// Convert bool to "Yes" or "No"
        /// </summary>
        public static string ToYesNo(this bool value)
        {
            return value ? "Yes" : "No";
        }

        /// <summary>
        /// Convert bool to "On" or "Off"
        /// </summary>
        public static string ToOnOff(this bool value)
        {
            return value ? "On" : "Off";
        }

        /// <summary>
        /// Convert bool to "Enabled" or "Disabled"
        /// </summary>
        public static string ToEnabledDisabled(this bool value)
        {
            return value ? "Enabled" : "Disabled";
        }

        /// <summary>
        /// Toggle bool
        /// </summary>
        public static bool Toggle(this bool value)
        {
            return !value;
        }
    }
}
