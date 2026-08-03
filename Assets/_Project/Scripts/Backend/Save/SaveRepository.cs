using UnityEngine;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace BreachAR.Backend
{
    /// <summary>
    /// Local save repository with encryption
    /// Referência: BK-004 - ISaveRepository local criptografado
    /// </summary>
    public class SaveRepository : ISaveRepository
    {
        private const string SaveFileName = "breachar_save.dat";
        private const string EncryptionKey = "BreachAR_2026_SecureKey"; // In production, use secure storage
        
        private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

        /// <summary>
        /// Load save data from local storage
        /// </summary>
        public SaveData Load()
        {
            try
            {
                if (!File.Exists(SavePath))
                {
                    Debug.Log("[SaveRepository] No save file found");
                    return null;
                }

                string encryptedData = File.ReadAllText(SavePath);
                string jsonData = Decrypt(encryptedData);
                SaveData data = JsonUtility.FromJson<SaveData>(jsonData);
                
                Debug.Log("[SaveRepository] Save loaded successfully");
                return data;
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveRepository] Failed to load: {e.Message}");
                return null;
            }
        }

        /// <summary>
        /// Save data to local storage with encryption
        /// </summary>
        public void Save(SaveData data)
        {
            try
            {
                data.LastSaveTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                
                string jsonData = JsonUtility.ToJson(data, true);
                string encryptedData = Encrypt(jsonData);
                
                File.WriteAllText(SavePath, encryptedData);
                Debug.Log("[SaveRepository] Save completed");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveRepository] Failed to save: {e.Message}");
            }
        }

        /// <summary>
        /// Delete save file
        /// </summary>
        public void DeleteSave()
        {
            try
            {
                if (File.Exists(SavePath))
                {
                    File.Delete(SavePath);
                    Debug.Log("[SaveRepository] Save deleted");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[SaveRepository] Failed to delete: {e.Message}");
            }
        }

        /// <summary>
        /// Check if save exists
        /// </summary>
        public bool HasSave()
        {
            return File.Exists(SavePath);
        }

        /// <summary>
        /// Encrypt string using XOR cipher (simple, not cryptographically secure)
        /// In production, use AES or similar
        /// </summary>
        private string Encrypt(string data)
        {
            byte[] key = Encoding.UTF8.GetBytes(EncryptionKey);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            byte[] encrypted = new byte[dataBytes.Length];

            for (int i = 0; i < dataBytes.Length; i++)
            {
                encrypted[i] = (byte)(dataBytes[i] ^ key[i % key.Length]);
            }

            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypt string
        /// </summary>
        private string Decrypt(string encryptedData)
        {
            byte[] key = Encoding.UTF8.GetBytes(EncryptionKey);
            byte[] dataBytes = Convert.FromBase64String(encryptedData);
            byte[] decrypted = new byte[dataBytes.Length];

            for (int i = 0; i < dataBytes.Length; i++)
            {
                decrypted[i] = (byte)(dataBytes[i] ^ key[i % key.Length]);
            }

            return Encoding.UTF8.GetString(decrypted);
        }
    }

    /// <summary>
    /// Interface for save repository
    /// </summary>
    public interface ISaveRepository
    {
        SaveData Load();
        void Save(SaveData data);
        void DeleteSave();
        bool HasSave();
    }

    /// <summary>
    /// Main save data container
    /// </summary>
    [Serializable]
    public class SaveData
    {
        [Header("Player Identity")]
        public string PlayerId;
        public int Level;
        public float Experience;

        [Header("Economy")]
        public int SoftCurrency;
        public int HardCurrency;

        [Header("Progression")]
        public OrbUpgradeData[] OrbUpgrades;
        public string[] UnlockedOrbs;
        public string[] UnlockedSkins;
        public int HighestWaveReached;
        public int TotalFragmentsKilled;
        public float HighScore;

        [Header("Settings")]
        public GameSettings Settings;

        [Header("Privacy")]
        public bool AnalyticsConsent;
        public bool AdConsent;

        [Header("Timestamps")]
        public long LastSaveTimestamp;
        public long FirstPlayTimestamp;
    }

    /// <summary>
    /// Orb upgrade data
    /// </summary>
    [Serializable]
    public struct OrbUpgradeData
    {
        public string OrbId;
        public int Level;
        public int DamageUpgrades;
        public int SpeedUpgrades;
        public int AreaUpgrades;
    }

    /// <summary>
    /// Game settings
    /// </summary>
    [Serializable]
    public class GameSettings
    {
        [Header("Audio")]
        public float MasterVolume = 1f;
        public float MusicVolume = 0.8f;
        public float SFXVolume = 1f;

        [Header("Graphics")]
        public int QualityLevel = 1; // 0=Low, 1=Medium, 2=High
        public bool ShowFPS = false;

        [Header("Controls")]
        public float Sensitivity = 1f;
        public bool HapticFeedback = true;

        [Header("Accessibility")]
        public bool HighContrast = false;
        public float FontScale = 1f;
        public bool ReduceShake = false;
    }
}
