using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Input extension methods
    /// </summary>
    public static class InputExtensions
    {
        /// <summary>
        /// Get touch or mouse position
        /// </summary>
        public static Vector2 GetInputPosition()
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }
            return Input.mousePosition;
        }

        /// <summary>
        /// Get touch or mouse delta
        /// </summary>
        public static Vector2 GetInputDelta()
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).deltaPosition;
            }
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        /// <summary>
        /// Check if input is down
        /// </summary>
        public static bool IsInputDown()
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).phase == TouchPhase.Began;
            }
            return Input.GetMouseButtonDown(0);
        }

        /// <summary>
        /// Check if input is pressed
        /// </summary>
        public static bool IsInputPressed()
        {
            if (Input.touchCount > 0)
            {
                TouchPhase phase = Input.GetTouch(0).phase;
                return phase == TouchPhase.Moved || phase == TouchPhase.Stationary;
            }
            return Input.GetMouseButton(0);
        }

        /// <summary>
        /// Check if input is up
        /// </summary>
        public static bool IsInputUp()
        {
            if (Input.touchCount > 0)
            {
                TouchPhase phase = Input.GetTouch(0).phase;
                return phase == TouchPhase.Ended || phase == TouchPhase.Canceled;
            }
            return Input.GetMouseButtonUp(0);
        }

        /// <summary>
        /// Get input duration
        /// </summary>
        public static float GetInputDuration(float startTime)
        {
            return Time.time - startTime;
        }

        /// <summary>
        /// Check if long press
        /// </summary>
        public static bool IsLongPress(float startTime, float threshold = 0.5f)
        {
            return GetInputDuration(startTime) >= threshold;
        }
    }
}
