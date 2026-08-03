using UnityEngine;
using System;
using System.Collections.Generic;

namespace BreachAR.Core
{
    /// <summary>
    /// Static event dispatcher for game events
    /// Referência: specs/RiftSystem.md - Eventos Emitidos
    /// </summary>
    public static class GameEvents
    {
        // =====================================================================
        // Gameplay Events
        // =====================================================================
        
        /// <summary>Raised when a rift is spawned</summary>
        public static Action<RiftData> OnRiftSpawned;
        
        /// <summary>Raised when a rift takes damage</summary>
        public static Action<RiftDamagedData> OnRiftDamaged;
        
        /// <summary>Raised when a rift is closed</summary>
        public static Action<RiftClosedData> OnRiftClosed;
        
        /// <summary>Raised when a fragment spawn is requested</summary>
        public static Action<FragmentSpawnRequestData> OnFragmentSpawnRequested;
        
        /// <summary>Raised when a fragment is killed</summary>
        public static Action<FragmentKilledData> OnFragmentKilled;
        
        /// <summary>Raised when the core is damaged</summary>
        public static Action<CoreDamagedData> OnCoreDamaged;
        
        /// <summary>Raised when combo changes</summary>
        public static Action<ComboChangedData> OnComboChanged;
        
        /// <summary>Raised when a wave starts</summary>
        public static Action<WaveStartedData> OnWaveStarted;
        
        /// <summary>Raised when a wave is completed</summary>
        public static Action<WaveCompletedData> OnWaveCompleted;
        
        /// <summary>Raised when score changes</summary>
        public static Action<ScoreChangedData> OnScoreChanged;
        
        /// <summary>Raised when a power-up is collected</summary>
        public static Action<PowerUpCollectedData> OnPowerUpCollected;
        
        /// <summary>Raised when a boss is defeated</summary>
        public static Action<BossDefeatedData> OnBossDefeated;
        
        /// <summary>Raised on game over</summary>
        public static Action<GameOverData> OnGameOver;
        
        /// <summary>Raised when an orb hits a rift</summary>
        public static Action<OrbHitData> OnOrbHit;
        
        // =====================================================================
        // AR Events
        // =====================================================================
        
        /// <summary>Raised when a surface is detected</summary>
        public static Action<SurfaceDetectedData> OnSurfaceDetected;
        
        /// <summary>Raised when a surface is lost</summary>
        public static Action<SurfaceLostData> OnSurfaceLost;
        
        /// <summary>Raised when scan is complete</summary>
        public static Action<ScanCompleteData> OnScanComplete;
        
        /// <summary>Raised when an anchor is created</summary>
        public static Action<AnchorCreatedData> OnAnchorCreated;
        
        /// <summary>Raised when difficulty changes</summary>
        public static Action<DifficultyChangedData> OnDifficultyChanged;
        
        // =====================================================================
        // UI Events
        // =====================================================================
        
        /// <summary>Raised when a menu is opened</summary>
        public static Action<MenuOpenedData> OnMenuOpened;
        
        /// <summary>Raised when a menu is closed</summary>
        public static Action<MenuClosedData> OnMenuClosed;
        
        /// <summary>Raised when pause is toggled</summary>
        public static Action<PauseToggledData> OnPauseToggled;
        
        // =====================================================================
        // System Events
        // =====================================================================
        
        /// <summary>Raised when settings change</summary>
        public static Action<SettingsChangedData> OnSettingsChanged;
        
        /// <summary>Raised when save completes</summary>
        public static Action<SaveCompletedData> OnSaveCompleted;
        
        /// <summary>Raised when load completes</summary>
        public static Action<LoadCompletedData> OnLoadCompleted;
        
        /// <summary>
        /// Clear all events (call on scene unload)
        /// </summary>
        public static void ClearAll()
        {
            // Gameplay
            OnRiftSpawned = null;
            OnRiftDamaged = null;
            OnRiftClosed = null;
            OnFragmentSpawnRequested = null;
            OnFragmentKilled = null;
            OnCoreDamaged = null;
            OnComboChanged = null;
            OnWaveStarted = null;
            OnWaveCompleted = null;
            OnScoreChanged = null;
            OnPowerUpCollected = null;
            OnBossDefeated = null;
            OnGameOver = null;
            OnOrbHit = null;
            
            // AR
            OnSurfaceDetected = null;
            OnSurfaceLost = null;
            OnScanComplete = null;
            OnAnchorCreated = null;
            OnDifficultyChanged = null;
            
            // UI
            OnMenuOpened = null;
            OnMenuClosed = null;
            OnPauseToggled = null;
            
            // System
            OnSettingsChanged = null;
            OnSaveCompleted = null;
            OnLoadCompleted = null;
        }
    }

    // =====================================================================
    // Event Data Structures
    // =====================================================================

    /// <summary>
    /// Data for rift spawned event
    /// Referência: specs/RiftSystem.md
    /// </summary>
    [System.Serializable]
    public struct RiftData
    {
        public string RiftId;
        public SurfaceType SurfaceType;
        public Vector3 Position;
        public Quaternion Rotation;
    }

    /// <summary>
    /// Data for rift damaged event
    /// Referência: specs/RiftSystem.md
    /// </summary>
    [System.Serializable]
    public struct RiftDamagedData
    {
        public string RiftId;
        public float Amount;
        public float CurrentIntegrity;
    }

    /// <summary>
    /// Data for rift closed event
    /// Referência: specs/RiftSystem.md
    /// </summary>
    [System.Serializable]
    public struct RiftClosedData
    {
        public string RiftId;
        public SurfaceType SurfaceType;
        public Vector3 Position;
    }

    /// <summary>
    /// Data for fragment spawn request
    /// Referência: specs/RiftSystem.md
    /// </summary>
    [System.Serializable]
    public struct FragmentSpawnRequestData
    {
        public ScriptableObjects.FragmentDefinitionSO FragmentDefinition;
        public Vector3 SpawnPosition;
        public Quaternion SpawnRotation;
        public string RiftId;
    }

    /// <summary>
    /// Data for fragment killed event
    /// </summary>
    [System.Serializable]
    public struct FragmentKilledData
    {
        public string FragmentId;
        public FragmentType FragmentType;
        public string OrbId;
        public float ComboMultiplier;
        public bool ViaRicochet;
        public Vector3 Position;
    }

    /// <summary>
    /// Data for core damaged event
    /// </summary>
    [System.Serializable]
    public struct CoreDamagedData
    {
        public float DamageAmount;
        public float CurrentHealth;
        public float MaxHealth;
        public string SourceFragmentId;
    }

    /// <summary>
    /// Data for combo changed event
    /// </summary>
    [System.Serializable]
    public struct ComboChangedData
    {
        public float Multiplier;
        public int ComboCount;
        public bool WasReset;
    }

    /// <summary>
    /// Data for wave started event
    /// </summary>
    [System.Serializable]
    public struct WaveStartedData
    {
        public int WaveIndex;
        public int TotalWaves;
        public bool IsBossWave;
    }

    /// <summary>
    /// Data for wave completed event
    /// </summary>
    [System.Serializable]
    public struct WaveCompletedData
    {
        public int WaveIndex;
        public float TimeTaken;
        public float CoreHpRemaining;
        public bool PerfectWave;
    }

    /// <summary>
    /// Data for score changed event
    /// </summary>
    [System.Serializable]
    public struct ScoreChangedData
    {
        public int NewScore;
        public int ScoreDelta;
        public string Reason;
    }

    /// <summary>
    /// Data for power-up collected event
    /// </summary>
    [System.Serializable]
    public struct PowerUpCollectedData
    {
        public string PowerUpId;
        public PowerUpType Type;
        public float Duration;
    }

    /// <summary>
    /// Data for boss defeated event
    /// </summary>
    [System.Serializable]
    public struct BossDefeatedData
    {
        public string BossId;
        public float TimeTaken;
        public int FinalScore;
    }

    /// <summary>
    /// Data for game over event
    /// </summary>
    [System.Serializable]
    public struct GameOverData
    {
        public bool Victory;
        public int FinalScore;
        public int WavesCleared;
        public float MaxCombo;
        public int FragmentsKilled;
        public int RiftsClosed;
    }

    /// <summary>
    /// Data for orb hit event
    /// </summary>
    [System.Serializable]
    public struct OrbHitData
    {
        public string OrbId;
        public Vector3 HitPosition;
        public string TargetId;
        public bool IsRift;
        public bool IsFragment;
        public bool IsCore;
    }

    /// <summary>
    /// Data for menu opened event
    /// </summary>
    [System.Serializable]
    public struct MenuOpenedData
    {
        public string MenuName;
    }

    /// <summary>
    /// Data for menu closed event
    /// </summary>
    [System.Serializable]
    public struct MenuClosedData
    {
        public string MenuName;
    }

    /// <summary>
    /// Data for pause toggled event
    /// </summary>
    [System.Serializable]
    public struct PauseToggledData
    {
        public bool IsPaused;
    }

    /// <summary>
    /// Data for settings changed event
    /// </summary>
    [System.Serializable]
    public struct SettingsChangedData
    {
        public string SettingName;
    }

    /// <summary>
    /// Data for save completed event
    /// </summary>
    [System.Serializable]
    public struct SaveCompletedData
    {
        public bool Success;
    }

    /// <summary>
    /// Data for load completed event
    /// </summary>
    [System.Serializable]
    public struct LoadCompletedData
    {
        public bool Success;
    }

    // =====================================================================
    // AR Event Data Structures
    // =====================================================================

    /// <summary>
    /// Data for surface detected event
    /// Referência: specs/ARSurfaceService.md
    /// </summary>
    [System.Serializable]
    public struct SurfaceDetectedData
    {
        public string SurfaceId;
        public SurfaceType Type;
        public float Area;
        public Vector3 Position;
    }

    /// <summary>
    /// Data for surface lost event
    /// Referência: specs/ARSurfaceService.md
    /// </summary>
    [System.Serializable]
    public struct SurfaceLostData
    {
        public string SurfaceId;
    }

    /// <summary>
    /// Data for scan complete event
    /// Referência: specs/ARSurfaceService.md
    /// </summary>
    [System.Serializable]
    public struct ScanCompleteData
    {
        public int SurfaceCount;
        public float Duration;
        public bool HasFloor;
        public bool HasWall;
    }

    /// <summary>
    /// Data for anchor created event
    /// Referência: specs/ARSurfaceService.md
    /// </summary>
    [System.Serializable]
    public struct AnchorCreatedData
    {
        public string AnchorId;
        public Vector3 Position;
        public string SurfaceId;
    }

    /// <summary>
    /// Data for difficulty changed event
    /// Referência: specs/DifficultyDirector.md
    /// </summary>
    [System.Serializable]
    public struct DifficultyChangedData
    {
        public int PreviousLevel;
        public int NewLevel;
        public string Reason;
    }

    // =====================================================================
    // Enums
    // =====================================================================

    public enum FragmentType
    {
        Basic,
        Fast,
        Tanky,
        Splitter,
        Shielded,
        Healer
    }

    public enum PowerUpType
    {
        Shield,
        Multiball,
        SlowTime,
        Piercing,
        Explosive,
        Magnet
    }

    public enum SurfaceType
    {
        Wall,
        Floor,
        Ceiling,
        Furniture,
        Other
    }

    public enum GameState
    {
        MainMenu,
        Scanning,
        Placement,
        Playing,
        Paused,
        GameOver
    }

    public enum GameMode
    {
        Campaign,
        Endless,
        DailyChallenge,
        Zen,
        MultiplayerAsync
    }
}
