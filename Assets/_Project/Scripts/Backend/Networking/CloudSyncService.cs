using UnityEngine;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.Backend
{
    /// <summary>
    /// Manages cloud save synchronization
    /// </summary>
    public class CloudSyncService : MonoBehaviour
    {

        [Header("Configuration")]
        [SerializeField] private float syncInterval = 300f; // 5 minutes
        [SerializeField] private float debounceDelay = 5f;
        [SerializeField] private bool enableAutoSync = true;

        private SaveData pendingSyncData;
        private float lastSyncTime;
        private bool isSyncing;
        private bool hasPendingChanges;



        private void Update()
        {
            if (!enableAutoSync) return;

            // Auto-sync periodically
            if (hasPendingChanges && !isSyncing && 
                Time.time - lastSyncTime > syncInterval)
            {
                SyncToCloud();
            }
        }

        /// <summary>
        /// Queue data for sync
        /// </summary>
        public void QueueSync(SaveData data)
        {
            pendingSyncData = data;
            hasPendingChanges = true;

            // Debounce - cancel previous pending sync
            CancelInvoke(nameof(SyncToCloud));
            if (enableAutoSync)
            {
                Invoke(nameof(SyncToCloud), debounceDelay);
            }
        }

        /// <summary>
        /// Sync data to cloud
        /// </summary>
        public void SyncToCloud()
        {
            if (isSyncing || !hasPendingChanges || pendingSyncData == null)
                return;

            Debug.Log("[CloudSync] Starting sync to cloud");
            isSyncing = true;

            // TODO: Implement actual cloud sync
            // StartCoroutine(SyncCoroutine());
            
            // For now, simulate sync
            SimulateSync();
        }

        private void SimulateSync()
        {
            // Simulate network delay
            Invoke(nameof(OnSyncComplete), 1f);
        }

        private void OnSyncComplete()
        {
            isSyncing = false;
            hasPendingChanges = false;
            lastSyncTime = Time.time;
            Debug.Log("[CloudSync] Sync completed");
        }

        /// <summary>
        /// Sync data from cloud
        /// </summary>
        public void SyncFromCloud()
        {
            if (isSyncing) return;

            Debug.Log("[CloudSync] Fetching from cloud");
            isSyncing = true;

            // TODO: Implement actual cloud fetch
            // StartCoroutine(FetchCoroutine());
            
            // For now, simulate
            Invoke(nameof(OnFetchComplete), 1f);
        }

        private void OnFetchComplete()
        {
            isSyncing = false;
            Debug.Log("[CloudSync] Fetch completed");
        }

        /// <summary>
        /// Resolve conflict between local and cloud data
        /// </summary>
        public SaveData ResolveConflict(SaveData localData, SaveData cloudData)
        {
            if (localData == null) return cloudData;
            if (cloudData == null) return localData;

            // Last-write-wins for most fields
            SaveData resolved = localData.LastSaveTimestamp > cloudData.LastSaveTimestamp 
                ? localData 
                : cloudData;

            // For cumulative values (currency, XP), take the higher value
            resolved.SoftCurrency = Mathf.Max(localData.SoftCurrency, cloudData.SoftCurrency);
            resolved.HardCurrency = Mathf.Max(localData.HardCurrency, cloudData.HardCurrency);
            resolved.Experience = Mathf.Max(localData.Experience, cloudData.Experience);
            resolved.Level = Mathf.Max(localData.Level, cloudData.Level);

            Debug.Log("[CloudSync] Conflict resolved");
            return resolved;
        }

        /// <summary>
        /// Force sync now
        /// </summary>
        public void ForceSync()
        {
            SyncToCloud();
        }

        /// <summary>
        /// Check if sync is in progress
        /// </summary>
        public bool IsSyncing()
        {
            return isSyncing;
        }
    }
}
