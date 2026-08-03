using UnityEngine;
using BreachAR.Core;
using BreachAR.ScriptableObjects;
using BreachAR.Utils;
using System.Collections;
using VContainer;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Controls rift behavior - integrity, fragment spawning, and closing
    /// Referência: GP-038, specs/RiftSystem.md
    /// </summary>
    public class RiftController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private RiftDefinitionSO riftDefinition;

        [Header("Pool Settings")]
        [SerializeField] private string poolTag = "Rift";

        [Header("State")]
        [SerializeField] private float currentIntegrity;
        [SerializeField] private SurfaceType surfaceType;
        [SerializeField] private bool isActive;
        [SerializeField] private bool isClosing;

        private float lastSpawnTime;
        private ARAnchor riftAnchor;
        private RiftState currentState;
        private int difficultyLevel;
        private PoolManager poolManager;

        public float CurrentIntegrity => currentIntegrity;
        public float MaxIntegrity => riftDefinition != null ? riftDefinition.baseIntegrity : 100f;
        public float IntegrityPercentage => currentIntegrity / MaxIntegrity;
        public SurfaceType SurfaceType => surfaceType;
        public bool IsActive => isActive;
        public RiftState CurrentState => currentState;

        [Inject]
        public void Construct(PoolManager pool)
        {
            poolManager = pool;
        }

        /// <summary>
        /// Initialize rift with definition and surface type
        /// </summary>
        public void Initialize(RiftDefinitionSO definition, SurfaceType type, int difficulty = 0)
        {
            riftDefinition = definition;
            surfaceType = type;
            difficultyLevel = difficulty;
            
            currentIntegrity = definition.GetIntegrity(difficulty);
            isActive = true;
            isClosing = false;
            currentState = RiftState.Open;
            lastSpawnTime = Time.time;

            // Reset scale
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Reset state for pool reuse
        /// Referência: 99_agent_rules.md - Regra de limpeza para pooling
        /// </summary>
        public void ResetState()
        {
            isActive = false;
            isClosing = false;
            currentIntegrity = 0f;
            currentState = RiftState.Closed;
            lastSpawnTime = 0f;
            difficultyLevel = 0;
            
            // Release anchor if exists
            if (riftAnchor != null)
            {
                Destroy(riftAnchor.gameObject);
                riftAnchor = null;
            }
            
            CancelInvoke();
            StopAllCoroutines();
        }

        private void Update()
        {
            if (!isActive || isClosing) return;

            // Spawn fragments at intervals
            float spawnInterval = riftDefinition.GetSpawnInterval(difficultyLevel);
            if (Time.time - lastSpawnTime >= spawnInterval)
            {
                SpawnFragment();
                lastSpawnTime = Time.time;
            }
        }

        /// <summary>
        /// Take damage to rift integrity
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (!isActive || isClosing || amount <= 0) return;

            currentIntegrity = Mathf.Max(0, currentIntegrity - amount);

            // Visual feedback for damage
            if (currentIntegrity < MaxIntegrity * 0.5f)
            {
                currentState = RiftState.Damaged;
            }

            // Emit damage event
            GameEvents.OnRiftDamaged?.Invoke(new RiftDamagedData
            {
                RiftId = gameObject.name,
                Amount = amount,
                CurrentIntegrity = currentIntegrity
            });

            // Check for closure
            if (currentIntegrity <= 0)
            {
                StartCoroutine(CloseRift());
            }
        }

        /// <summary>
        /// Spawn a fragment from this rift
        /// </summary>
        private void SpawnFragment()
        {
            if (riftDefinition == null) return;

            FragmentDefinitionSO fragmentType = riftDefinition.GetRandomFragment();
            if (fragmentType == null) return;

            // Calculate spawn position (slightly in front of rift)
            Vector3 spawnOffset = transform.forward * 0.5f;
            Vector3 spawnPosition = transform.position + spawnOffset;

            // Emit event for fragment spawning
            GameEvents.OnFragmentSpawnRequested?.Invoke(new FragmentSpawnRequestData
            {
                FragmentDefinition = fragmentType,
                SpawnPosition = spawnPosition,
                SpawnRotation = transform.rotation,
                RiftId = gameObject.name
            });
        }

        /// <summary>
        /// Force close the rift (e.g., end of wave)
        /// </summary>
        public void ForceClose()
        {
            if (!isActive || isClosing) return;
            StartCoroutine(CloseRift());
        }

        /// <summary>
        /// Close the rift with implosion effect
        /// </summary>
        private IEnumerator CloseRift()
        {
            if (isClosing) yield break;

            isClosing = true;
            currentState = RiftState.Closing;

            // Stop spawning
            isActive = false;

            // Play closing animation
            float duration = riftDefinition != null ? riftDefinition.closingDuration : 1f;
            float elapsed = 0f;
            Vector3 startScale = transform.localScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Scale down to zero
                transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);

                // Rotate for implosion effect
                transform.Rotate(Vector3.forward * 360f * Time.deltaTime);

                yield return null;
            }

            // Notify listeners
            GameEvents.OnRiftClosed?.Invoke(new RiftClosedData
            {
                RiftId = gameObject.name,
                SurfaceType = surfaceType,
                Position = transform.position
            });

            currentState = RiftState.Closed;
            
            // Return to pool
            ReturnToPool();
        }

        /// <summary>
        /// Set the AR anchor for this rift
        /// </summary>
        public void SetAnchor(ARAnchor anchor)
        {
            riftAnchor = anchor;
        }

        /// <summary>
        /// Get health percentage for UI
        /// </summary>
        public float GetIntegrityPercentage()
        {
            return currentIntegrity / MaxIntegrity;
        }

        private void ReturnToPool()
        {
            ResetState();
            
            if (poolManager != null)
            {
                poolManager.Return(poolTag, gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Rift states
    /// </summary>
    public enum RiftState
    {
        Open,
        Damaged,
        Closing,
        Closed
    }

    /// <summary>
    /// Placeholder AR anchor class
    /// </summary>
    public class ARAnchor : MonoBehaviour
    {
        // This will be replaced by AR Foundation's ARAnchor
    }
}
