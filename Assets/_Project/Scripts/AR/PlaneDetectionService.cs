using UnityEngine;
using System.Collections.Generic;
using BreachAR.Core;
using VContainer;

namespace BreachAR.AR
{
    /// <summary>
    /// Wrapper service for AR plane detection
    /// Referência: AR-004
    /// </summary>
    public class PlaneDetectionService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float minPlaneArea = 0.1f;
        [SerializeField] private bool detectHorizontal = true;
        [SerializeField] private bool detectVertical = true;
        [SerializeField] private float planeUpdateInterval = 0.5f;

        [Header("State")]
        [SerializeField] private bool isDetecting;
        [SerializeField] private int detectedPlaneCount;
        [SerializeField] private float totalDetectedArea;

        private ARSessionService arSessionService;
        private List<ARDetectedPlane> detectedPlanes = new List<ARDetectedPlane>();
        private float lastUpdateTime;
        private bool isInitialized;

        public bool IsDetecting => isDetecting;
        public int DetectedPlaneCount => detectedPlaneCount;
        public float TotalDetectedArea => totalDetectedArea;
        public IReadOnlyList<ARDetectedPlane> DetectedPlanes => detectedPlanes;

        /// <summary>
        /// Event raised when a new plane is detected
        /// </summary>
        public event System.Action<ARDetectedPlane> OnPlaneDetected;

        /// <summary>
        /// Event raised when a plane is updated
        /// </summary>
        public event System.Action<ARDetectedPlane> OnPlaneUpdated;

        /// <summary>
        /// Event raised when a plane is removed
        /// </summary>
        public event System.Action<ARDetectedPlane> OnPlaneRemoved;

        [Inject]
        public void Construct(ARSessionService session)
        {
            arSessionService = session;
            isInitialized = true;
        }

        private void Start()
        {
            if (!isInitialized)
            {
                isInitialized = true;
            }
        }

        private void Update()
        {
            if (!isDetecting || !isInitialized) return;

            if (Time.time - lastUpdateTime >= planeUpdateInterval)
            {
                UpdatePlaneDetection();
                lastUpdateTime = Time.time;
            }
        }

        /// <summary>
        /// Start plane detection
        /// Referência: AR-004
        /// </summary>
        public void StartDetection()
        {
            isDetecting = true;
            detectedPlanes.Clear();
            detectedPlaneCount = 0;
            totalDetectedArea = 0f;
            lastUpdateTime = Time.time;

            Debug.Log("[PlaneDetection] Started");
        }

        /// <summary>
        /// Stop plane detection
        /// </summary>
        public void StopDetection()
        {
            isDetecting = false;
            Debug.Log("[PlaneDetection] Stopped");
        }

        /// <summary>
        /// Update plane detection (called periodically)
        /// </summary>
        private void UpdatePlaneDetection()
        {
            // In production, this would read from ARPlaneManager
            // For now, we simulate the detection

            if (arSessionService == null) return;

            // Check for new surfaces from AR session
            var surfaces = arSessionService.DetectedSurfaces;
            if (surfaces == null) return;

            // Process detected surfaces
            foreach (var surface in surfaces)
            {
                if (surface == null) continue;

                // Check if plane is already tracked
                var existing = FindPlaneById(surface.SurfaceId);
                if (existing != null)
                {
                    // Update existing plane
                    existing.UpdateFromSurface(surface);
                    OnPlaneUpdated?.Invoke(existing);
                }
                else if (surface.Area >= minPlaneArea)
                {
                    // Create new tracked plane
                    var newPlane = new ARDetectedPlane
                    {
                        PlaneId = surface.SurfaceId,
                        Type = ClassifyPlaneType(surface),
                        Center = surface.Center,
                        Normal = surface.Normal,
                        Area = surface.Area,
                        LastUpdated = Time.time
                    };

                    detectedPlanes.Add(newPlane);
                    detectedPlaneCount = detectedPlanes.Count;
                    totalDetectedArea += surface.Area;

                    // Add to AR session
                    arSessionService.AddSurface(surface);

                    OnPlaneDetected?.Invoke(newPlane);
                }
            }

            // Remove old planes (not updated in 5 seconds)
            float currentTime = Time.time;
            for (int i = detectedPlanes.Count - 1; i >= 0; i--)
            {
                if (currentTime - detectedPlanes[i].LastUpdated > 5f)
                {
                    var removed = detectedPlanes[i];
                    detectedPlanes.RemoveAt(i);
                    detectedPlaneCount = detectedPlanes.Count;
                    totalDetectedArea -= removed.Area;
                    OnPlaneRemoved?.Invoke(removed);
                }
            }
        }

        /// <summary>
        /// Classify plane type based on orientation
        /// Referência: AR-005
        /// </summary>
        private ARPlaneType ClassifyPlaneType(ScannedSurface surface)
        {
            float dotUp = Vector3.Dot(surface.Normal, Vector3.up);

            if (dotUp > 0.7f)
                return ARPlaneType.Horizontal;
            else if (dotUp < -0.7f)
                return ARPlaneType.Ceiling;
            else
                return ARPlaneType.Vertical;
        }

        /// <summary>
        /// Find plane by ID
        /// </summary>
        private ARDetectedPlane FindPlaneById(string planeId)
        {
            return detectedPlanes.Find(p => p.PlaneId == planeId);
        }

        /// <summary>
        /// Get planes by type
        /// </summary>
        public List<ARDetectedPlane> GetPlanesByType(ARPlaneType type)
        {
            return detectedPlanes.FindAll(p => p.Type == type);
        }

        /// <summary>
        /// Get largest plane
        /// </summary>
        public ARDetectedPlane GetLargestPlane()
        {
            ARDetectedPlane largest = null;
            float largestArea = 0f;

            foreach (var plane in detectedPlanes)
            {
                if (plane.Area > largestArea)
                {
                    largest = plane;
                    largestArea = plane.Area;
                }
            }

            return largest;
        }

        /// <summary>
        /// Check if minimum scan criteria is met
        /// Referência: AR-019
        /// </summary>
        public bool IsScanComplete()
        {
            bool hasHorizontal = GetPlanesByType(ARPlaneType.Horizontal).Count > 0;
            bool hasVertical = GetPlanesByType(ARPlaneType.Vertical).Count > 0;
            bool hasEnoughArea = totalDetectedArea >= 1.0f; // 1 square meter minimum

            return hasHorizontal && hasVertical && hasEnoughArea;
        }

        /// <summary>
        /// Get scan progress (0-1)
        /// </summary>
        public float GetScanProgress()
        {
            float planeProgress = Mathf.Clamp01(detectedPlaneCount / 3f); // Need 3 planes
            float areaProgress = Mathf.Clamp01(totalDetectedArea / 2f); // Need 2 sq meters

            return (planeProgress + areaProgress) / 2f;
        }
    }

    /// <summary>
    /// Detected plane data
    /// </summary>
    [System.Serializable]
    public class ARDetectedPlane
    {
        public string PlaneId;
        public ARPlaneType Type;
        public Vector3 Center;
        public Vector3 Normal;
        public float Area;
        public float LastUpdated;

        public void UpdateFromSurface(ScannedSurface surface)
        {
            Center = surface.Center;
            Normal = surface.Normal;
            Area = surface.Area;
            LastUpdated = Time.time;
        }
    }

    /// <summary>
    /// Plane types
    /// </summary>
    public enum ARPlaneType
    {
        Horizontal,
        Vertical,
        Ceiling,
        Slanted
    }
}
