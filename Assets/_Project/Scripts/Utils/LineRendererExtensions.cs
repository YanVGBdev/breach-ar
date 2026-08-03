using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// LineRenderer extension methods
    /// </summary>
    public static class LineRendererExtensions
    {
        /// <summary>
        /// Set points
        /// </summary>
        public static void SetPoints(this LineRenderer lineRenderer, Vector3[] points)
        {
            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);
        }

        /// <summary>
        /// Add point
        /// </summary>
        public static void AddPoint(this LineRenderer lineRenderer, Vector3 point)
        {
            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);
        }

        /// <summary>
        /// Clear points
        /// </summary>
        public static void ClearPoints(this LineRenderer lineRenderer)
        {
            lineRenderer.positionCount = 0;
        }

        /// <summary>
        /// Set color
        /// </summary>
        public static void SetColor(this LineRenderer lineRenderer, Color color)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }

        /// <summary>
        /// Set width
        /// </summary>
        public static void SetWidth(this LineRenderer lineRenderer, float width)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }

        /// <summary>
        /// Set gradient color
        /// </summary>
        public static void SetGradient(this LineRenderer lineRenderer, Color startColor, Color endColor)
        {
            lineRenderer.startColor = startColor;
            lineRenderer.endColor = endColor;
        }

        /// <summary>
        /// Show line
        /// </summary>
        public static void Show(this LineRenderer lineRenderer)
        {
            lineRenderer.enabled = true;
        }

        /// <summary>
        /// Hide line
        /// </summary>
        public static void Hide(this LineRenderer lineRenderer)
        {
            lineRenderer.enabled = false;
        }
    }
}
