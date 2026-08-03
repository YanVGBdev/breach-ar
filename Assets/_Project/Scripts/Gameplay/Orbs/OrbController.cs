using UnityEngine;
using BreachAR.Core;
using BreachAR.ScriptableObjects;
using BreachAR.Utils;
using VContainer;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Controls orb behavior - flight, collision, damage, and expiration
    /// Referência: GP-038, specs/OrbLaunch.md
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class OrbController : MonoBehaviour, IOrbBehaviour
    {
        [Header("Configuration")]
        [SerializeField] private OrbDefinitionSO orbDefinition;

        [Header("Pool Settings")]
        [SerializeField] private string poolTag = "Orb";

        [Header("Runtime Settings")]
        [SerializeField] private int currentRicochetCount;
        [SerializeField] private float currentDamage;
        [SerializeField] private bool isActive;

        private Rigidbody rb;
        private float spawnTime;
        private int upgradeLevel;
        private PoolManager poolManager;

        public OrbDefinitionSO OrbDefinition => orbDefinition;
        public float Damage => currentDamage;
        public int MaxRicochets => orbDefinition != null ? orbDefinition.MaxRicochets : 3;
        public float DamageFalloffPerBounce => orbDefinition != null ? orbDefinition.DamageFalloffPerBounce : 0.1f;
        public bool IsActive => isActive;

        [Inject]
        public void Construct(PoolManager pool)
        {
            poolManager = pool;
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// Initialize orb with definition and upgrade level
        /// </summary>
        public void Initialize(OrbDefinitionSO definition, int level = 0)
        {
            orbDefinition = definition;
            upgradeLevel = level;

            if (definition != null)
            {
                currentDamage = definition.GetDamageAtLevel(level);
                rb.mass = definition.Mass;
                rb.useGravity = true;
                Physics.gravity = new Vector3(0, -9.81f * definition.GravityScale, 0);
            }

            currentRicochetCount = 0;
            spawnTime = Time.time;
            isActive = true;
        }

        /// <summary>
        /// Reset state for pool reuse
        /// Referência: 99_agent_rules.md - Regra de limpeza para pooling
        /// </summary>
        public void ResetState()
        {
            isActive = false;
            currentRicochetCount = 0;
            currentDamage = 0f;
            upgradeLevel = 0;
            
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }
            
            CancelInvoke();
        }

        /// <summary>
        /// Launch the orb with force
        /// </summary>
        public void Launch(Vector3 direction, float force)
        {
            if (!isActive) return;

            rb.isKinematic = false;
            rb.AddForce(direction * force, ForceMode.VelocityChange);

            // Emit launch event
            GameEvents.OnOrbLaunched?.Invoke(new OrbLaunchData
            {
                OrbId = gameObject.name,
                Direction = direction,
                Force = force
            });
        }

        private void FixedUpdate()
        {
            if (!isActive) return;

            // Check for expiration based on ricochets or time
            if (currentRicochetCount >= MaxRicochets)
            {
                Expire();
                return;
            }

            // Auto-expire after 10 seconds
            if (Time.time - spawnTime > 10f)
            {
                Expire();
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!isActive) return;

            // Handle ricochets
            if (collision.gameObject.CompareTag("RealWorldSurface"))
            {
                OnRicochet();
                return;
            }

            // Handle damage to damageable objects
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
            if (damageable != null)
            {
                OnHit(damageable, collision.GetContact(0).point);
            }
        }

        /// <summary>
        /// Handle hitting a damageable target
        /// </summary>
        public void OnHit(IDamageable target, Vector3 hitPosition)
        {
            if (target == null || !target.IsAlive) return;

            target.TakeDamage(currentDamage);

            // Emit hit event
            GameEvents.OnOrbHit?.Invoke(new OrbHitData
            {
                OrbId = gameObject.name,
                HitPosition = hitPosition,
                TargetId = target.ToString(),
                IsRift = target is RiftController,
                IsFragment = target is FragmentController,
                IsCore = target is CoreController
            });

            Expire(); // Orb is consumed on hit
        }

        /// <summary>
        /// Handle ricochet off surface
        /// </summary>
        public void OnRicochet()
        {
            currentRicochetCount++;
            currentDamage *= (1f - DamageFalloffPerBounce);

            // Emit ricochet event
            GameEvents.OnOrbRicochet?.Invoke(new OrbRicochetData
            {
                OrbId = gameObject.name,
                Position = transform.position,
                RicochetCount = currentRicochetCount
            });

            // Play ricochet sound
            if (orbDefinition != null && orbDefinition.RicochetSound != null)
            {
                AudioSource.PlayClipAtPoint(orbDefinition.RicochetSound, transform.position);
            }
        }

        /// <summary>
        /// Expire the orb (dissipate)
        /// </summary>
        public void OnExpire()
        {
            Expire();
        }

        private void Expire()
        {
            if (!isActive) return;

            isActive = false;
            rb.isKinematic = true;

            // Emit expire event
            GameEvents.OnOrbExpired?.Invoke(new OrbExpiredData
            {
                OrbId = gameObject.name,
                Reason = currentRicochetCount >= MaxRicochets ? "MaxRicochets" : "Timeout"
            });

            // Play expiration effect
            // TODO: Spawn VFX from pool

            // Return to pool after delay
            Invoke(nameof(ReturnToPool), 0.5f);
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

        /// <summary>
        /// Activate area damage if applicable
        /// </summary>
        public void ActivateAreaDamage(Vector3 center)
        {
            if (orbDefinition == null || !orbDefinition.HasAreaDamage) return;

            float radius = orbDefinition.GetAreaRadiusAtLevel(upgradeLevel);
            float areaDamage = currentDamage * orbDefinition.AreaDamageMultiplier;

            Collider[] colliders = Physics.OverlapSphere(center, radius);
            foreach (Collider col in colliders)
            {
                IDamageable damageable = col.GetComponent<IDamageable>();
                if (damageable != null && col.gameObject != gameObject)
                {
                    damageable.TakeDamage(areaDamage);
                }
            }
        }
    }

    /// <summary>
    /// Data for orb launched event
    /// Referência: specs/OrbLaunch.md
    /// </summary>
    [System.Serializable]
    public struct OrbLaunchData
    {
        public string OrbId;
        public Vector3 Direction;
        public float Force;
    }

    /// <summary>
    /// Data for orb ricochet event
    /// Referência: specs/OrbLaunch.md
    /// </summary>
    [System.Serializable]
    public struct OrbRicochetData
    {
        public string OrbId;
        public Vector3 Position;
        public int RicochetCount;
    }

    /// <summary>
    /// Data for orb expired event
    /// Referência: specs/OrbLaunch.md
    /// </summary>
    [System.Serializable]
    public struct OrbExpiredData
    {
        public string OrbId;
        public string Reason;
    }
}
