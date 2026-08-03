using UnityEngine;
using BreachAR.Core;
using BreachAR.Gameplay;
using VContainer;

namespace BreachAR.AR
{
    /// <summary>
    /// Service for placing and anchoring the Core in AR
    /// Referência: AR-006, specs/CoreSystem.md
    /// </summary>
    public class CorePlacementService : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject corePrefab;
        [SerializeField] private float placementHeight = 0.5f;
        [SerializeField] private LayerMask placementLayer;

        [Header("State")]
        [SerializeField] private bool isPlaced;
        [SerializeField] private GameObject placedCore;

        private Camera mainCamera;
        private ARSessionService arSessionService;

        public bool IsPlaced => isPlaced;
        public GameObject PlacedCore => placedCore;

        [Inject]
        public void Construct(ARSessionService arService)
        {
            arSessionService = arService;
        }

        private void Start()
        {
            mainCamera = Camera.main;
        }

        /// <summary>
        /// Check if position is valid for Core placement
        /// </summary>
        public bool IsValidPlacement(Vector3 position)
        {
            // Check if position is on a valid surface
            Ray ray = new Ray(position + Vector3.up * 0.1f, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 1f, placementLayer))
            {
                // Check surface type
                var surface = arSessionService?.GetLargestSurface(SurfaceType.Floor);
                if (surface != null)
                {
                    float distance = Vector3.Distance(position, surface.Position);
                    return distance < 2f; // Within 2 meters of detected floor
                }
            }
            return false;
        }

        /// <summary>
        /// Place Core at position
        /// </summary>
        public void PlaceCore(Vector3 position)
        {
            if (isPlaced || corePrefab == null) return;

            // Snap to floor
            Vector3 placementPos = new Vector3(position.x, placementHeight, position.z);

            placedCore = Instantiate(corePrefab, placementPos, Quaternion.identity);
            isPlaced = true;

            Debug.Log($"[AR] Core placed at {placementPos}");
        }

        /// <summary>
        /// Remove placed Core
        /// </summary>
        public void RemoveCore()
        {
            if (placedCore != null)
            {
                Destroy(placedCore);
                placedCore = null;
            }
            isPlaced = false;
        }

        /// <summary>
        /// Get placement position from screen tap
        /// </summary>
        public Vector3 GetPlacementPosition(Vector2 screenPosition)
        {
            if (mainCamera == null) return Vector3.zero;

            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, placementLayer))
            {
                return hit.point;
            }

            return Vector3.zero;
        }
    }
}
