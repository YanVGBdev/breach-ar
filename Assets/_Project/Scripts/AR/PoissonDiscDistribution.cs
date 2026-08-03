using UnityEngine;
using System.Collections.Generic;
using BreachAR.Core;
using VContainer;

namespace BreachAR.AR
{
    /// <summary>
    /// Poisson-disc distribution for procedural rift placement
    /// Referência: AR-018, AR-024, AI-012
    /// Ensures minimum distance between rifts for natural spacing
    /// </summary>
    public class PoissonDiscDistribution : MonoBehaviour
    {
        [Header("Distribution Settings")]
        [SerializeField] private float minDistance = 0.8f; // Minimum distance between rifts
        [SerializeField] private float maxDistance = 2.5f; // Maximum distance from core
        [SerializeField] private int maxAttempts = 30; // Max attempts per point
        [SerializeField] private int maxRifts = 8; // Maximum rifts per session

        [Header("Surface Preferences")]
        [SerializeField] private float floorWeight = 1.0f;
        [SerializeField] private float wallWeight = 0.7f;
        [SerializeField] private float ceilingWeight = 0.3f;
        [SerializeField] private float furnitureWeight = 0.5f;

        [Inject] private ARSessionService arSessionService;

        private List<Vector2> generatedPoints;
        private System.Random sessionRandom;

        public int GeneratedCount => generatedPoints?.Count ?? 0;

        private void Awake()
        {
            generatedPoints = new List<Vector2>();
            sessionRandom = new System.Random(System.Environment.TickCount);
        }

        /// <summary>
        /// Generate rift positions using Poisson-disc sampling
        /// Referência: AR-018
        /// </summary>
        public List<RiftSpawnPoint> GenerateRiftPositions(Vector3 corePosition, int count = -1)
        {
            if (count < 0) count = maxRifts;
            count = Mathf.Min(count, maxRifts);

            var spawnPoints = new List<RiftSpawnPoint>();
            generatedPoints.Clear();

            // Get valid surfaces
            var surfaces = arSessionService?.DetectedSurfaces;
            if (surfaces == null || surfaces.Count == 0)
            {
                Debug.LogWarning("[PoissonDisc] No surfaces available for rift placement");
                return spawnPoints;
            }

            // Convert 3D positions to 2D for Poisson-disc sampling
            // We'll sample on horizontal plane and then project to surfaces
            Vector2 center2D = new Vector2(corePosition.x, corePosition.z);

            // Run Poisson-disc sampling
            var candidates = PoissonDiscSample(center2D, minDistance, maxDistance, maxAttempts * count);

            // Filter and select best positions
            foreach (var candidate in candidates)
            {
                if (spawnPoints.Count >= count) break;

                // Convert back to 3D
                Vector3 candidatePos3D = new Vector3(candidate.x, corePosition.y, candidate.y);

                // Find closest surface
                ScannedSurface closestSurface = FindClosestSurface(candidatePos3D, surfaces);
                if (closestSurface == null) continue;

                // Calculate surface-specific position
                Vector3 surfacePosition = ProjectToSurface(candidatePos3D, closestSurface);
                Quaternion surfaceRotation = Quaternion.LookRotation(closestSurface.Normal);

                // Apply weight-based filtering
                float surfaceWeight = GetSurfaceWeight(closestSurface.Type);
                if (sessionRandom.NextDouble() > surfaceWeight) continue;

                // Check minimum distance from existing points
                if (!MeetsMinimumDistance(surfacePosition, spawnPoints, minDistance)) continue;

                spawnPoints.Add(new RiftSpawnPoint
                {
                    Position = surfacePosition,
                    Rotation = surfaceRotation,
                    SurfaceType = closestSurface.Type,
                    SurfaceArea = closestSurface.Area,
                    DistanceFromCore = Vector3.Distance(surfacePosition, corePosition)
                });

                generatedPoints.Add(candidate);
            }

            Debug.Log($"[PoissonDisc] Generated {spawnPoints.Count} rift positions (requested: {count})");
            return spawnPoints;
        }

        /// <summary>
        /// Poisson-disc sampling algorithm
        /// </summary>
        private List<Vector2> PoissonDiscSample(Vector2 center, float minDist, float maxDist, int maxPoints)
        {
            var points = new List<Vector2>();
            var active = new List<Vector2>();

            // Start with center point
            Vector2 firstPoint = center;
            points.Add(firstPoint);
            active.Add(firstPoint);

            while (active.Count > 0 && points.Count < maxPoints)
            {
                // Pick random active point
                int idx = sessionRandom.Next(active.Count);
                Vector2 point = active[idx];

                bool found = false;

                for (int attempt = 0; attempt < maxAttempts; attempt++)
                {
                    // Generate random point around candidate
                    float angle = (float)(sessionRandom.NextDouble() * 2 * Mathf.PI);
                    float dist = minDist + (float)(sessionRandom.NextDouble() * (maxDist - minDist));

                    Vector2 newPoint = point + new Vector2(
                        Mathf.Cos(angle) * dist,
                        Mathf.Sin(angle) * dist
                    );

                    // Check if within bounds (distance from center)
                    float distFromCenter = Vector2.Distance(newPoint, center);
                    if (distFromCenter > maxDist) continue;

                    // Check minimum distance from existing points
                    bool valid = true;
                    foreach (var existing in points)
                    {
                        if (Vector2.Distance(newPoint, existing) < minDist)
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid)
                    {
                        points.Add(newPoint);
                        active.Add(newPoint);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    active.RemoveAt(idx);
                }
            }

            return points;
        }

        /// <summary>
        /// Find closest surface to a position
        /// </summary>
        private ScannedSurface FindClosestSurface(Vector3 position, List<ScannedSurface> surfaces)
        {
            ScannedSurface closest = null;
            float closestDist = float.MaxValue;

            foreach (var surface in surfaces)
            {
                float dist = Vector3.Distance(position, surface.Position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closest = surface;
                }
            }

            return closest;
        }

        /// <summary>
        /// Project a point onto a surface
        /// </summary>
        private Vector3 ProjectToSurface(Vector3 point, ScannedSurface surface)
        {
            // Simple projection: move to surface position with slight offset
            Vector3 projected = surface.Position;
            projected.y = point.y; // Keep height from original position

            // Add slight offset along surface normal
            projected += surface.Normal * 0.05f;

            return projected;
        }

        /// <summary>
        /// Get weight for surface type
        /// </summary>
        private float GetSurfaceWeight(SurfaceType type)
        {
            return type switch
            {
                SurfaceType.Floor => floorWeight,
                SurfaceType.Wall => wallWeight,
                SurfaceType.Ceiling => ceilingWeight,
                SurfaceType.Furniture => furnitureWeight,
                _ => 0.5f
            };
        }

        /// <summary>
        /// Check if position meets minimum distance from existing points
        /// Referência: AR-024
        /// </summary>
        private bool MeetsMinimumDistance(Vector3 position, List<RiftSpawnPoint> existing, float minDist)
        {
            foreach (var point in existing)
            {
                if (Vector3.Distance(position, point.Position) < minDist)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Reset for new session
        /// </summary>
        public void ResetSession()
        {
            generatedPoints.Clear();
            sessionRandom = new System.Random(System.Environment.TickCount);
        }
    }

    /// <summary>
    /// Rift spawn point data
    /// </summary>
    [System.Serializable]
    public class RiftSpawnPoint
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public SurfaceType SurfaceType;
        public float SurfaceArea;
        public float DistanceFromCore;
    }
}
