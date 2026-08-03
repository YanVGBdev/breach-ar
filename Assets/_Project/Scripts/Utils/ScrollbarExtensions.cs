using UnityEngine;
using UnityEngine.UI;

namespace BreachAR.Utils
{
    /// <summary>
    /// Scrollbar extension methods
    /// </summary>
    public static class ScrollbarExtensions
    {
        /// <summary>
        /// Set value safely
        /// </summary>
        public static void SetValueSafe(this Scrollbar scrollbar, float value)
        {
            if (scrollbar != null)
            {
                scrollbar.value = Mathf.Clamp01(value);
            }
        }

        /// <summary>
        /// Set size safely
        /// </summary>
        public static void SetSizeSafe(this Scrollbar scrollbar, float size)
        {
            if (scrollbar != null)
            {
                scrollbar.size = Mathf.Clamp01(size);
            }
        }

        /// <summary>
        /// Set enabled
        /// </summary>
        public static void SetEnabled(this Scrollbar scrollbar, bool enabled)
        {
            if (scrollbar != null)
            {
                scrollbar.interactable = enabled;
            }
        }
    }
}
