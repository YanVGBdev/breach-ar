using UnityEngine;
using BreachAR.Core;
using BreachAR.ScriptableObjects;
using BreachAR.Utils;
using VContainer;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Controls power-up behavior - spawning, collection, and effects
    /// Referência: GP-038, GP-016
    /// </summary>
    public class PowerUpController : MonoBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private PowerUpDefinitionSO powerUpDefinition;

        [Header("Pool Settings")]
        [SerializeField] private string poolTag = "PowerUp";

        [Header("State")]
        [SerializeField] private float lifetime;
        [SerializeField] private bool isActive;

        private float spawnTime;
        private Rigidbody rb;
        private PowerUpEffect activeEffect;
        private PoolManager poolManager;
        private ScoreSystem scoreSystem;

        public PowerUpDefinitionSO PowerUpDefinition => powerUpDefinition;
        public bool IsActive => isActive;

        [Inject]
        public void Construct(PoolManager pool, ScoreSystem score)
        {
            poolManager = pool;
            scoreSystem = score;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Initialize power-up
        /// </summary>
        public void Initialize(PowerUpDefinitionSO definition)
        {
            powerUpDefinition = definition;
            spawnTime = Time.time;
            isActive = true;

            if (definition != null)
            {
                lifetime = definition.Lifetime;
            }

            // Setup physics for floating fall
            if (rb != null)
            {
                rb.useGravity = true;
                rb.mass = 0.1f;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }

        /// <summary>
        /// Reset state for pool reuse
        /// Referência: 99_agent_rules.md - Regra de limpeza para pooling
        /// </summary>
        public void ResetState()
        {
            isActive = false;
            lifetime = 0f;
            activeEffect = null;
            
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
            
            CancelInvoke();
        }

        private void Update()
        {
            if (!isActive) return;

            // Check expiration
            if (Time.time - spawnTime > lifetime)
            {
                Expire();
            }

            // Floating animation - slow down falling
            if (rb != null && rb.linearVelocity.y < -0.1f)
            {
                rb.linearVelocity *= 0.98f;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!isActive) return;

            // Check if collected by orb or player
            if (other.CompareTag("Orb") || other.CompareTag("Player"))
            {
                Collect(other.gameObject);
            }
        }

        /// <summary>
        /// Collect the power-up
        /// </summary>
        public void Collect(GameObject collector)
        {
            if (!isActive) return;

            isActive = false;

            // Add score for collection
            scoreSystem?.AddPowerUpScore(powerUpDefinition.PowerUpId);

            // Create and apply effect
            CreateEffect();

            // Emit event
            GameEvents.OnPowerUpCollected?.Invoke(new PowerUpCollectedData
            {
                PowerUpId = powerUpDefinition.PowerUpId,
                Type = powerUpDefinition.Type,
                Duration = powerUpDefinition.ActiveDuration
            });

            Debug.Log($"[PowerUp] Collected: {powerUpDefinition.DisplayName}");

            // Visual feedback
            PlayCollectionEffect();

            // Return to pool
            Invoke(nameof(ReturnToPool), 0.5f);
        }

        /// <summary>
        /// Create the power-up effect
        /// </summary>
        private void CreateEffect()
        {
            if (powerUpDefinition == null) return;

            switch (powerUpDefinition.Type)
            {
                case PowerUpType.MultipleOrb:
                    activeEffect = new MultipleOrbEffect(powerUpDefinition);
                    break;
                case PowerUpType.TemporalRift:
                    activeEffect = new TemporalRiftEffect(powerUpDefinition);
                    break;
                case PowerUpType.CoreShield:
                    activeEffect = new CoreShieldEffect(powerUpDefinition);
                    break;
                case PowerUpType.Overcharge:
                    activeEffect = new OverchargeEffect(powerUpDefinition);
                    break;
                case PowerUpType.EnergyMagnet:
                    activeEffect = new EnergyMagnetEffect(powerUpDefinition);
                    break;
            }

            activeEffect?.Apply();
        }

        /// <summary>
        /// Play collection visual/audio effect
        /// </summary>
        private void PlayCollectionEffect()
        {
            if (powerUpDefinition.CollectEffectPrefab != null)
            {
                // Use pool if available
                if (poolManager != null && poolManager.HasPool("VFX"))
                {
                    GameObject vfx = poolManager.Get("VFX", transform.position, Quaternion.identity);
                    if (vfx != null)
                    {
                        var particle = vfx.GetComponent<ParticleSystem>();
                        if (particle != null)
                        {
                            var main = particle.main;
                            main.startColor = powerUpDefinition.PowerUpColor;
                            particle.Play();
                        }
                        // VFX will handle its own return to pool
                    }
                }
                else
                {
                    // Fallback to instantiate if no pool
                    var vfx = Instantiate(powerUpDefinition.CollectEffectPrefab, transform.position, Quaternion.identity);
                    Destroy(vfx, 2f);
                }
            }

            if (powerUpDefinition.CollectSound != null)
            {
                AudioSource.PlayClipAtPoint(powerUpDefinition.CollectSound, transform.position);
            }
        }

        /// <summary>
        /// Expire the power-up
        /// </summary>
        private void Expire()
        {
            if (!isActive) return;

            isActive = false;
            Debug.Log($"[PowerUp] Expired: {powerUpDefinition?.DisplayName}");
            
            ReturnToPool();
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
    /// Base class for power-up effects
    /// </summary>
    public abstract class PowerUpEffect
    {
        protected PowerUpDefinitionSO definition;
        protected float duration;

        public PowerUpEffect(PowerUpDefinitionSO def)
        {
            definition = def;
            duration = def.ActiveDuration;
        }

        public abstract void Apply();
        public virtual void Remove() { }
        public virtual void Update() { }
    }

    /// <summary>
    /// Multiple Orb effect - next 3 launches fire 3 orbs
    /// Referência: GP-017
    /// </summary>
    public class MultipleOrbEffect : PowerUpEffect
    {
        private int remainingShots = 3;

        public MultipleOrbEffect(PowerUpDefinitionSO def) : base(def) { }

        public override void Apply()
        {
            Debug.Log("[PowerUp] Multiple Orb activated - 3 shots remaining");
            // Would integrate with LaunchSystem
        }
    }

    /// <summary>
    /// Temporal Rift effect - slow all fragments
    /// Referência: GP-018
    /// </summary>
    public class TemporalRiftEffect : PowerUpEffect
    {
        public TemporalRiftEffect(PowerUpDefinitionSO def) : base(def) { }

        public override void Apply()
        {
            Debug.Log($"[PowerUp] Temporal Rift activated - {duration}s slow");
            // Would find all FragmentControllers and apply slow
        }

        public override void Remove()
        {
            Debug.Log("[PowerUp] Temporal Rift ended");
        }
    }

    /// <summary>
    /// Core Shield effect - absorb impacts
    /// Referência: GP-019
    /// </summary>
    public class CoreShieldEffect : PowerUpEffect
    {
        private int shieldHits = 3;

        public CoreShieldEffect(PowerUpDefinitionSO def) : base(def) { }

        public override void Apply()
        {
            Debug.Log($"[PowerUp] Core Shield activated - {shieldHits} hits");
            // Would set CoreController invulnerable for N hits
        }
    }

    /// <summary>
    /// Overcharge effect - next orb causes area damage
    /// Referência: GP-020
    /// </summary>
    public class OverchargeEffect : PowerUpEffect
    {
        public OverchargeEffect(PowerUpDefinitionSO def) : base(def) { }

        public override void Apply()
        {
            Debug.Log("[PowerUp] Overcharge activated - next orb explodes");
            // Would flag next OrbController for area damage
        }
    }

    /// <summary>
    /// Energy Magnet effect - orbs attract fragments
    /// Referência: GP-021
    /// </summary>
    public class EnergyMagnetEffect : PowerUpEffect
    {
        public EnergyMagnetEffect(PowerUpDefinitionSO def) : base(def) { }

        public override void Apply()
        {
            Debug.Log($"[PowerUp] Energy Magnet activated - {duration}s");
            // Would modify orb physics to attract fragments
        }

        public override void Remove()
        {
            Debug.Log("[PowerUp] Energy Magnet ended");
        }
    }

    /// <summary>
    /// Power-up types
    /// </summary>
    public enum PowerUpType
    {
        MultipleOrb,    // Next 3 launches fire 3 orbs
        TemporalRift,   // Slow all fragments by 50% for 8s
        CoreShield,     // Absorb next 3 impacts
        Overcharge,     // Next orb causes area damage
        EnergyMagnet    // Orbs attract nearby fragments
    }

    /// <summary>
    /// ScriptableObject for power-up definition
    /// Referência: GP-016
    /// </summary>
    [CreateAssetMenu(fileName = "NewPowerUp", menuName = "BreachAR/PowerUps/PowerUpDefinition")]
    public class PowerUpDefinitionSO : ScriptableObject
    {
        [Header("Identity")]
        public string PowerUpId;
        public string DisplayName;
        [TextArea(2, 4)]
        public string Description;
        public Sprite Icon;

        [Header("Gameplay")]
        public PowerUpType Type;
        public float ActiveDuration = 8f;
        public float Lifetime = 15f;
        [Range(0f, 1f)]
        public float SpawnChance = 0.1f;

        [Header("Visual")]
        public Color PowerUpColor = Color.yellow;
        public GameObject Prefab;
        public GameObject CollectEffectPrefab;
        public GameObject ActiveEffectPrefab;

        [Header("Audio")]
        public AudioClip CollectSound;
        public AudioClip ActiveSound;
    }
}
