using UnityEngine;
using UnityEngine.UI;

namespace BreachAR.Utils
{
    /// <summary>
    /// Toggle extension methods
    /// </summary>
    public static class ToggleExtensions
    {
        /// <summary>
        /// Set isOn safely
        /// </summary>
        public static void SetIsOnSafe(this Toggle toggle, bool isOn)
        {
            if (toggle != null)
            {
                toggle.isOn = isOn;
            }
        }

        /// <summary>
        /// Set isOn without notifying
        /// </summary>
        public static void SetIsOnWithoutNotify(this Toggle toggle, bool isOn)
        {
            if (toggle != null)
            {
                toggle.SetIsOnWithoutNotify(isOn);
            }
        }

        /// <summary>
        /// Set enabled
        /// </summary>
        public static void SetEnabled(this Toggle toggle, bool enabled)
        {
            if (toggle != null)
            {
                toggle.interactable = enabled;
            }
        }

        /// <summary>
        /// Set color tint
        /// </summary>
        public static void SetColor(this Toggle toggle, Color color)
        {
            if (toggle != null)
            {
                ColorBlock colors = toggle.colors;
                colors.normalColor = color;
                toggle.colors = colors;
            }
        }

        /// <summary>
        /// Toggle value
        /// </summary>
        public static void ToggleValue(this Toggle toggle)
        {
            if (toggle != null)
            {
                toggle.isOn = !toggle.isOn;
            }
        }
    }
}
