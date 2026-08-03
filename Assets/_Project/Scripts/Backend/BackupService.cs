using UnityEngine;
using System;
using System.Collections;
using System.IO;
using VContainer;

namespace BreachAR.Backend
{
    /// <summary>
    /// Handles automatic periodic backups of player data
    /// Referência: BK-022
    /// </summary>
    public class BackupService : MonoBehaviour
    {
        [Header("Backup Configuration")]
        [SerializeField] private float backupIntervalHours = 24f;
        [SerializeField] private int maxBackups = 7;
        [SerializeField] private bool enableAutoBackup = true;
        [SerializeField] private bool enableCloudBackup = true;

        [Header("State")]
        [SerializeField] private float lastBackupTime;
        [SerializeField] private int backupCount;
        [SerializeField] private bool isBackingUp;

        [Inject] private SupabaseService supabaseService;

        private string backupFolderPath;
        private DateTime lastBackupDateTime;

        public bool IsBackingUp => isBackingUp;
        public int BackupCount => backupCount;
        public DateTime LastBackupTime => lastBackupDateTime;

        /// <summary>
        /// Event raised when backup completes
        /// </summary>
        public event Action<BackupResult> OnBackupCompleted;

        private void Awake()
        {
            backupFolderPath = Path.Combine(Application.persistentDataPath, "backups");
            Directory.CreateDirectory(backupFolderPath);

            LoadBackupState();
        }

        private void Start()
        {
            if (enableAutoBackup)
            {
                StartCoroutine(AutoBackupCoroutine());
            }
        }

        /// <summary>
        /// Auto backup coroutine
        /// Referência: BK-022
        /// </summary>
        private IEnumerator AutoBackupCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(backupIntervalHours * 3600f);

                if (enableAutoBackup && !isBackingUp)
                {
                    yield return PerformBackup();
                }
            }
        }

        /// <summary>
        /// Perform a backup
        /// Referência: BK-022
        /// </summary>
        public IEnumerator PerformBackup()
        {
            if (isBackingUp)
            {
                Debug.LogWarning("[Backup] Backup already in progress");
                yield break;
            }

            isBackingUp = true;
            Debug.Log("[Backup] Starting backup...");

            var result = new BackupResult
            {
                Timestamp = DateTime.UtcNow,
                Success = false
            };

            try
            {
                // Create local backup
                string backupFile = CreateLocalBackup();
                result.LocalBackupPath = backupFile;
                result.Success = true;

                // Sync to cloud if enabled
                if (enableCloudBackup && supabaseService != null && supabaseService.IsAuthenticated)
                {
                    yield return SyncBackupToCloud(backupFile);
                    result.CloudSynced = true;
                }

                // Update state
                lastBackupTime = Time.time;
                lastBackupDateTime = DateTime.UtcNow;
                backupCount++;

                // Clean old backups
                CleanOldBackups();

                // Save state
                SaveBackupState();

                Debug.Log($"[Backup] Backup completed successfully: {backupFile}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[Backup] Backup failed: {e.Message}");
                result.ErrorMessage = e.Message;
            }

            isBackingUp = false;
            OnBackupCompleted?.Invoke(result);
        }

        /// <summary>
        /// Create local backup file
        /// </summary>
        private string CreateLocalBackup()
        {
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
            string backupFile = Path.Combine(backupFolderPath, $"backup_{timestamp}.json");

            // Read current save data
            string savePath = Application.persistentDataPath + "/save.json";
            if (File.Exists(savePath))
            {
                string saveData = File.ReadAllText(savePath);
                File.WriteAllText(backupFile, saveData);
            }
            else
            {
                // Create empty backup
                File.WriteAllText(backupFile, "{}");
            }

            return backupFile;
        }

        /// <summary>
        /// Sync backup to cloud storage
        /// Referência: BK-022
        /// </summary>
        private IEnumerator SyncBackupToCloud(string backupFile)
        {
            if (!File.Exists(backupFile))
            {
                Debug.LogError("[Backup] Backup file not found");
                yield break;
            }

            string backupData = File.ReadAllText(backupFile);
            var data = new System.Collections.Generic.Dictionary<string, object>
            {
                { "user_id", supabaseService.CurrentUserId },
                { "backup_data", backupData },
                { "timestamp", DateTime.UtcNow.ToString("o") },
                { "device_info", SystemInfo.deviceModel }
            };

            yield return supabaseService.SaveData("backups", null, data);
        }

        /// <summary>
        /// Restore from backup
        /// Referência: BK-022
        /// </summary>
        public bool RestoreFromBackup(string backupPath)
        {
            if (!File.Exists(backupPath))
            {
                Debug.LogError("[Backup] Backup file not found");
                return false;
            }

            try
            {
                string backupData = File.ReadAllText(backupPath);
                string savePath = Application.persistentDataPath + "/save.json";

                // Create backup of current save before restoring
                if (File.Exists(savePath))
                {
                    string preRestoreBackup = Path.Combine(backupFolderPath, 
                        $"pre_restore_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
                    File.Copy(savePath, preRestoreBackup);
                }

                File.WriteAllText(savePath, backupData);

                Debug.Log($"[Backup] Restored from: {backupPath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[Backup] Restore failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get list of available backups
        /// </summary>
        public BackupInfo[] GetAvailableBackups()
        {
            if (!Directory.Exists(backupFolderPath))
                return Array.Empty<BackupInfo>();

            string[] files = Directory.GetFiles(backupFolderPath, "backup_*.json");
            var backups = new BackupInfo[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                var fileInfo = new FileInfo(files[i]);
                backups[i] = new BackupInfo
                {
                    Path = files[i],
                    FileName = fileInfo.Name,
                    CreatedAt = fileInfo.CreationTimeUtc,
                    SizeBytes = fileInfo.Length
                };
            }

            // Sort by creation time (newest first)
            Array.Sort(backups, (a, b) => b.CreatedAt.CompareTo(a.CreatedAt));

            return backups;
        }

        /// <summary>
        /// Clean old backups beyond retention limit
        /// Referência: BK-022
        /// </summary>
        private void CleanOldBackups()
        {
            var backups = GetAvailableBackups();

            if (backups.Length <= maxBackups)
                return;

            // Delete oldest backups
            for (int i = maxBackups; i < backups.Length; i++)
            {
                try
                {
                    File.Delete(backups[i].Path);
                    Debug.Log($"[Backup] Deleted old backup: {backups[i].FileName}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[Backup] Failed to delete: {e.Message}");
                }
            }
        }

        /// <summary>
        /// Save backup state to PlayerPrefs
        /// </summary>
        private void SaveBackupState()
        {
            PlayerPrefs.SetFloat("backup_last_time", lastBackupTime);
            PlayerPrefs.SetInt("backup_count", backupCount);
            PlayerPrefs.SetString("backup_last_datetime", lastBackupDateTime.ToString("o"));
            PlayerPrefs.Save();
        }

        /// <summary>
        /// Load backup state from PlayerPrefs
        /// </summary>
        private void LoadBackupState()
        {
            lastBackupTime = PlayerPrefs.GetFloat("backup_last_time", 0);
            backupCount = PlayerPrefs.GetInt("backup_count", 0);
            string datetimeStr = PlayerPrefs.GetString("backup_last_datetime", "");

            if (!string.IsNullOrEmpty(datetimeStr) && DateTime.TryParse(datetimeStr, out DateTime dt))
            {
                lastBackupDateTime = dt;
            }
        }

        /// <summary>
        /// Get backup status for debugging
        /// </summary>
        public string GetBackupStatus()
        {
            return $"=== Backup Status ===\n" +
                   $"Auto Backup: {enableAutoBackup}\n" +
                   $"Cloud Backup: {enableCloudBackup}\n" +
                   $"Interval: {backupIntervalHours}h\n" +
                   $"Max Backups: {maxBackups}\n" +
                   $"Backup Count: {backupCount}\n" +
                   $"Last Backup: {(backupCount > 0 ? lastBackupDateTime.ToString("o") : "Never")}\n" +
                   $"Currently Backing Up: {isBackingUp}";
        }
    }

    /// <summary>
    /// Backup result data
    /// </summary>
    [System.Serializable]
    public class BackupResult
    {
        public DateTime Timestamp;
        public bool Success;
        public string LocalBackupPath;
        public bool CloudSynced;
        public string ErrorMessage;
    }

    /// <summary>
    /// Backup info for listing
    /// </summary>
    [System.Serializable]
    public class BackupInfo
    {
        public string Path;
        public string FileName;
        public DateTime CreatedAt;
        public long SizeBytes;
    }
}
