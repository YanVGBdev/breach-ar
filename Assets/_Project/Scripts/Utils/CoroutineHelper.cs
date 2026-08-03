using UnityEngine;
using System.Collections;
using VContainer;

namespace BreachAR.Utils
{
    /// <summary>
    /// Coroutine helper for common operations
    /// </summary>
    public class CoroutineHelper : MonoBehaviour
    {
        /// <summary>
        /// Delayed action
        /// </summary>
        public void Delay(float delay, System.Action action)
        {
            StartCoroutine(DelayCoroutine(delay, action));
        }

        private IEnumerator DelayCoroutine(float delay, System.Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        /// <summary>
        /// Delayed action (unscaled time)
        /// </summary>
        public void DelayUnscaled(float delay, System.Action action)
        {
            StartCoroutine(DelayUnscaledCoroutine(delay, action));
        }

        private IEnumerator DelayUnscaledCoroutine(float delay, System.Action action)
        {
            yield return new WaitForSecondsRealtime(delay);
            action?.Invoke();
        }

        /// <summary>
        /// Wait until condition is true
        /// </summary>
        public void WaitUntil(System.Func<bool> condition, System.Action action)
        {
            StartCoroutine(WaitUntilCoroutine(condition, action));
        }

        private IEnumerator WaitUntilCoroutine(System.Func<bool> condition, System.Action action)
        {
            yield return new WaitUntil(condition);
            action?.Invoke();
        }

        /// <summary>
        /// Lerp over time
        /// </summary>
        public void Lerp(float from, float to, float duration, System.Action<float> onProgress, System.Action onComplete = null)
        {
            StartCoroutine(LerpCoroutine(from, to, duration, onProgress, onComplete));
        }

        private IEnumerator LerpCoroutine(float from, float to, float duration, System.Action<float> onProgress, System.Action onComplete)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                float value = Mathf.Lerp(from, to, t);
                onProgress?.Invoke(value);
                yield return null;
            }
            onProgress?.Invoke(to);
            onComplete?.Invoke();
        }

        /// <summary>
        /// Lerp over time (unscaled)
        /// </summary>
        public void LerpUnscaled(float from, float to, float duration, System.Action<float> onProgress, System.Action onComplete = null)
        {
            StartCoroutine(LerpUnscaledCoroutine(from, to, duration, onProgress, onComplete));
        }

        private IEnumerator LerpUnscaledCoroutine(float from, float to, float duration, System.Action<float> onProgress, System.Action onComplete)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                float value = Mathf.Lerp(from, to, t);
                onProgress?.Invoke(value);
                yield return null;
            }
            onProgress?.Invoke(to);
            onComplete?.Invoke();
        }

        /// <summary>
        /// Shake transform
        /// </summary>
        public void Shake(Transform target, float duration, float magnitude, System.Action onComplete = null)
        {
            StartCoroutine(ShakeCoroutine(target, duration, magnitude, onComplete));
        }

        private IEnumerator ShakeCoroutine(Transform target, float duration, float magnitude, System.Action onComplete)
        {
            Vector3 originalPosition = target.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;
                target.localPosition = originalPosition + new Vector3(x, y, 0);
                yield return null;
            }

            target.localPosition = originalPosition;
            onComplete?.Invoke();
        }

        /// <summary>
        /// Pulse transform scale
        /// </summary>
        public void Pulse(Transform target, float fromScale, float toScale, float duration, System.Action onComplete = null)
        {
            StartCoroutine(PulseCoroutine(target, fromScale, toScale, duration, onComplete));
        }

        private IEnumerator PulseCoroutine(Transform target, float fromScale, float toScale, float duration, System.Action onComplete)
        {
            float halfDuration = duration / 2f;

            // Scale up
            float elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(fromScale, toScale, elapsed / halfDuration);
                target.localScale = Vector3.one * scale;
                yield return null;
            }

            // Scale down
            elapsed = 0f;
            while (elapsed < halfDuration)
            {
                elapsed += Time.deltaTime;
                float scale = Mathf.Lerp(toScale, fromScale, elapsed / halfDuration);
                target.localScale = Vector3.one * scale;
                yield return null;
            }

            target.localScale = Vector3.one * fromScale;
            onComplete?.Invoke();
        }
    }
}
