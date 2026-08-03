using UnityEngine;
using UnityEngine.UI;

namespace BreachAR.Utils
{
    /// <summary>
    /// Slider extension methods
    /// </summary>
    public static class SliderExtensions
    {
        /// <summary>
        /// Set value safely
        /// </summary>
        public static void SetValueSafe(this Slider slider, float value)
        {
            if (slider != null)
            {
                slider.value = Mathf.Clamp(value, slider.minValue, slider.maxValue);
            }
        }

        /// <summary>
        /// Set value without notifying
        /// </summary>
        public static void SetValueWithoutNotify(this Slider slider, float value)
        {
            if (slider != null)
            {
                slider.SetValueWithoutNotify(Mathf.Clamp(value, slider.minValue, slider.maxValue));
            }
        }

        /// <summary>
        /// Set range
        /// </summary>
        public static void SetRange(this Slider slider, float min, float max)
        {
            if (slider != null)
            {
                slider.minValue = min;
                slider.maxValue = max;
            }
        }

        /// <summary>
        /// Get normalized value
        /// </summary>
        public static float GetNormalized(this Slider slider)
        {
            if (slider == null) return 0f;
            return slider.normalizedValue;
        }

        /// <summary>
        /// Set normalized value
        /// </summary>
        public static void SetNormalized(this Slider slider, float value)
        {
            if (slider != null)
            {
                slider.normalizedValue = Mathf.Clamp01(value);
            }
        }

        /// <summary>
        /// Set enabled
        /// </summary>
        public static void SetEnabled(this Slider slider, bool enabled)
        {
            if (slider != null)
            {
                slider.interactable = enabled;
            }
        }
    }
}
