using UnityEngine;
using UnityEngine.UI;

namespace BreachAR.Utils
{
    /// <summary>
    /// Button extension methods
    /// </summary>
    public static class ButtonExtensions
    {
        /// <summary>
        /// Set interactable safely
        /// </summary>
        public static void SetInteractableSafe(this Button button, bool interactable)
        {
            if (button != null)
            {
                button.interactable = interactable;
            }
        }

        /// <summary>
        /// Set enabled
        /// </summary>
        public static void SetEnabled(this Button button, bool enabled)
        {
            if (button != null)
            {
                button.interactable = enabled;
            }
        }

        /// <summary>
        /// Set color tint
        /// </summary>
        public static void SetColor(this Button button, Color color)
        {
            if (button != null)
            {
                ColorBlock colors = button.colors;
                colors.normalColor = color;
                button.colors = colors;
            }
        }

        /// <summary>
        /// Set highlight color
        /// </summary>
        public static void SetHighlightColor(this Button button, Color color)
        {
            if (button != null)
            {
                ColorBlock colors = button.colors;
                colors.highlightedColor = color;
                button.colors = colors;
            }
        }

        /// <summary>
        /// Set pressed color
        /// </summary>
        public static void SetPressedColor(this Button button, Color color)
        {
            if (button != null)
            {
                ColorBlock colors = button.colors;
                colors.pressedColor = color;
                button.colors = colors;
            }
        }

        /// <summary>
        /// Set selected color
        /// </summary>
        public static void SetSelectedColor(this Button button, Color color)
        {
            if (button != null)
            {
                ColorBlock colors = button.colors;
                colors.selectedColor = color;
                button.colors = colors;
            }
        }
    }
}
