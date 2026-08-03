using UnityEngine;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Shows trajectory preview line for orb launch
    /// </summary>
    [RequireComponent(typeof(LineRenderer))]
    public class TrajectoryPreview : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int pointCount = 30;
        [SerializeField] private float timeStep = 0.05f;
        [SerializeField] private float maxTime = 2f;
        [SerializeField] private LayerMask collisionMask;

        [Header("Visual")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color dangerColor = Color.red;
        [SerializeField] private float normalWidth = 0.05f;
        [SerializeField] private float warningWidth = 0.08f;

        private LineRenderer lineRenderer;
        private bool isVisible;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 0;
        }

        /// <summary>
        /// Show trajectory preview
        /// </summary>
        public void Show(Vector3 startPosition, Vector3 initialVelocity)
        {
            isVisible = true;
            gameObject.SetActive(true);
            UpdateTrajectory(startPosition, initialVelocity);
        }

        /// <summary>
        /// Hide trajectory preview
        /// </summary>
        public void Hide()
        {
            isVisible = false;
            lineRenderer.positionCount = 0;
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Update trajectory visualization
        /// </summary>
        public void UpdateTrajectory(Vector3 startPosition, Vector3 initialVelocity)
        {
            if (!isVisible) return;

            Vector3[] points = CalculateTrajectory(startPosition, initialVelocity);
            
            if (points == null || points.Length == 0)
            {
                lineRenderer.positionCount = 0;
                return;
            }

            lineRenderer.positionCount = points.Length;
            lineRenderer.SetPositions(points);

            // Update color based on predicted hits
            UpdateColor(points);
        }

        /// <summary>
        /// Calculate trajectory points
        /// </summary>
        private Vector3[] CalculateTrajectory(Vector3 startPosition, Vector3 initialVelocity)
        {
            var points = new System.Collections.Generic.List<Vector3>();
            Vector3 position = startPosition;
            Vector3 velocity = initialVelocity;
            float time = 0f;

            points.Add(position);

            while (time < maxTime && points.Count < pointCount)
            {
                // Apply gravity
                velocity += Physics.gravity * timeStep;
                
                // Calculate next position
                Vector3 nextPosition = position + velocity * timeStep;

                // Check for collision
                RaycastHit hit;
                if (Physics.Linecast(position, nextPosition, out hit, collisionMask))
                {
                    // Add point at collision
                    points.Add(hit.point);
                    
                    // Optionally continue with ricochet
                    // For now, stop at collision
                    break;
                }

                position = nextPosition;
                points.Add(position);
                time += timeStep;
            }

            return points.ToArray();
        }

        /// <summary>
        /// Update line color based on trajectory
        /// </summary>
        private void UpdateColor(Vector3[] points)
        {
            if (lineRenderer == null || points.Length < 2) return;

            // Simple color based on trajectory length
            float totalDistance = 0f;
            for (int i = 1; i < points.Length; i++)
            {
                totalDistance += Vector3.Distance(points[i - 1], points[i]);
            }

            Color color;
            if (totalDistance < 5f)
                color = normalColor;
            else if (totalDistance < 10f)
                color = warningColor;
            else
                color = dangerColor;

            lineRenderer.startColor = color;
            lineRenderer.endColor = new Color(color.r, color.g, color.b, 0.3f);

            // Update width
            lineRenderer.startWidth = normalWidth;
            lineRenderer.endWidth = normalWidth * 0.5f;
        }

        /// <summary>
        /// Set trajectory parameters
        /// </summary>
        public void SetParameters(int points, float step, float max)
        {
            pointCount = points;
            timeStep = step;
            maxTime = max;
        }
    }
}
