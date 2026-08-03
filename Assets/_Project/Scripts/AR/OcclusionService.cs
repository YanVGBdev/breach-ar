using UnityEngine;
using VContainer;

namespace BreachAR.AR
{
    /// <summary>
    /// Manages AR occlusion (Depth API + mesh fallback)
    /// Referência: AR-008, AR-009, AR-010
    /// </summary>
    public class OcclusionService : MonoBehaviour
    {
        [Header("Occlusion Settings")]
        [SerializeField] private bool enableOcclusion = true;
        [SerializeField] private float depthSamplingFrequency = 30f;
        [SerializeField] private float meshUpdateInterval = 2f;

        [Header("References")]
        [SerializeField] private Material occlusionMaterial;
        [SerializeField] private Camera arCamera;

        [Inject] private DeviceCompatibilityService deviceCompat;
        [Inject] private ARSessionService arSession;

        private OcclusionMode currentMode;
        private bool isInitialized;
        private float lastMeshUpdateTime;

        public OcclusionMode CurrentMode => currentMode;
        public bool IsEnabled => enableOcclusion && isInitialized;

        private void Start()
        {
            InitializeOcclusion();
        }

        /// <summary>
        /// Initialize occlusion based on device capabilities
        /// Referência: AR-008, AR-010
        /// </summary>
        private void InitializeOcclusion()
        {
            if (deviceCompat == null)
            {
                Debug.LogWarning("[Occlusion] DeviceCompatibilityService not available");
                currentMode = OcclusionMode.Disabled;
                return;
            }

            // Check device tier and depth API support
            var tier = deviceCompat.DetectedTier;
            bool supportsDepth = deviceCompat.CachedCapability.SupportsDepthAPI;

            if (supportsDepth && tier != DeviceTier.Low)
            {
                currentMode = OcclusionMode.DepthAPI;
                Debug.Log("[Occlusion] Using Depth API occlusion");
            }
            else if (tier == DeviceTier.Medium)
            {
                currentMode = OcclusionMode.MeshFallback;
                Debug.Log("[Occlusion] Using mesh fallback occlusion");
            }
            else
            {
                currentMode = OcclusionMode.Disabled;
                Debug.Log("[Occlusion] Occlusion disabled for low-tier device");
            }

            isInitialized = true;
        }

        private void Update()
        {
            if (!isInitialized || !enableOcclusion) return;

            switch (currentMode)
            {
                case OcclusionMode.DepthAPI:
                    UpdateDepthOcclusion();
                    break;
                case OcclusionMode.MeshFallback:
                    UpdateMeshOcclusion();
                    break;
            }
        }

        /// <summary>
        /// Update depth-based occlusion
        /// Referência: AR-008
        /// </summary>
        private void UpdateDepthOcclusion()
        {
            if (arCamera == null) return;

            // In production, this would:
            // 1. Read depth texture from AROcclusionManager
            // 2. Apply to occlusion shader
            // 3. Update depth sampling based on tier

            // Simplified - just ensure material is set
            if (occlusionMaterial != null)
            {
                Shader.SetGlobalVector("_DepthTexelSize", new Vector4(1f / 256f, 1f / 256f, 0, 0));
            }
        }

        /// <summary>
        /// Update mesh-based occlusion (fallback)
        /// Referência: AR-010
        /// </summary>
        private void UpdateMeshOcclusion()
        {
            // Throttle mesh updates
            if (Time.time - lastMeshUpdateTime < meshUpdateInterval) return;
            lastMeshUpdateTime = Time.time;

            // Generate occlusion mesh from detected planes
            if (arSession != null && arSession.DetectedSurfaces != null)
            {
                GenerateOcclusionMeshes(arSession.DetectedSurfaces);
            }
        }

        /// <summary>
        /// Generate occlusion meshes from detected AR planes
        /// </summary>
        private void GenerateOcclusionMeshes(System.Collections.Generic.List<ScannedSurface> surfaces)
        {
            foreach (var surface in surfaces)
            {
                if (surface == null || surface.Area < 0.1f) continue;

                // Create or update occlusion mesh for this surface
                MeshFilter meshFilter = surface.Anchor?.GetComponent<MeshFilter>();
                if (meshFilter == null && surface.Anchor != null)
                {
                    meshFilter = surface.Anchor.gameObject.AddComponent<MeshFilter>();
                    MeshRenderer renderer = surface.Anchor.gameObject.AddComponent<MeshRenderer>();
                    renderer.material = occlusionMaterial;
                    renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    renderer.receiveShadows = false;

                    // Set layer to occlusion layer
                    surface.Anchor.gameObject.layer = LayerMask.NameToLayer("AROcclusion");
                }

                if (meshFilter != null && meshFilter.mesh == null)
                {
                    meshFilter.mesh = CreatePlaneMesh(surface);
                }
            }
        }

        /// <summary>
        /// Create a simple plane mesh for occlusion
        /// </summary>
        private Mesh CreatePlaneMesh(ScannedSurface surface)
        {
            Mesh mesh = new Mesh();
            mesh.name = $"OcclusionMesh_{surface.SurfaceId}";

            Vector3 center = surface.Position;
            Vector3 normal = surface.Normal;
            Vector3 right = Vector3.Cross(normal, Vector3.up).normalized;
            if (right.sqrMagnitude < 0.01f) right = Vector3.Cross(normal, Vector3.forward).normalized;
            Vector3 forward = Vector3.Cross(right, normal).normalized;

            float halfSize = Mathf.Sqrt(surface.Area) * 0.5f;

            Vector3[] vertices = new Vector3[4]
            {
                center + (-right - forward) * halfSize,
                center + (right - forward) * halfSize,
                center + (right + forward) * halfSize,
                center + (-right + forward) * halfSize
            };

            int[] triangles = new int[6] { 0, 2, 1, 0, 3, 2 };
            Vector3[] normals = new Vector3[4] { normal, normal, normal, normal };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;

            return mesh;
        }

        /// <summary>
        /// Enable/disable occlusion
        /// </summary>
        public void SetEnabled(bool enabled)
        {
            enableOcclusion = enabled;

            if (!enabled)
            {
                DisableOcclusion();
            }
        }

        /// <summary>
        /// Disable occlusion and clean up
        /// </summary>
        private void DisableOcclusion()
        {
            // Remove occlusion meshes
            if (arSession?.DetectedSurfaces != null)
            {
                foreach (var surface in arSession.DetectedSurfaces)
                {
                    if (surface?.Anchor != null)
                    {
                        var meshFilter = surface.Anchor.GetComponent<MeshFilter>();
                        if (meshFilter != null) Destroy(meshFilter);
                        var renderer = surface.Anchor.GetComponent<MeshRenderer>();
                        if (renderer != null) Destroy(renderer);
                    }
                }
            }
        }

        /// <summary>
        /// Get occlusion status for debugging
        /// </summary>
        public string GetStatus()
        {
            return $"Mode: {currentMode} | Enabled: {enableOcclusion} | Initialized: {isInitialized}";
        }
    }

    /// <summary>
    /// Occlusion rendering modes
    /// </summary>
    public enum OcclusionMode
    {
        Disabled,
        DepthAPI,
        MeshFallback
    }
}
