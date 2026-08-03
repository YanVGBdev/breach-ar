using UnityEngine;

namespace BreachAR.ScriptableObjects
{
    /// <summary>
    /// Configuration data for a rift type
    /// Referência: specs/RiftSystem.md - RiftDefinitionSO
    /// </summary>
    [CreateAssetMenu(fileName = "RiftDefinition", menuName = "BreachAR/Rift Definition")]
    public class RiftDefinitionSO : ScriptableObject
    {
        [Header("Rift Identity")]
        [Tooltip("Unique identifier for this rift type")]
        public string riftId;
        
        [Tooltip("Display name for UI")]
        public string displayName;
        
        [Tooltip("Description for tooltips")]
        [TextArea(2, 4)]
        public string description;

        [Header("Integrity")]
        [Tooltip("Base integrity (health) of the rift")]
        public float baseIntegrity = 100f;
        
        [Tooltip("Integrity multiplier per difficulty level")]
        public float integrityDifficultyMultiplier = 1.1f;

        [Header("Spawning")]
        [Tooltip("Base interval between fragment spawns (seconds)")]
        public float baseSpawnInterval = 3f;
        
        [Tooltip("Minimum spawn interval (seconds)")]
        public float minSpawnInterval = 1f;
        
        [Tooltip("Spawn interval reduction per difficulty level")]
        public float spawnIntervalDifficultyReduction = 0.1f;

        [Header("Fragments")]
        [Tooltip("Fragment types that can spawn from this rift")]
        public FragmentDefinitionSO[] spawnableFragments;
        
        [Tooltip("Max fragments alive at once from this rift")]
        public int maxConcurrentFragments = 5;

        [Header("Visual")]
        [Tooltip("Prefab for the rift visual")]
        public GameObject riftPrefab;
        
        [Tooltip("Closing animation duration (seconds)")]
        public float closingDuration = 1f;
        
        [Tooltip("Color tint for this rift type")]
        public Color riftColor = Color.magenta;
        
        [Tooltip("Particle effect when rift spawns")]
        public GameObject spawnVFX;
        
        [Tooltip("Particle effect when rift closes")]
        public GameObject closeVFX;

        [Header("Audio")]
        [Tooltip("Sound when rift spawns")]
        public AudioClip spawnSFX;
        
        [Tooltip("Sound when rift is damaged")]
        public AudioClip damageSFX;
        
        [Tooltip("Sound when rift closes")]
        public AudioClip closeSFX;

        [Header("Surface Types")]
        [Tooltip("Which surface types this rift can spawn on")]
        public SurfaceType[] allowedSurfaceTypes = new SurfaceType[] 
        { 
            SurfaceType.Wall, 
            SurfaceType.Floor, 
            SurfaceType.Ceiling 
        };

        /// <summary>
        /// Get spawn interval adjusted for difficulty
        /// </summary>
        public float GetSpawnInterval(int difficultyLevel)
        {
            float interval = baseSpawnInterval - (spawnIntervalDifficultyReduction * difficultyLevel);
            return Mathf.Max(minSpawnInterval, interval);
        }

        /// <summary>
        /// Get integrity adjusted for difficulty
        /// </summary>
        public float GetIntegrity(int difficultyLevel)
        {
            return baseIntegrity * Mathf.Pow(integrityDifficultyMultiplier, difficultyLevel);
        }

        /// <summary>
        /// Check if this rift can spawn on given surface type
        /// </summary>
        public bool CanSpawnOnSurface(SurfaceType surfaceType)
        {
            foreach (var allowed in allowedSurfaceTypes)
            {
                if (allowed == surfaceType)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Get a random spawnable fragment
        /// </summary>
        public FragmentDefinitionSO GetRandomFragment()
        {
            if (spawnableFragments == null || spawnableFragments.Length == 0)
                return null;
            
            return spawnableFragments[Random.Range(0, spawnableFragments.Length)];
        }
    }

    /// <summary>
    /// Surface types for AR rift placement
    /// </summary>
    public enum SurfaceType
    {
        Wall,
        Floor,
        Ceiling,
        Furniture,
        Other
    }
}
