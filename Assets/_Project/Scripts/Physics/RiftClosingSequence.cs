using UnityEngine;
using BreachAR.Core;
using VContainer;

namespace BreachAR.Physics
{
    /// <summary>
    /// Handles Rift closing sequence with implosion effect
    /// </summary>
    public class RiftClosingSequence : MonoBehaviour
    {
        [Header("Implosion Settings")]
        [SerializeField] private float implosionDuration = 1f;
        [SerializeField] private AnimationCurve implosionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        [SerializeField] private float implosionScaleMultiplier = 0.1f;

        [Header("Visual Effects")]
        [SerializeField] private ParticleSystem implosionParticles;
        [SerializeField] private Light riftLight;
        [SerializeField] private float lightIntensityFade = 2f;

        [Header("Audio")]
        [SerializeField] private AudioClip closingSound;
        [SerializeField] private float closingSoundVolume = 0.8f;

        [Header("Pool Settings")]
        [SerializeField] private string poolTag = "Rift";
        [SerializeField] private float returnDelay = 0.5f;

        private PoolManager poolManager;
        private RiftController riftController;
        private Vector3 originalScale;
        private float implosionStartTime;
        private bool isImpoding;
        private bool isAnchorReleased;

        [Inject]
        public void Construct(PoolManager pool)
        {
            poolManager = pool;
        }

        private void Awake()
        {
            riftController = GetComponent<RiftController>();
            originalScale = transform.localScale;
        }

        /// <summary>
        /// Start closing sequence
        /// </summary>
        public void StartClosingSequence()
        {
            if (isImpoding) return;

            isImpoding = true;
            implosionStartTime = Time.time;
            isAnchorReleased = false;

            // Play closing sound
            PlayClosingSound();

            // Play implosion particles
            if (implosionParticles != null)
            {
                implosionParticles.Play();
            }

            // Disable spawning
            if (riftController != null)
            {
                riftController.StopSpawning();
            }

            // Disable collider
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            Debug.Log($"[Rift] Closing sequence started: {gameObject.name}");
        }

        private void Update()
        {
            if (!isImpoding) return;

            float elapsed = Time.time - implosionStartTime;
            float progress = Mathf.Clamp01(elapsed / implosionDuration);
            float curveValue = implosionCurve.Evaluate(progress);

            // Scale down
            Vector3 newScale = Vector3.Lerp(originalScale, originalScale * implosionScaleMultiplier, curveValue);
            transform.localScale = newScale;

            // Fade light
            if (riftLight != null)
            {
                riftLight.intensity = Mathf.Lerp(riftLight.intensity, 0f, Time.deltaTime * lightIntensityFade);
            }

            // Check if implosion complete
            if (progress >= 1f)
            {
                OnImplosionComplete();
            }
        }

        /// <summary>
        /// Handle implosion completion
        /// </summary>
        private void OnImplosionComplete()
        {
            isImpoding = false;

            // Release anchor
            if (!isAnchorReleased)
            {
                ReleaseAnchor();
                isAnchorReleased = true;
            }

            // Return to pool
            StartCoroutine(ReturnToPoolAfterDelay());
        }

        /// <summary>
        /// Release AR anchor
        /// </summary>
        private void ReleaseAnchor()
        {
            // AR anchor release will be handled by ARSessionService
            // This is a hook for future integration
            Debug.Log($"[Rift] Anchor released: {gameObject.name}");
        }

        /// <summary>
        /// Return to pool after delay
        /// </summary>
        private System.Collections.IEnumerator ReturnToPoolAfterDelay()
        {
            yield return new WaitForSeconds(returnDelay);
            ReturnToPool();
        }

        /// <summary>
        /// Return rift to pool
        /// </summary>
        private void ReturnToPool()
        {
            // Reset state
            ResetState();

            // Return to pool
            if (poolManager != null)
            {
                poolManager.Return(poolTag, gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }

            Debug.Log($"[Rift] Returned to pool: {gameObject.name}");
        }

        /// <summary>
        /// Play closing sound
        /// </summary>
        private void PlayClosingSound()
        {
            if (closingSound == null) return;

            AudioSource.PlayClipAtPoint(closingSound, transform.position, closingSoundVolume);
        }

        /// <summary>
        /// Reset closing sequence state
        /// </summary>
        public void ResetState()
        {
            isImpoding = false;
            implosionStartTime = 0f;
            isAnchorReleased = false;

            // Reset scale
            transform.localScale = originalScale;

            // Reset light
            if (riftLight != null)
            {
                riftLight.intensity = 1f;
            }

            // Stop particles
            if (implosionParticles != null)
            {
                implosionParticles.Stop();
            }

            // Re-enable components
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }

            if (riftController != null)
            {
                riftController.enabled = true;
            }
        }

        /// <summary>
        /// Get implosion progress
        /// </summary>
        public float GetProgress()
        {
            if (!isImpoding) return 0f;
            float elapsed = Time.time - implosionStartTime;
            return Mathf.Clamp01(elapsed / implosionDuration);
        }
    }
}
