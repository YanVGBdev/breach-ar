using UnityEngine;
using VContainer;

namespace BreachAR.Physics
{
    /// <summary>
    /// Manages physics settings and collision layers
    /// </summary>
    public class PhysicsManager : MonoBehaviour
    {

        [Header("Physics Layers")]
        [SerializeField] private LayerMask orbLayer;
        [SerializeField] private LayerMask fragmentLayer;
        [SerializeField] private LayerMask realWorldSurfaceLayer;
        [SerializeField] private LayerMask coreLayer;
        [SerializeField] private LayerMask powerUpLayer;

        [Header("Physics Materials")]
        [SerializeField] private PhysicMaterial wallMaterial;
        [SerializeField] private PhysicMaterial furnitureMaterial;
        [SerializeField] private PhysicMaterial floorMaterial;

        [Header("Settings")]
        [SerializeField] private float fixedTimestep = 0.02f;
        [SerializeField] private float gravityScale = 0.6f;

        public LayerMask OrbLayer => orbLayer;
        public LayerMask FragmentLayer => fragmentLayer;
        public LayerMask RealWorldSurfaceLayer => realWorldSurfaceLayer;
        public LayerMask CoreLayer => coreLayer;
        public LayerMask PowerUpLayer => powerUpLayer;

        [Inject]
        private void Initialize()
        {
            InitializePhysics();
        }

        /// <summary>
        /// Initialize physics settings
        /// </summary>
        private void InitializePhysics()
        {
            // Set fixed timestep for consistent physics
            Time.fixedDeltaTime = fixedTimestep;

            // Set gravity with game scale
            Physics.gravity = new Vector3(0, -9.81f * gravityScale, 0);

            // Create physics materials if not assigned
            CreateDefaultMaterials();

            Debug.Log("[Physics] Initialized");
        }

        /// <summary>
        /// Create default physics materials
        /// </summary>
        private void CreateDefaultMaterials()
        {
            if (wallMaterial == null)
            {
                wallMaterial = new PhysicMaterial("WallMaterial");
                wallMaterial.bounciness = 0.7f;
                wallMaterial.dynamicFriction = 0.4f;
                wallMaterial.staticFriction = 0.4f;
            }

            if (furnitureMaterial == null)
            {
                furnitureMaterial = new PhysicMaterial("FurnitureMaterial");
                furnitureMaterial.bounciness = 0.4f;
                furnitureMaterial.dynamicFriction = 0.5f;
                furnitureMaterial.staticFriction = 0.5f;
            }

            if (floorMaterial == null)
            {
                floorMaterial = new PhysicMaterial("FloorMaterial");
                floorMaterial.bounciness = 0.2f;
                floorMaterial.dynamicFriction = 0.6f;
                floorMaterial.staticFriction = 0.6f;
            }
        }

        /// <summary>
        /// Get physics material for surface type
        /// </summary>
        public PhysicMaterial GetMaterialForSurface(Core.SurfaceType surfaceType)
        {
            switch (surfaceType)
            {
                case Core.SurfaceType.Wall:
                    return wallMaterial;
                case Core.SurfaceType.Furniture:
                    return furnitureMaterial;
                case Core.SurfaceType.Floor:
                case Core.SurfaceType.Ceiling:
                default:
                    return floorMaterial;
            }
        }

        /// <summary>
        /// Check if two layers can collide
        /// </summary>
        public bool CanCollide(LayerMask layer1, LayerMask layer2)
        {
            return Physics.GetIgnoreLayerCollision(
                LayerMaskToLayer(layer1),
                LayerMaskToLayer(layer2)
            ) == false;
        }

        /// <summary>
        /// Convert LayerMask to layer index
        /// </summary>
        private int LayerMaskToLayer(LayerMask mask)
        {
            int layer = mask.value;
            int layerIndex = 0;
            while (layer > 1)
            {
                layer >>= 1;
                layerIndex++;
            }
            return layerIndex;
        }

        /// <summary>
        /// Perform depth raycast
        /// </summary>
        public bool DepthRaycast(Vector3 origin, Vector3 direction, float maxDistance, out RaycastHit hit)
        {
            // TODO: Implement Depth API raycast
            // For now, use regular raycast
            return Physics.Raycast(origin, direction, out hit, maxDistance, realWorldSurfaceLayer);
        }

        /// <summary>
        /// Get gravity scale
        /// </summary>
        public float GetGravityScale()
        {
            return gravityScale;
        }

        /// <summary>
        /// Set gravity scale
        /// </summary>
        public void SetGravityScale(float scale)
        {
            gravityScale = scale;
            Physics.gravity = new Vector3(0, -9.81f * gravityScale, 0);
        }
    }
}
