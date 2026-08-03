using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BreachAR.Core;
using VContainer;

namespace BreachAR.AR
{
    /// <summary>
    /// Manages AR anchors for Rifts
    /// Referência: AR-007
    /// </summary>
    public class RiftAnchorManager : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float anchorLifetime = 60f;
        [SerializeField] private float cleanupInterval = 10f;
        [SerializeField] private int maxAnchors = 10;

        [Header("State")]
        [SerializeField] private int activeAnchorCount;
        [SerializeField] private int totalAnchorsCreated;

        private ARSessionService arSessionService;
        private List<ARAnchorData> anchors = new List<ARAnchorData>();
        private float lastCleanupTime;

        public int ActiveAnchorCount => activeAnchorCount;
        public int TotalAnchorsCreated => totalAnchorsCreated;

        /// <summary>
        /// Event raised when an anchor is created
        /// </summary>
        public event System.Action<ARAnchorData> OnAnchorCreated;

        /// <summary>
        /// Event raised when an anchor is released
        /// </summary>
        public event System.Action<ARAnchorData> OnAnchorReleased;

        [Inject]
        public void Construct(ARSessionService session)
        {
            arSessionService = session;
        }

        private void Start()
        {
            StartCoroutine(CleanupLoop());
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        /// <summary>
        /// Create an anchor at a position on a surface
        /// Referência: AR-007
        /// </summary>
        public ARAnchorData CreateAnchor(Vector3 position, Quaternion rotation, SurfaceType surfaceType)
        {
            if (anchors.Count >= maxAnchors)
            {
                Debug.LogWarning("[RiftAnchor] Maximum anchors reached, releasing oldest");
                ReleaseOldestAnchor();
            }

            var anchorData = new ARAnchorData
            {
                AnchorId = $"anchor_{totalAnchorsCreated++}",
                Position = position,
                Rotation = rotation,
                SurfaceType = surfaceType,
                CreatedTime = Time.time,
                IsActive = true,
                GameObject = CreateAnchorObject(position, rotation)
            };

            anchors.Add(anchorData);
            activeAnchorCount = anchors.Count;

            OnAnchorCreated?.Invoke(anchorData);

            Debug.Log($"[RiftAnchor] Created: {anchorData.AnchorId} at {position}");
            return anchorData;
        }

        /// <summary>
        /// Create anchor GameObject
        /// </summary>
        private GameObject CreateAnchorObject(Vector3 position, Quaternion rotation)
        {
            var anchorObj = new GameObject($"ARAnchor_{position.GetHashCode()}");
            anchorObj.transform.position = position;
            anchorObj.transform.rotation = rotation;

            // Add visual indicator (small sphere)
            var indicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            indicator.transform.SetParent(anchorObj.transform);
            indicator.transform.localPosition = Vector3.zero;
            indicator.transform.localScale = Vector3.one * 0.05f;

            // Remove collider
            var collider = indicator.GetComponent<Collider>();
            if (collider != null) Destroy(collider);

            // Set material
            var renderer = indicator.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = Color.cyan;
            }

            return anchorObj;
        }

        /// <summary>
        /// Release an anchor
        /// Referência: AR-020
        /// </summary>
        public void ReleaseAnchor(string anchorId)
        {
            var anchor = anchors.Find(a => a.AnchorId == anchorId);
            if (anchor != null)
            {
                ReleaseAnchor(anchor);
            }
        }

        /// <summary>
        /// Release an anchor
        /// </summary>
        private void ReleaseAnchor(ARAnchorData anchor)
        {
            if (anchor.GameObject != null)
            {
                Destroy(anchor.GameObject);
            }

            anchor.IsActive = false;
            anchors.Remove(anchor);
            activeAnchorCount = anchors.Count;

            OnAnchorReleased?.Invoke(anchor);
        }

        /// <summary>
        /// Release the oldest anchor
        /// </summary>
        private void ReleaseOldestAnchor()
        {
            if (anchors.Count == 0) return;

            ARAnchorData oldest = anchors[0];
            float oldestTime = oldest.CreatedTime;

            foreach (var anchor in anchors)
            {
                if (anchor.CreatedTime < oldestTime)
                {
                    oldest = anchor;
                    oldestTime = anchor.CreatedTime;
                }
            }

            ReleaseAnchor(oldest);
        }

        /// <summary>
        /// Cleanup loop for expired anchors
        /// Referência: AR-020
        /// </summary>
        private IEnumerator CleanupLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(cleanupInterval);

                if (Time.time - lastCleanupTime >= cleanupInterval)
                {
                    CleanupExpiredAnchors();
                    lastCleanupTime = Time.time;
                }
            }
        }

        /// <summary>
        /// Cleanup anchors that have exceeded their lifetime
        /// </summary>
        private void CleanupExpiredAnchors()
        {
            float currentTime = Time.time;
            var expiredAnchors = new List<ARAnchorData>();

            foreach (var anchor in anchors)
            {
                if (currentTime - anchor.CreatedTime > anchorLifetime)
                {
                    expiredAnchors.Add(anchor);
                }
            }

            foreach (var expired in expiredAnchors)
            {
                Debug.Log($"[RiftAnchor] Releasing expired anchor: {expired.AnchorId}");
                ReleaseAnchor(expired);
            }

            if (expiredAnchors.Count > 0)
            {
                Debug.Log($"[RiftAnchor] Cleaned up {expiredAnchors.Count} expired anchors");
            }
        }

        /// <summary>
        /// Get all active anchors
        /// </summary>
        public List<ARAnchorData> GetActiveAnchors()
        {
            return new List<ARAnchorData>(anchors);
        }

        /// <summary>
        /// Get anchor by ID
        /// </summary>
        public ARAnchorData GetAnchor(string anchorId)
        {
            return anchors.Find(a => a.AnchorId == anchorId);
        }

        /// <summary>
        /// Get anchor stats for debugging
        /// </summary>
        public string GetStats()
        {
            return $"Active: {activeAnchorCount}/{maxAnchors} | " +
                   $"Total Created: {totalAnchorsCreated} | " +
                   $"Lifetime: {anchorLifetime}s";
        }
    }

    /// <summary>
    /// AR anchor data
    /// </summary>
    [System.Serializable]
    public class ARAnchorData
    {
        public string AnchorId;
        public Vector3 Position;
        public Quaternion Rotation;
        public SurfaceType SurfaceType;
        public float CreatedTime;
        public bool IsActive;
        public GameObject GameObject;
    }
}
