using UnityEngine;
using System.Collections.Generic;
using BreachAR.Core;

namespace BreachAR.Tests
{
    /// <summary>
    /// Mock AR surface provider for testing without real AR
    /// Referência: QA-011
    /// </summary>
    public class MockARSurfaceProvider : MonoBehaviour, IARSurfaceProvider
    {
        [Header("Mock Surfaces")]
        [SerializeField] private List<MockSurface> mockSurfaces = new List<MockSurface>();

        [Header("State")]
        [SerializeField] private bool simulateScanComplete;
        [SerializeField] private float simulateScanProgress;

        private List<ScannedSurface> surfaces = new List<ScannedSurface>();

        public bool IsScanComplete => simulateScanComplete;
        public float ScanProgress => simulateScanProgress;

        private void Start()
        {
            GenerateMockSurfaces();
        }

        /// <summary>
        /// Generate mock surfaces for testing
        /// </summary>
        private void GenerateMockSurfaces()
        {
            surfaces.Clear();

            foreach (var mock in mockSurfaces)
            {
                surfaces.Add(new ScannedSurface
                {
                    SurfaceId = mock.SurfaceId,
                    Type = mock.Type,
                    Center = mock.Position,
                    Position = mock.Position,
                    Normal = mock.Normal,
                    Area = mock.Area,
                    Bounds = new Bounds(mock.Position, Vector3.one * mock.Area)
                });
            }

            // Add default surfaces if none configured
            if (surfaces.Count == 0)
            {
                surfaces.Add(new ScannedSurface
                {
                    SurfaceId = "mock_floor_1",
                    Type = SurfaceType.Floor,
                    Center = Vector3.zero,
                    Position = Vector3.zero,
                    Normal = Vector3.up,
                    Area = 2.0f
                });

                surfaces.Add(new ScannedSurface
                {
                    SurfaceId = "mock_wall_1",
                    Type = SurfaceType.Wall,
                    Center = new Vector3(0, 1, 2),
                    Position = new Vector3(0, 1, 2),
                    Normal = Vector3.back,
                    Area = 1.5f
                });

                surfaces.Add(new ScannedSurface
                {
                    SurfaceId = "mock_ceiling_1",
                    Type = SurfaceType.Ceiling,
                    Center = new Vector3(0, 2.5f, 0),
                    Position = new Vector3(0, 2.5f, 0),
                    Normal = Vector3.down,
                    Area = 1.0f
                });
            }

            Debug.Log($"[MockAR] Generated {surfaces.Count} mock surfaces");
        }

        /// <summary>
        /// Get all surfaces (IARSurfaceProvider implementation)
        /// </summary>
        public ScannedSurface[] GetSurfaces()
        {
            return surfaces.ToArray();
        }

        /// <summary>
        /// Simulate scan progress
        /// </summary>
        public void SimulateScan(float progress)
        {
            simulateScanProgress = Mathf.Clamp01(progress);
            simulateScanComplete = progress >= 1f;
        }

        /// <summary>
        /// Add a runtime mock surface
        /// </summary>
        public void AddSurface(SurfaceType type, Vector3 position, float area = 1f)
        {
            surfaces.Add(new ScannedSurface
            {
                SurfaceId = $"runtime_{surfaces.Count}",
                Type = type,
                Center = position,
                Position = position,
                Normal = type == SurfaceType.Floor ? Vector3.up :
                         type == SurfaceType.Ceiling ? Vector3.down :
                         type == SurfaceType.Wall ? Vector3.back : Vector3.up,
                Area = area
            });
        }

        /// <summary>
        /// Clear all surfaces
        /// </summary>
        public void ClearSurfaces()
        {
            surfaces.Clear();
            simulateScanProgress = 0f;
            simulateScanComplete = false;
        }

        /// <summary>
        /// Get surfaces of a specific type
        /// </summary>
        public ScannedSurface[] GetSurfacesByType(SurfaceType type)
        {
            return surfaces.FindAll(s => s.Type == type).ToArray();
        }
    }

    /// <summary>
    /// Mock surface configuration
    /// </summary>
    [System.Serializable]
    public class MockSurface
    {
        public string SurfaceId;
        public SurfaceType Type;
        public Vector3 Position;
        public Vector3 Normal;
        public float Area = 1f;
    }
}
