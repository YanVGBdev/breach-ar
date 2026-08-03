using UnityEngine;
using System.Collections;
using BreachAR.Core;

namespace BreachAR.AR
{
    /// <summary>
    /// Generates and updates dynamic mesh colliders for AR planes
    /// Referência: AR-013
    /// </summary>
    public class DynamicMeshCollider : MonoBehaviour
    {
        [Header("Collider Settings")]
        [SerializeField] private float updateInterval = 1f;
        [SerializeField] private float minAreaForCollider = 0.2f;
        [SerializeField] private int simplificationLevel = 0;

        [Header("State")]
        [SerializeField] private bool isUpdating;
        [SerializeField] private float lastUpdateTime;
        [SerializeField] private int colliderCount;

        private ScannedSurface trackedSurface;
        private MeshCollider meshCollider;
        private MeshFilter meshFilter;
        private Vector3[] lastBoundary;
        private float lastArea;

        public bool IsUpdating => isUpdating;
        public int ColliderCount => colliderCount;

        /// <summary>
        /// Initialize for a specific surface
        /// </summary>
        public void Initialize(ScannedSurface surface)
        {
            trackedSurface = surface;

            // Ensure required components
            meshCollider = gameObject.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }

            meshFilter = gameObject.GetComponent<MeshFilter>();
            if (meshFilter == null)
            {
                meshFilter = gameObject.AddComponent<MeshFilter>();
            }

            // Generate initial mesh
            UpdateColliderMesh();

            isUpdating = true;
            lastUpdateTime = Time.time;
        }

        private void Update()
        {
            if (!isUpdating || trackedSurface == null) return;

            // Throttle updates
            if (Time.time - lastUpdateTime >= updateInterval)
            {
                // Check if surface has changed significantly
                if (HasSurfaceChanged())
                {
                    UpdateColliderMesh();
                }
                lastUpdateTime = Time.time;
            }
        }

        /// <summary>
        /// Check if surface has changed enough to warrant update
        /// </summary>
        private bool HasSurfaceChanged()
        {
            if (trackedSurface == null) return false;

            // Check area change
            float areaDelta = Mathf.Abs(trackedSurface.Area - lastArea);
            if (areaDelta > lastArea * 0.2f) return true; // 20% change

            // Check position change
            Vector3 currentCenter = trackedSurface.Center;
            if (lastBoundary == null || lastBoundary.Length == 0) return true;

            Vector3 lastCenter = Vector3.zero;
            foreach (var v in lastBoundary) lastCenter += v;
            lastCenter /= lastBoundary.Length;

            float positionDelta = Vector3.Distance(currentCenter, lastCenter);
            return positionDelta > 0.1f; // 10cm movement
        }

        /// <summary>
        /// Update the collider mesh from surface data
        /// Referência: AR-013
        /// </summary>
        private void UpdateColliderMesh()
        {
            if (trackedSurface == null) return;

            // Skip small surfaces
            if (trackedSurface.Area < minAreaForCollider)
            {
                meshCollider.enabled = false;
                return;
            }

            meshCollider.enabled = true;

            // Generate mesh from surface bounds
            Mesh mesh = GenerateMeshFromSurface(trackedSurface);

            if (mesh != null)
            {
                meshFilter.mesh = mesh;
                meshCollider.sharedMesh = mesh;
                lastBoundary = mesh.vertices;
                lastArea = trackedSurface.Area;
                colliderCount++;
            }
        }

        /// <summary>
        /// Generate a mesh from scanned surface data
        /// </summary>
        private Mesh GenerateMeshFromSurface(ScannedSurface surface)
        {
            Mesh mesh = new Mesh();
            mesh.name = $"DynamicCollider_{surface.SurfaceId}";

            Vector3 center = surface.Center;
            Vector3 normal = surface.Normal;

            // Calculate orientation
            Vector3 right = Vector3.Cross(normal, Vector3.up).normalized;
            if (right.sqrMagnitude < 0.01f)
                right = Vector3.Cross(normal, Vector3.forward).normalized;
            Vector3 forward = Vector3.Cross(right, normal).normalized;

            // Create quad based on surface area
            float size = Mathf.Sqrt(surface.Area);

            // Apply simplification (reduce vertices for performance)
            int segments = Mathf.Max(1, 4 - simplificationLevel);

            Vector3[] vertices = new Vector3[(segments + 1) * (segments + 1)];
            Vector2[] uvs = new Vector2[vertices.Length];
            Vector3[] normals = new Vector3[vertices.Length];

            float step = size / segments;
            float halfSize = size * 0.5f;

            for (int y = 0; y <= segments; y++)
            {
                for (int x = 0; x <= segments; x++)
                {
                    int index = y * (segments + 1) + x;
                    Vector2 uv = new Vector2((float)x / segments, (float)y / segments);
                    Vector3 localPos = new Vector3(
                        (uv.x - 0.5f) * size,
                        0,
                        (uv.y - 0.5f) * size
                    );

                    vertices[index] = center + (right * localPos.x + forward * localPos.z);
                    uvs[index] = uv;
                    normals[index] = normal;
                }
            }

            // Generate triangles
            int[] triangles = new int[segments * segments * 6];
            int triIndex = 0;

            for (int y = 0; y < segments; y++)
            {
                for (int x = 0; x < segments; x++)
                {
                    int bottomLeft = y * (segments + 1) + x;
                    int bottomRight = bottomLeft + 1;
                    int topLeft = bottomLeft + segments + 1;
                    int topLeftRight = topLeft + 1;

                    triangles[triIndex++] = bottomLeft;
                    triangles[triIndex++] = topLeft;
                    triangles[triIndex++] = bottomRight;

                    triangles[triIndex++] = bottomRight;
                    triangles[triIndex++] = topLeft;
                    triangles[triIndex++] = topLeftRight;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uvs;

            return mesh;
        }

        /// <summary>
        /// Stop updating this collider
        /// </summary>
        public void StopUpdating()
        {
            isUpdating = false;
        }

        /// <summary>
        /// Cleanup when destroyed
        /// </summary>
        private void OnDestroy()
        {
            if (meshFilter?.mesh != null)
            {
                Destroy(meshFilter.mesh);
            }
        }
    }
}
