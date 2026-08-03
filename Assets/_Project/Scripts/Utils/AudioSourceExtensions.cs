using UnityEngine;
using System.Collections;

namespace BreachAR.Utils
{
    /// <summary>
    /// AudioSource extension methods
    /// </summary>
    public static class AudioSourceExtensions
    {
        /// <summary>
        /// Play clip at point
        /// </summary>
        public static void PlayClipAtPoint(this AudioSource source, AudioClip clip, Vector3 position, float volume = 1f)
        {
            AudioSource.PlayClipAtPoint(clip, position, volume);
        }

        /// <summary>
        /// Fade volume
        /// </summary>
        public static IEnumerator FadeVolume(this AudioSource source, float targetVolume, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(startVolume, targetVolume, elapsed / duration);
                yield return null;
            }

            source.volume = targetVolume;
        }

        /// <summary>
        /// Fade in
        /// </summary>
        public static IEnumerator FadeIn(this AudioSource source, float duration, float targetVolume = 1f)
        {
            source.volume = 0f;
            source.Play();
            return source.FadeVolume(targetVolume, duration);
        }

        /// <summary>
        /// Fade out
        /// </summary>
        public static IEnumerator FadeOut(this AudioSource source, float duration)
        {
            float startVolume = source.volume;
            yield return source.FadeVolume(0f, duration);
            source.Stop();
            source.volume = startVolume;
        }

        /// <summary>
        /// Crossfade to another source
        /// </summary>
        public static IEnumerator CrossfadeTo(this AudioSource from, AudioSource to, float duration)
        {
            if (from != null)
            {
                yield return from.FadeOut(duration);
            }

            if (to != null)
            {
                yield return to.FadeIn(duration);
            }
        }

        /// <summary>
        /// Play with pitch variation
        /// </summary>
        public static void PlayWithPitch(this AudioSource source, float pitchVariation = 0.1f)
        {
            source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            source.Play();
        }

        /// <summary>
        /// Play one shot with pitch variation
        /// </summary>
        public static void PlayOneShotWithPitch(this AudioSource source, AudioClip clip, float pitchVariation = 0.1f)
        {
            source.pitch = 1f + Random.Range(-pitchVariation, pitchVariation);
            source.PlayOneShot(clip);
        }
    }
}
