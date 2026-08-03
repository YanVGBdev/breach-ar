using UnityEngine;
using System.Collections.Generic;
using BreachAR.Core;

namespace BreachAR.Utils
{
    /// <summary>
    /// OPT-008: Otimiza atualizações de MeshColliders para evitar overhead de CPU.
    /// Throttle de atualizações baseado em distância e necessidade.
    /// </summary>
    public class MeshColliderOptimizer : MonoBehaviour
    {
        [Header("Throttle Settings")]
        [SerializeField] private float updateInterval = 0.5f;
        [SerializeField] private float distanceThreshold = 2f;
        [SerializeField] private int maxUpdatesPerFrame = 2;
        
        [Header("LOD Settings")]
        [SerializeField] private float lodDistanceNear = 5f;
        [SerializeField] private float lodDistanceFar = 15f;
        
        private List<MeshColliderEntry> entries = new List<MeshColliderEntry>();
        private float lastUpdateTime;
        private int updatesThisFrame;
        private Transform cameraTransform;
        
        private struct MeshColliderEntry
        {
            public MeshCollider Collider;
            public Mesh OriginalMesh;
            public float LastDistanceToCamera;
            public float LastUpdateTime;
            public bool NeedsUpdate;
        }
        
        private void Start()
        {
            cameraTransform = Camera.main?.transform;
        }
        
        private void LateUpdate()
        {
            if (Time.time - lastUpdateTime < updateInterval)
                return;
                
            updatesThisFrame = 0;
            lastUpdateTime = Time.time;
            
            if (cameraTransform == null)
                cameraTransform = Camera.main?.transform;
                
            if (cameraTransform == null)
                return;
            
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                if (entries[i].Collider == null)
                {
                    entries.RemoveAt(i);
                    continue;
                }
                
                if (updatesThisFrame >= maxUpdatesPerFrame)
                    break;
                
                UpdateEntry(i);
            }
        }
        
        private void UpdateEntry(int index)
        {
            var entry = entries[index];
            float distance = Vector3.Distance(
                cameraTransform.position, 
                entry.Collider.transform.position
            );
            
            // Only update if moved significantly
            if (Mathf.Abs(distance - entry.LastDistanceToCamera) < distanceThreshold)
                return;
            
            // Check if we should enable/disable based on distance
            if (distance > lodDistanceFar)
            {
                if (entry.Collider.enabled)
                {
                    entry.Collider.enabled = false;
                    entry.LastDistanceToCamera = distance;
                    entry.LastUpdateTime = Time.time;
                    entries[index] = entry;
                }
                return;
            }
            
            // Enable if disabled and within range
            if (!entry.Collider.enabled)
            {
                entry.Collider.enabled = true;
            }
            
            // Update mesh complexity based on distance
            UpdateMeshLOD(index, distance);
            
            entry.LastDistanceToCamera = distance;
            entry.LastUpdateTime = Time.time;
            entries[index] = entry;
            updatesThisFrame++;
        }
        
        private void UpdateMeshLOD(int index, float distance)
        {
            var entry = entries[index];
            
            // For distant objects, use simplified convex hull
            if (distance > lodDistanceNear && !entry.Collider.convex)
            {
                entry.Collider.convex = true;
                entries[index] = entry;
            }
            else if (distance <= lodDistanceNear && entry.Collider.convex)
            {
                entry.Collider.convex = false;
                entries[index] = entry;
            }
        }
        
        /// <summary>
        /// Register a MeshCollider for optimization.
        /// </summary>
        public void Register(MeshCollider collider)
        {
            if (collider == null) return;
            
            // Check if already registered
            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].Collider == collider)
                    return;
            }
            
            entries.Add(new MeshColliderEntry
            {
                Collider = collider,
                OriginalMesh = collider.sharedMesh,
                LastDistanceToCamera = float.MaxValue,
                LastUpdateTime = 0,
                NeedsUpdate = true
            });
        }
        
        /// <summary>
        /// Unregister a MeshCollider from optimization.
        /// </summary>
        public void Unregister(MeshCollider collider)
        {
            for (int i = entries.Count - 1; i >= 0; i--)
            {
                if (entries[i].Collider == collider)
                {
                    // Restore original settings
                    var entry = entries[i];
                    if (entry.Collider != null)
                    {
                        entry.Collider.convex = false;
                        entry.Collider.enabled = true;
                    }
                    entries.RemoveAt(i);
                    return;
                }
            }
        }
        
        /// <summary>
        /// Get statistics about managed colliders.
        /// </summary>
        public MeshColliderStats GetStats()
        {
            int enabled = 0;
            int convex = 0;
            
            foreach (var entry in entries)
            {
                if (entry.Collider != null && entry.Collider.enabled)
                    enabled++;
                if (entry.Collider != null && entry.Collider.convex)
                    convex++;
            }
            
            return new MeshColliderStats
            {
                Total = entries.Count,
                Enabled = enabled,
                Convex = convex
            };
        }
        
        private void OnDestroy()
        {
            // Restore all colliders
            foreach (var entry in entries)
            {
                if (entry.Collider != null)
                {
                    entry.Collider.convex = false;
                    entry.Collider.enabled = true;
                }
            }
            entries.Clear();
        }
    }
    
    public struct MeshColliderStats
    {
        public int Total;
        public int Enabled;
        public int Convex;
    }
}
