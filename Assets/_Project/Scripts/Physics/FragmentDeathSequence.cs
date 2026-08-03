using UnityEngine;
using BreachAR.Core;
using VContainer;

namespace BreachAR.Physics
{
    /// <summary>
    /// Handles Fragment death sequence with dissolve effect and pool return
    /// </summary>
    public class FragmentDeathSequence : MonoBehaviour
    {
        [Header("Dissolve Settings")]
        [SerializeField] private float dissolveDuration = 0.5f;
        [SerializeField] private AnimationCurve dissolveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private string dissolvePropertyName = "_DissolveAmount";

        [Header("Effects")]
        [SerializeField] private GameObject deathVfxPrefab;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private float deathSoundVolume = 0.7f;

        [Header("Pool Settings")]
        [SerializeField] private string poolTag = "Fragment";
        [SerializeField] private float returnDelay = 1f;

        private PoolManager poolManager;
        private Renderer[] renderers;
        private MaterialPropertyBlock propertyBlock;
        private float dissolveAmount;
        private bool isDissolving;
        private float dissolveStartTime;

        [Inject]
        public void Construct(PoolManager pool)
        {
            poolManager = pool;
        }

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>();
            propertyBlock = new MaterialPropertyBlock();
        }

        /// <summary>
        /// Start death sequence
        /// </summary>
        public void StartDeathSequence()
        {
            if (isDissolving) return;

            isDissolving = true;
            dissolveStartTime = Time.time;

            // Spawn death VFX
            SpawnDeathVfx();

            // Play death sound
            PlayDeathSound();

            // Disable collider to prevent further interactions
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            // Disable FragmentController
            FragmentController controller = GetComponent<FragmentController>();
            if (controller != null)
            {
                controller.enabled = false;
            }
        }

        private void Update()
        {
            if (!isDissolving) return;

            float elapsed = Time.time - dissolveStartTime;
            float progress = Mathf.Clamp01(elapsed / dissolveDuration);
            dissolveAmount = dissolveCurve.Evaluate(progress);

            // Update dissolve on all renderers
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;

                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(dissolvePropertyName, dissolveAmount);
                renderer.SetPropertyBlock(propertyBlock);
            }

            // Check if dissolve complete
            if (progress >= 1f)
            {
                ReturnToPool();
            }
        }

        /// <summary>
        /// Spawn death visual effect
        /// </summary>
        private void SpawnDeathVfx()
        {
            if (deathVfxPrefab == null || poolManager == null) return;

            GameObject vfx = poolManager.Get("VFX", transform.position, Quaternion.identity);
            if (vfx != null)
            {
                // Auto-return VFX after 2 seconds
                PoolAutoReturn autoReturn = vfx.GetComponent<PoolAutoReturn>();
                if (autoReturn == null)
                {
                    autoReturn = vfx.AddComponent<PoolAutoReturn>();
                }
                autoReturn.Initialize(poolManager, "VFX", 2f);
            }
        }

        /// <summary>
        /// Play death sound effect
        /// </summary>
        private void PlayDeathSound()
        {
            if (deathSound == null) return;

            // Play at position with spatial audio
            AudioSource.PlayClipAtPoint(deathSound, transform.position, deathSoundVolume);
        }

        /// <summary>
        /// Return fragment to pool
        /// </summary>
        private void ReturnToPool()
        {
            isDissolving = false;
            dissolveAmount = 0f;

            // Reset material properties
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;

                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(dissolvePropertyName, 0f);
                renderer.SetPropertyBlock(propertyBlock);
            }

            // Re-enable components
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }

            FragmentController controller = GetComponent<FragmentController>();
            if (controller != null)
            {
                controller.enabled = true;
            }

            // Return to pool
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
        /// Reset death sequence state
        /// </summary>
        public void ResetState()
        {
            isDissolving = false;
            dissolveAmount = 0f;
            dissolveStartTime = 0f;

            // Reset material properties
            foreach (var renderer in renderers)
            {
                if (renderer == null) continue;

                renderer.GetPropertyBlock(propertyBlock);
                propertyBlock.SetFloat(dissolvePropertyName, 0f);
                renderer.SetPropertyBlock(propertyBlock);
            }

            // Re-enable components
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }

            FragmentController controller = GetComponent<FragmentController>();
            if (controller != null)
            {
                controller.enabled = true;
            }
        }
    }
}
