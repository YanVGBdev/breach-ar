using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// RectTransform extension methods
    /// </summary>
    public static class RectTransformExtensions
    {
        /// <summary>
        /// Set anchored position
        /// </summary>
        public static void SetAnchoredPosition(this RectTransform rectTransform, Vector2 position)
        {
            rectTransform.anchoredPosition = position;
        }

        /// <summary>
        /// Set anchored position X
        /// </summary>
        public static void SetAnchoredPositionX(this RectTransform rectTransform, float x)
        {
            Vector2 position = rectTransform.anchoredPosition;
            position.x = x;
            rectTransform.anchoredPosition = position;
        }

        /// <summary>
        /// Set anchored position Y
        /// </summary>
        public static void SetAnchoredPositionY(this RectTransform rectTransform, float y)
        {
            Vector2 position = rectTransform.anchoredPosition;
            position.y = y;
            rectTransform.anchoredPosition = position;
        }

        /// <summary>
        /// Set size delta
        /// </summary>
        public static void SetSizeDelta(this RectTransform rectTransform, Vector2 size)
        {
            rectTransform.sizeDelta = size;
        }

        /// <summary>
        /// Set width
        /// </summary>
        public static void SetWidth(this RectTransform rectTransform, float width)
        {
            Vector2 size = rectTransform.sizeDelta;
            size.x = width;
            rectTransform.sizeDelta = size;
        }

        /// <summary>
        /// Set height
        /// </summary>
        public static void SetHeight(this RectTransform rectTransform, float height)
        {
            Vector2 size = rectTransform.sizeDelta;
            size.y = height;
            rectTransform.sizeDelta = size;
        }

        /// <summary>
        /// Get width
        /// </summary>
        public static float GetWidth(this RectTransform rectTransform)
        {
            return rectTransform.sizeDelta.x;
        }

        /// <summary>
        /// Get height
        /// </summary>
        public static float GetHeight(this RectTransform rectTransform)
        {
            return rectTransform.sizeDelta.y;
        }

        /// <summary>
        /// Center pivot
        /// </summary>
        public static void CenterPivot(this RectTransform rectTransform)
        {
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
        }

        /// <summary>
        /// Set pivot
        /// </summary>
        public static void SetPivot(this RectTransform rectTransform, Vector2 pivot)
        {
            rectTransform.pivot = pivot;
        }

        /// <summary>
        /// Get screen position
        /// </summary>
        public static Vector2 GetScreenPosition(this RectTransform rectTransform)
        {
            return RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
        }

        /// <summary>
        /// Is point in rect
        /// </summary>
        public static bool IsPointInRect(this RectTransform rectTransform, Vector2 screenPoint)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, screenPoint);
        }
    }
}
