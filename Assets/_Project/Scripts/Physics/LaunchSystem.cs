using UnityEngine;
using BreachAR.Core;
using BreachAR.ScriptableObjects;

namespace BreachAR.Physics
{
    /// <summary>
    /// Handles orb launching mechanics with drag-based input
    /// </summary>
    public class LaunchSystem : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private LaunchConfig launchConfig;

        [Header("References")]
        [SerializeField] private Transform launchPoint;
        [SerializeField] private LineRenderer trajectoryLine;

        [Header("State")]
        [SerializeField] private bool isDragging;
        [SerializeField] private Vector2 dragStart;
        [SerializeField] private Vector2 dragCurrent;

        private Camera mainCamera;
        private float lastLaunchTime;

        public bool IsDragging => isDragging;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void Update()
        {
            HandleInput();
            
            if (isDragging)
            {
                UpdateTrajectoryPreview();
            }
        }

        private void HandleInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        StartDrag(touch.position);
                        break;
                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        UpdateDrag(touch.position);
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        EndDrag(touch.position);
                        break;
                }
            }
            else if (Input.GetMouseButtonDown(0))
            {
                StartDrag(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                UpdateDrag(Input.mousePosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                EndDrag(Input.mousePosition);
            }
        }

        private void StartDrag(Vector2 screenPosition)
        {
            // Check cooldown
            if (Time.time - lastLaunchTime < launchConfig.cooldown)
                return;

            // Check if dragging from valid area
            Vector2 worldPoint = mainCamera.ScreenToWorldPoint(screenPosition);
            float distanceFromLaunchPoint = Vector2.Distance(worldPoint, launchPoint.position);

            if (distanceFromLaunchPoint > launchConfig.maxDragDistance)
                return;

            isDragging = true;
            dragStart = screenPosition;
            dragCurrent = screenPosition;

            // Show trajectory line
            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = true;
            }
        }

        private void UpdateDrag(Vector2 screenPosition)
        {
            if (!isDragging) return;

            dragCurrent = screenPosition;

            // Clamp drag distance
            Vector2 dragVector = dragCurrent - dragStart;
            if (dragVector.magnitude > launchConfig.maxDragDistance * launchConfig.pixelsPerUnit)
            {
                dragVector = dragVector.normalized * launchConfig.maxDragDistance * launchConfig.pixelsPerUnit;
                dragCurrent = dragStart + dragVector;
            }
        }

        private void EndDrag(Vector2 screenPosition)
        {
            if (!isDragging) return;

            isDragging = false;
            lastLaunchTime = Time.time;

            // Calculate launch vector
            Vector2 dragVector = dragStart - dragCurrent; // Inverted direction
            float dragMagnitude = dragVector.magnitude;

            // Check minimum drag
            if (dragMagnitude < launchConfig.minDragDistance * launchConfig.pixelsPerUnit)
            {
                HideTrajectory();
                return;
            }

            // Calculate force
            float forceMagnitude = Mathf.Clamp(
                dragMagnitude / launchConfig.pixelsPerUnit,
                launchConfig.minForce,
                launchConfig.maxForce
            );

            Vector3 launchDirection = new Vector3(dragVector.x, dragVector.y, 0).normalized;

            // Launch orb
            LaunchOrb(launchDirection, forceMagnitude * launchConfig.forceMultiplier);

            HideTrajectory();
        }

        private void LaunchOrb(Vector3 direction, float force)
        {
            // TODO: Get orb from pool
            // GameObject orb = ObjectPool.Instance.Spawn("Orb", launchPoint.position, Quaternion.identity);
            // OrbController controller = orb.GetComponent<OrbController>();
            // controller.Initialize(currentOrbDefinition);
            // controller.Launch(direction, force);

            Debug.Log($"[Launch] Direction: {direction}, Force: {force}");
        }

        private void UpdateTrajectoryPreview()
        {
            if (trajectoryLine == null) return;

            Vector2 dragVector = dragStart - dragCurrent;
            float dragMagnitude = dragVector.magnitude;

            if (dragMagnitude < launchConfig.minDragDistance * launchConfig.pixelsPerUnit)
            {
                trajectoryLine.positionCount = 0;
                return;
            }

            float forceMagnitude = Mathf.Clamp(
                dragMagnitude / launchConfig.pixelsPerUnit,
                launchConfig.minForce,
                launchConfig.maxForce
            );

            Vector3 launchDirection = new Vector3(dragVector.x, dragVector.y, 0).normalized;
            Vector3 launchVelocity = launchDirection * forceMagnitude * launchConfig.forceMultiplier;

            // Simulate trajectory
            int pointCount = launchConfig.trajectoryPoints;
            trajectoryLine.positionCount = pointCount;

            Vector3 position = launchPoint.position;
            Vector3 velocity = launchVelocity;
            float timeStep = launchConfig.trajectoryTimeStep;

            for (int i = 0; i < pointCount; i++)
            {
                trajectoryLine.SetPosition(i, position);

                // Apply gravity
                velocity += Physics.gravity * timeStep;
                position += velocity * timeStep;
            }
        }

        private void HideTrajectory()
        {
            if (trajectoryLine != null)
            {
                trajectoryLine.positionCount = 0;
                trajectoryLine.enabled = false;
            }
        }
    }

    /// <summary>
    /// Configuration for launch system
    /// </summary>
    [System.Serializable]
    public class LaunchConfig
    {
        public float minDragDistance = 50f;
        public float maxDragDistance = 300f;
        public float pixelsPerUnit = 100f;
        public float minForce = 5f;
        public float maxForce = 20f;
        public float forceMultiplier = 1f;
        public float cooldown = 0.3f;
        public int trajectoryPoints = 30;
        public float trajectoryTimeStep = 0.05f;
    }
}
