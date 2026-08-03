using UnityEngine;
using System.Collections;

namespace BreachAR.Utils
{
    /// <summary>
    /// Coroutine extension methods
    /// </summary>
    public static class CoroutineExtensions
    {
        /// <summary>
        /// Wait for seconds
        /// </summary>
        public static IEnumerator WaitFor(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        /// <summary>
        /// Wait for unscaled seconds
        /// </summary>
        public static IEnumerator WaitForUnscaled(float seconds)
        {
            yield return new WaitForSecondsRealtime(seconds);
        }

        /// <summary>
        /// Wait for end of frame
        /// </summary>
        public static IEnumerator WaitForEndOfFrame()
        {
            yield return new WaitForEndOfFrame();
        }

        /// <summary>
        /// Wait for next frame
        /// </summary>
        public static IEnumerator WaitForNextFrame()
        {
            yield return null;
        }

        /// <summary>
        /// Wait for condition
        /// </summary>
        public static IEnumerator WaitUntil(System.Func<bool> condition)
        {
            yield return new WaitUntil(condition);
        }

        /// <summary>
        /// Wait while condition
        /// </summary>
        public static IEnumerator WaitWhile(System.Func<bool> condition)
        {
            yield return new WaitWhile(condition);
        }

        /// <summary>
        /// Lerp over time
        /// </summary>
        public static IEnumerator LerpOverTime(float from, float to, float duration, System.Action<float> onProgress)
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
        }

        /// <summary>
        /// Lerp over unscaled time
        /// </summary>
        public static IEnumerator LerpOverUnscaledTime(float from, float to, float duration, System.Action<float> onProgress)
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
        }
    }
}
