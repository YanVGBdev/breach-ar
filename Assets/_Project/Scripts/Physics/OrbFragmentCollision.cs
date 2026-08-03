using UnityEngine;
using BreachAR.Core;
using BreachAR.Gameplay;
using VContainer;

namespace BreachAR.Physics
{
    /// <summary>
    /// Handles collision detection and resolution between Orbs and Fragments
    /// </summary>
    public class OrbFragmentCollision : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask orbLayer;
        [SerializeField] private LayerMask fragmentLayer;
        [SerializeField] private float collisionCheckRadius = 0.5f;
        [SerializeField] private float damageRadius = 1.0f;

        [Header("Feedback")]
        [SerializeField] private GameObject impactVfxPrefab;
        [SerializeField] private float impactForceMultiplier = 5f;

        private PoolManager poolManager;
        private ScoreSystem scoreSystem;
        private ComboSystem comboSystem;

        [Inject]
        public void Construct(PoolManager pool, ScoreSystem score, ComboSystem combo)
        {
            poolManager = pool;
            scoreSystem = score;
            comboSystem = combo;
        }

        private void FixedUpdate()
        {
            CheckCollisions();
        }

        /// <summary>
        /// Check for collisions between orbs and fragments
        /// </summary>
        private void CheckCollisions()
        {
            // Find all orbs
            Collider[] orbColliders = Physics.OverlapSphere(
                transform.position, 
                100f, // Large radius to catch all orbs
                orbLayer
            );

            foreach (var orbCollider in orbColliders)
            {
                OrbController orb = orbCollider.GetComponent<OrbController>();
                if (orb == null || !orb.IsActive) continue;

                // Check for fragment collisions near this orb
                Collider[] fragmentColliders = Physics.OverlapSphere(
                    orb.transform.position,
                    collisionCheckRadius,
                    fragmentLayer
                );

                foreach (var fragmentCollider in fragmentColliders)
                {
                    FragmentController fragment = fragmentCollider.GetComponent<FragmentController>();
                    if (fragment == null || !fragment.IsActive) continue;

                    // Calculate distance
                    float distance = Vector3.Distance(
                        orb.transform.position,
                        fragment.transform.position
                    );

                    if (distance < collisionCheckRadius)
                    {
                        HandleOrbFragmentCollision(orb, fragment);
                    }
                }
            }
        }

        /// <summary>
        /// Handle collision between orb and fragment
        /// </summary>
        private void HandleOrbFragmentCollision(OrbController orb, FragmentController fragment)
        {
            // Calculate damage based on orb power and combo
            float baseDamage = orb.GetDamage();
            float comboMultiplier = comboSystem != null ? comboSystem.Multiplier : 1f;
            float totalDamage = baseDamage * comboMultiplier;

            // Apply damage to fragment
            fragment.TakeDamage(totalDamage);

            // Spawn impact VFX
            SpawnImpactVfx(orb.transform.position, fragment.transform.position);

            // Apply physics force to fragment
            Rigidbody fragmentRb = fragment.GetComponent<Rigidbody>();
            if (fragmentRb != null)
            {
                Vector3 forceDirection = (fragment.transform.position - orb.transform.position).normalized;
                fragmentRb.AddForce(forceDirection * impactForceMultiplier, ForceMode.Impulse);
            }

            // Register hit for scoring
            if (scoreSystem != null)
            {
                scoreSystem.RegisterHit(HitType.OrbHit, totalDamage);
            }

            // Notify orb of collision
            orb.OnHitTarget();

            Debug.Log($"[Collision] Orb hit Fragment for {totalDamage:F1} damage (combo: {comboMultiplier:F1}x)");
        }

        /// <summary>
        /// Spawn impact visual effect
        /// </summary>
        private void SpawnImpactVfx(Vector3 orbPosition, Vector3 fragmentPosition)
        {
            if (impactVfxPrefab == null || poolManager == null) return;

            Vector3 midpoint = Vector3.Lerp(orbPosition, fragmentPosition, 0.5f);
            GameObject vfx = poolManager.Get("VFX", midpoint, Quaternion.identity);
            
            if (vfx != null)
            {
                // Auto-return to pool after animation
                PoolAutoReturn autoReturn = vfx.GetComponent<PoolAutoReturn>();
                if (autoReturn == null)
                {
                    autoReturn = vfx.AddComponent<PoolAutoReturn>();
                }
                autoReturn.Initialize(poolManager, "VFX", 2f);
            }
        }

        /// <summary>
        /// Visualize collision radius in editor
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, collisionCheckRadius);
        }
    }

    /// <summary>
    /// Auto-return pooled object after delay
    /// </summary>
    public class PoolAutoReturn : MonoBehaviour
    {
        private PoolManager poolManager;
        private string poolTag;
        private float returnDelay;
        private float spawnTime;

        public void Initialize(PoolManager pool, string tag, float delay)
        {
            poolManager = pool;
            poolTag = tag;
            returnDelay = delay;
            spawnTime = Time.time;
        }

        private void Update()
        {
            if (Time.time - spawnTime >= returnDelay)
            {
                if (poolManager != null)
                {
                    poolManager.Return(poolTag, gameObject);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
