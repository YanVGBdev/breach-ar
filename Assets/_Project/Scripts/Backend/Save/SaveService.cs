using UnityEngine;
using System;
using BreachAR.Core;

namespace BreachAR.Backend
{
    /// <summary>
    /// Manages save/load operations for game data
    /// Injected via VContainer DI
    /// </summary>
    public class SaveService : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private float autoSaveInterval = 60f;
        [SerializeField] private bool encryptSave = true;

        private ISaveRepository localRepository;
        private SaveData currentSaveData;
        private float lastAutoSaveTime;
        private bool hasUnsavedChanges;

        public SaveData CurrentSaveData => currentSaveData;

        private void Start()
        {
            InitializeRepository();
            Load();
        }

        private void Update()
        {
            if (hasUnsavedChanges && Time.time - lastAutoSaveTime > autoSaveInterval)
            {
                Save();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus && hasUnsavedChanges)
            {
                Save();
            }
        }

        private void OnApplicationQuit()
        {
            if (hasUnsavedChanges)
            {
                Save();
            }
        }

        /// <summary>
        /// Initialize save repository
        /// </summary>
        private void InitializeRepository()
        {
            localRepository = new SaveRepository();
            Debug.Log("[SaveService] Repository initialized");
        }

        /// <summary>
        /// Load save data
        /// </summary>
        public void Load()
        {
            try
            {
                currentSaveData = localRepository.Load();
                
                if (currentSaveData == null)
                {
                    currentSaveData = CreateDefaultSave();
                }

                Debug.Log("[SaveService] Save loaded successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] Failed to load: {e.Message}");
                currentSaveData = CreateDefaultSave();
            }
        }

        /// <summary>
        /// Save current data
        /// </summary>
        public void Save()
        {
            try
            {
                if (currentSaveData == null)
                {
                    Debug.LogWarning("[SaveService] No data to save");
                    return;
                }

                localRepository.Save(currentSaveData);
                
                hasUnsavedChanges = false;
                lastAutoSaveTime = Time.time;
                
                Debug.Log("[SaveService] Save completed");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveService] Failed to save: {e.Message}");
            }
        }

        /// <summary>
        /// Mark data as changed (triggers auto-save)
        /// </summary>
        public void MarkDirty()
        {
            hasUnsavedChanges = true;
        }

        /// <summary>
        /// Get player level
        /// </summary>
        public int GetLevel()
        {
            return currentSaveData?.Level ?? 1;
        }

        /// <summary>
        /// Add experience
        /// </summary>
        public void AddExperience(float amount)
        {
            if (currentSaveData == null) return;

            currentSaveData.Experience += amount;

            float expRequired = GetExperienceRequired(currentSaveData.Level);
            while (currentSaveData.Experience >= expRequired)
            {
                currentSaveData.Experience -= expRequired;
                currentSaveData.Level++;
                expRequired = GetExperienceRequired(currentSaveData.Level);
                Debug.Log($"[SaveService] Level up! Now level {currentSaveData.Level}");
            }

            MarkDirty();
        }

        /// <summary>
        /// Get experience required for a level
        /// </summary>
        public float GetExperienceRequired(int level)
        {
            return 100f * level;
        }

        /// <summary>
        /// Update settings
        /// </summary>
        public void UpdateSettings(GameSettings settings)
        {
            if (currentSaveData == null) return;

            currentSaveData.Settings = settings;
            MarkDirty();
        }

        /// <summary>
        /// Get orb upgrade data
        /// </summary>
        public OrbUpgradeData GetOrbUpgrade(string orbId)
        {
            if (currentSaveData?.OrbUpgrades == null) return null;

            foreach (var upgrade in currentSaveData.OrbUpgrades)
            {
                if (upgrade.OrbId == orbId)
                {
                    return upgrade;
                }
            }

            return null;
        }

        /// <summary>
        /// Set orb upgrade
        /// </summary>
        public void SetOrbUpgrade(OrbUpgradeData upgrade)
        {
            if (currentSaveData == null) return;

            bool found = false;
            for (int i = 0; i < currentSaveData.OrbUpgrades.Length; i++)
            {
                if (currentSaveData.OrbUpgrades[i].OrbId == upgrade.OrbId)
                {
                    currentSaveData.OrbUpgrades[i] = upgrade;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                var newArray = new OrbUpgradeData[currentSaveData.OrbUpgrades.Length + 1];
                Array.Copy(currentSaveData.OrbUpgrades, newArray, currentSaveData.OrbUpgrades.Length);
                newArray[newArray.Length - 1] = upgrade;
                currentSaveData.OrbUpgrades = newArray;
            }

            MarkDirty();
        }

        /// <summary>
        /// Delete save data
        /// </summary>
        public void DeleteSave()
        {
            localRepository.DeleteSave();
            currentSaveData = CreateDefaultSave();
            Debug.Log("[SaveService] Save deleted");
        }

        /// <summary>
        /// Create default save data
        /// </summary>
        private SaveData CreateDefaultSave()
        {
            return new SaveData
            {
                PlayerId = Guid.NewGuid().ToString(),
                Level = 1,
                Experience = 0f,
                SoftCurrency = 0,
                HardCurrency = 0,
                OrbUpgrades = new OrbUpgradeData[0],
                UnlockedOrbs = new string[] { "orb_basic" },
                UnlockedSkins = new string[0],
                Settings = new GameSettings(),
                LastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
        }
    }
}
