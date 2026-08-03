using UnityEngine;
using System.Collections.Generic;
using BreachAR.Core;

namespace BreachAR.AR
{
    /// <summary>
    /// Wrapper service for AR Foundation functionality
    /// Referência: AR-002, specs/ARSurfaceService.md
    /// </summary>
    public class ARSessionService : MonoBehaviour
    {
        [Header("AR Settings")]
        [SerializeField] private float minPlaneArea = 0.3f;
        [SerializeField] private float scanTimeout = 8f;
        [SerializeField] private int minPlanesRequired = 2;

        [Header("State")]
        [SerializeField] private bool isSessionActive;
        [SerializeField] private bool isScanComplete;
        [SerializeField] private float scanProgress;

        private List<ScannedSurface> detectedSurfaces;
        private float scanStartTime;
        private bool hasFloor;
        private bool hasWall;

        public bool IsSessionActive => isSessionActive;
        public bool IsScanComplete => isScanComplete;
        public float ScanProgress => scanProgress;
        public List<ScannedSurface> DetectedSurfaces => detectedSurfaces;

        private void Awake()
        {
            detectedSurfaces = new List<ScannedSurface>();
        }

        /// <summary>
        /// Start AR session
        /// </summary>
        public void StartSession()
        {
            Debug.Log("[AR] Starting AR session");
            isSessionActive = true;
            isScanComplete = false;
            scanProgress = 0f;
            scanStartTime = Time.time;
            detectedSurfaces.Clear();
            hasFloor = false;
            hasWall = false;
        }

        /// <summary>
        /// Stop AR session
        /// </summary>
        public void StopSession()
        {
            Debug.Log("[AR] Stopping AR session");
            isSessionActive = false;
        }

        /// <summary>
        /// Check if device supports AR
        /// Referência: AR-016
        /// </summary>
        public ARDeviceCapability CheckDeviceCapability()
        {
            var capability = new ARDeviceCapability
            {
                SupportsAR = true,
                SupportsDepthAPI = false,
                RAMGB = SystemInfo.systemMemorySize / 1024,
                GPUMemoryMB = SystemInfo.graphicsMemorySize,
                HasGyroscope = SystemInfo.supportsGyroscope
            };

            return capability;
        }

        /// <summary>
        /// Classify a detected plane
        /// Referência: AR-005
        /// </summary>
        public SurfaceType ClassifySurface(Vector3 planeNormal, float planeHeight, float cameraHeight)
        {
            if (Vector3.Dot(planeNormal, Vector3.up) > 0.7f)
            {
                if (planeHeight < cameraHeight - 0.5f)
                {
                    hasFloor = true;
                    return SurfaceType.Floor;
                }
                else
                {
                    return SurfaceType.Furniture;
                }
            }

            if (Vector3.Dot(planeNormal, Vector3.down) > 0.7f || 
                planeHeight > cameraHeight + 1.5f)
            {
                return SurfaceType.Ceiling;
            }

            if (Mathf.Abs(Vector3.Dot(planeNormal, Vector3.up)) < 0.3f)
            {
                hasWall = true;
                return SurfaceType.Wall;
            }

            return SurfaceType.Floor;
        }

        /// <summary>
        /// Add a detected surface
        /// </summary>
        public void AddSurface(ScannedSurface surface)
        {
            if (surface.Area < minPlaneArea) return;

            detectedSurfaces.Add(surface);
            
            // Emit event
            GameEvents.OnSurfaceDetected?.Invoke(new SurfaceDetectedData
            {
                SurfaceId = surface.SurfaceId,
                Type = surface.Type,
                Area = surface.Area,
                Position = surface.Position
            });

            UpdateScanProgress();
        }

        /// <summary>
        /// Update scan progress
        /// Referência: AR-019
        /// </summary>
        private void UpdateScanProgress()
        {
            int surfaceCount = detectedSurfaces.Count;
            bool meetsMinimum = hasFloor && hasWall;
            bool timeoutReached = (Time.time - scanStartTime) >= scanTimeout;

            if (meetsMinimum || timeoutReached || surfaceCount >= minPlanesRequired)
            {
                scanProgress = 1f;
                isScanComplete = true;
                
                // Emit scan complete event
                GameEvents.OnScanComplete?.Invoke(new ScanCompleteData
                {
                    SurfaceCount = surfaceCount,
                    Duration = Time.time - scanStartTime,
                    HasFloor = hasFloor,
                    HasWall = hasWall
                });
                
                Debug.Log("[AR] Scan complete");
            }
            else
            {
                float surfaceProgress = Mathf.Clamp01(surfaceCount / (float)minPlanesRequired);
                float timeProgress = Mathf.Clamp01((Time.time - scanStartTime) / scanTimeout);
                scanProgress = Mathf.Max(surfaceProgress, timeProgress);
            }
        }

        /// <summary>
        /// Start rescan
        /// Referência: AR-015
        /// </summary>
        public void Rescan()
        {
            Debug.Log("[AR] Starting rescan");
            isScanComplete = false;
            scanProgress = 0f;
            scanStartTime = Time.time;
            detectedSurfaces.Clear();
            hasFloor = false;
            hasWall = false;
        }

        /// <summary>
        /// Get surfaces by type
        /// </summary>
        public List<ScannedSurface> GetSurfacesByType(SurfaceType type)
        {
            return detectedSurfaces.FindAll(s => s.Type == type);
        }

        /// <summary>
        /// Get largest surface of a type
        /// </summary>
        public ScannedSurface GetLargestSurface(SurfaceType type)
        {
            ScannedSurface largest = null;
            float largestArea = 0f;

            foreach (var surface in detectedSurfaces)
            {
                if (surface.Type == type && surface.Area > largestArea)
                {
                    largest = surface;
                    largestArea = surface.Area;
                }
            }

            return largest;
        }

        /// <summary>
        /// Get a random valid surface for spawning
        /// Referência: AR-018
        /// </summary>
        public ScannedSurface GetRandomValidSurface()
        {
            if (detectedSurfaces.Count == 0) return null;
            
            // Prefer floor surfaces
            var floorSurfaces = GetSurfacesByType(SurfaceType.Floor);
            if (floorSurfaces.Count > 0)
            {
                return floorSurfaces[Random.Range(0, floorSurfaces.Count)];
            }
            
            return detectedSurfaces[Random.Range(0, detectedSurfaces.Count)];
        }
    }

    /// <summary>
    /// AR device capability check result
    /// </summary>
    [System.Serializable]
    public struct ARDeviceCapability
    {
        public bool SupportsAR;
        public bool SupportsDepthAPI;
        public int RAMGB;
        public int GPUMemoryMB;
        public bool HasGyroscope;
    }


}
