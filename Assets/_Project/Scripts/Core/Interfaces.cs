using UnityEngine;

namespace BreachAR.Core
{
    /// <summary>
    /// Interface for objects that can receive damage
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float amount);
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
    }

    /// <summary>
    /// Interface for orb behaviors (Strategy Pattern)
    /// </summary>
    public interface IOrbBehaviour
    {
        void OnHit(IDamageable target);
        void OnRicochet();
        void OnExpire();
        float Damage { get; }
        int MaxRicochets { get; }
        float DamageFalloffPerBounce { get; }
    }

    /// <summary>
    /// Interface for pathfinding agents (Fragments)
    /// </summary>
    public interface IPathfindingAgent
    {
        void SetDestination(Vector3 destination);
        void UpdatePath();
        bool HasReachedDestination { get; }
    }

    /// <summary>
    /// Interface for AR surface provider (for testing without real AR)
    /// </summary>
    public interface IARSurfaceProvider
    {
        ScannedSurface[] GetSurfaces();
        bool IsScanComplete { get; }
        float ScanProgress { get; }
    }

    /// <summary>
    /// Represents a scanned surface in AR
    /// </summary>
    public enum SurfaceType
    {
        Floor,
        Ceiling,
        Wall,
        Furniture
    }

    /// <summary>
    /// Data structure for scanned surfaces
    /// </summary>
    [System.Serializable]
    public class ScannedSurface
    {
        public SurfaceType Type;
        public Vector3 Center;
        public Vector3 Normal;
        public float Area;
        public Bounds Bounds;
    }

    /// <summary>
    /// Interface for game state management
    /// </summary>
    public interface IGameState
    {
        void Enter();
        void Exit();
        void Update();
    }

    /// <summary>
    /// Interface for save repository
    /// </summary>
    public interface ISaveRepository
    {
        SaveData Load();
        void Save(SaveData data);
        bool HasSave();
        void DeleteSave();
    }

    /// <summary>
    /// Save data structure
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        public string PlayerId;
        public int Level;
        public float Experience;
        public int SoftCurrency;
        public int HardCurrency;
        public OrbUpgradeData[] OrbUpgrades;
        public string[] UnlockedOrbs;
        public string[] UnlockedSkins;
        public GameSettings Settings;
        public long LastSaveTimestamp;
    }

    /// <summary>
    /// Orb upgrade data
    /// </summary>
    [System.Serializable]
    public class OrbUpgradeData
    {
        public string OrbId;
        public int DamageLevel;
        public int SpeedLevel;
        public int AreaLevel;
        public int ElementalLevel;
    }

    /// <summary>
    /// Game settings
    /// </summary>
    [System.Serializable]
    public class GameSettings
    {
        public float MasterVolume = 1f;
        public float MusicVolume = 0.7f;
        public float SFXVolume = 1f;
        public int GraphicsQuality = 2; // 0=Low, 1=Medium, 2=High, 3=Auto
        public bool ReducedParticles = false;
        public bool AdvancedOcclusion = true;
        public float DragSensitivity = 1f;
        public bool ShowTrajectory = true;
        public bool HighContrastHUD = false;
        public float FontScale = 1f;
        public bool ReducedShake = false;
    }
}
