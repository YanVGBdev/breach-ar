using UnityEngine;
using BreachAR.Core;
using BreachAR.Gameplay;
using System.Collections.Generic;
using VContainer;

namespace BreachAR.AI
{
    /// <summary>
    /// Decides where and when new rifts appear
    /// Referência: specs/RiftSystem.md - RiftSpawnDirector
    /// </summary>
    public class RiftSpawnDirector : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private RiftSpawnConfig config;

        [Header("References")]
        [SerializeField] private Transform riftParent;

        [Inject] private ARSessionService arSessionService;
        [Inject] private DifficultyDirector difficultyDirector;

        private List<RiftController> activeRifts = new List<RiftController>();
        private float lastSpawnTime;
        private int riftIdCounter;

        private void OnEnable()
        {
            GameEvents.OnRiftClosed += HandleRiftClosed;
        }

        private void OnDisable()
        {
            GameEvents.OnRiftClosed -= HandleRiftClosed;
        }

        private void Start()
        {
            lastSpawnTime = Time.time;
        }

        /// <summary>
        /// Activate rift spawning for a new wave
        /// </summary>
        public void ActivateForWave(WaveDefinitionSO wave)
        {
            if (config == null || config.riftDefinitions.Length == 0) return;

            // Spawn initial rifts
            int riftsToSpawn = Mathf.Min(config.initialRiftsPerWave, config.maxActiveRifts);
            for (int i = 0; i < riftsToSpawn; i++)
            {
                SpawnRift();
            }
        }

        /// <summary>
        /// Deactivate rift spawning (end of wave)
        /// </summary>
        public void Deactivate()
        {
            // Close all active rifts
            foreach (var rift in activeRifts)
            {
                if (rift != null && rift.IsActive)
                {
                    rift.ForceClose();
                }
            }
            activeRifts.Clear();
        }

        private void Update()
        {
            if (config == null) return;

            // Check if we should spawn a new rift
            float spawnCooldown = config.GetSpawnCooldown(difficultyDirector.CurrentDifficulty);
            if (Time.time - lastSpawnTime >= spawnCooldown &&
                activeRifts.Count < config.maxActiveRifts)
            {
                SpawnRift();
                lastSpawnTime = Time.time;
            }
        }

        private void SpawnRift()
        {
            // Get valid AR surface
            var surface = arSessionService.GetRandomValidSurface();
            if (surface == null)
            {
                Debug.LogWarning("[RiftSpawnDirector] No valid AR surface found");
                return;
            }

            // Select random rift definition
            RiftDefinitionSO riftDef = config.GetRandomRiftDefinition();
            if (riftDef == null || riftDef.riftPrefab == null) return;

            // Calculate spawn position on surface
            Vector3 spawnPosition = surface.position;
            Quaternion spawnRotation = surface.rotation;

            // Add some randomness within surface bounds
            spawnPosition += surface.transform.right * Random.Range(-0.3f, 0.3f);
            spawnPosition += surface.transform.up * Random.Range(-0.3f, 0.3f);

            // Instantiate rift
            GameObject riftObj = Instantiate(riftDef.riftPrefab, spawnPosition, spawnRotation, riftParent);
            riftObj.name = $"Rift_{riftIdCounter++}";

            // Initialize rift controller
            RiftController riftController = riftObj.GetComponent<RiftController>();
            if (riftController != null)
            {
                int difficulty = difficultyDirector.CurrentDifficulty;
                riftController.Initialize(
                    riftDef,
                    surface.SurfaceType,
                    difficulty
                );

                activeRifts.Add(riftController);
            }

            Debug.Log($"[RiftSpawnDirector] Spawned rift at {spawnPosition}");
        }

        private void HandleRiftClosed(RiftClosedData data)
        {
            // Remove from active list
            activeRifts.RemoveAll(r => r == null || !r.IsActive);
        }

        /// <summary>
        /// Get number of active rifts
        /// </summary>
        public int ActiveRiftCount => activeRifts.Count;
    }

    /// <summary>
    /// Configuration for rift spawning behavior
    /// </summary>
    [CreateAssetMenu(fileName = "RiftSpawnConfig", menuName = "BreachAR/Rift Spawn Config")]
    public class RiftSpawnConfig : ScriptableObject
    {
        [Header("Rift Definitions")]
        [Tooltip("Available rift types for spawning")]
        public RiftDefinitionSO[] riftDefinitions;

        [Header("Spawn Limits")]
        [Tooltip("Maximum active rifts at once")]
        public int maxActiveRifts = 5;

        [Tooltip("Initial rifts spawned per wave")]
        public int initialRiftsPerWave = 2;

        [Header("Spawn Cooldown")]
        [Tooltip("Base cooldown between rift spawns (seconds)")]
        public float baseSpawnCooldown = 15f;

        [Tooltip("Minimum cooldown between spawns (seconds)")]
        public float minSpawnCooldown = 5f;

        [Tooltip("Cooldown reduction per difficulty level")]
        public float cooldownDifficultyReduction = 1f;

        /// <summary>
        /// Get spawn cooldown adjusted for difficulty
        /// </summary>
        public float GetSpawnCooldown(int difficultyLevel)
        {
            float cooldown = baseSpawnCooldown - (cooldownDifficultyReduction * difficultyLevel);
            return Mathf.Max(minSpawnCooldown, cooldown);
        }

        /// <summary>
        /// Get a random rift definition
        /// </summary>
        public RiftDefinitionSO GetRandomRiftDefinition()
        {
            if (riftDefinitions == null || riftDefinitions.Length == 0)
                return null;

            return riftDefinitions[Random.Range(0, riftDefinitions.Length)];
        }
    }
}
