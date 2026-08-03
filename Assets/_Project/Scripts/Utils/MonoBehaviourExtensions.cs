using UnityEngine;
using System.Collections;

namespace BreachAR.Utils
{
    /// <summary>
    /// MonoBehaviour extension methods
    /// </summary>
    public static class MonoBehaviourExtensions
    {
        /// <summary>
        /// Start coroutine with automatic null check
        /// </summary>
        public static Coroutine StartSafeCoroutine(this MonoBehaviour mono, IEnumerator routine)
        {
            if (mono != null && mono.gameObject.activeInHierarchy)
            {
                return mono.StartCoroutine(routine);
            }
            return null;
        }

        /// <summary>
        /// Stop coroutine safely
        /// </summary>
        public static void StopSafeCoroutine(this MonoBehaviour mono, Coroutine coroutine)
        {
            if (mono != null && coroutine != null)
            {
                mono.StopCoroutine(coroutine);
            }
        }

        /// <summary>
        /// Invoke action after delay
        /// </summary>
        public static void InvokeAfter(this MonoBehaviour mono, float delay, System.Action action)
        {
            mono.StartCoroutine(InvokeAfterCoroutine(delay, action));
        }

        private static IEnumerator InvokeAfterCoroutine(float delay, System.Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }

        /// <summary>
        /// Invoke action after frame
        /// </summary>
        public static void InvokeNextFrame(this MonoBehaviour mono, System.Action action)
        {
            mono.StartCoroutine(InvokeNextFrameCoroutine(action));
        }

        private static IEnumerator InvokeNextFrameCoroutine(System.Action action)
        {
            yield return null;
            action?.Invoke();
        }

        /// <summary>
        /// Invoke action at end of frame
        /// </summary>
        public static void InvokeAtEndOfFrame(this MonoBehaviour mono, System.Action action)
        {
            mono.StartCoroutine(InvokeAtEndOfFrameCoroutine(action));
        }

        private static IEnumerator InvokeAtEndOfFrameCoroutine(System.Action action)
        {
            yield return new WaitForEndOfFrame();
            action?.Invoke();
        }

        /// <summary>
        /// Check if component is enabled
        /// </summary>
        public static bool IsEnabled<T>(this MonoBehaviour mono) where T : Behaviour
        {
            T component = mono.GetComponent<T>();
            return component != null && component.enabled;
        }
    }
}
