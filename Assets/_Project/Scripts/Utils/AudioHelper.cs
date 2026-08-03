using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Audio utility functions
    /// </summary>
    public static class AudioHelper
    {
        /// <summary>
        /// Play sound at point with auto-destroy
        /// </summary>
        public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return null;

            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = position;
            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.clip = clip;
            aSource.volume = volume;
            aSource.Play();

            Object.Destroy(tempGO, clip.length + 0.1f);
            return aSource;
        }

        /// <summary>
        /// Play 3D sound with spatial blend
        /// </summary>
        public static AudioSource Play3D(AudioClip clip, Vector3 position, float volume = 1f, float spatialBlend = 1f)
        {
            if (clip == null) return null;

            GameObject tempGO = new GameObject("TempAudio3D");
            tempGO.transform.position = position;
            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.clip = clip;
            aSource.volume = volume;
            aSource.spatialBlend = spatialBlend;
            aSource.Play();

            Object.Destroy(tempGO, clip.length + 0.1f);
            return aSource;
        }

        /// <summary>
        /// Get random clip from array
        /// </summary>
        public static AudioClip GetRandomClip(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0) return null;
            return clips[Random.Range(0, clips.Length)];
        }

        /// <summary>
        /// Convert decibels to linear volume
        /// </summary>
        public static float DecibelToLinear(float decibels)
        {
            return Mathf.Pow(10f, decibels / 20f);
        }

        /// <summary>
        /// Convert linear volume to decibels
        /// </summary>
        public static float LinearToDecibel(float linear)
        {
            if (linear <= 0) return -80f;
            return 20f * Mathf.Log10(linear);
        }

        /// <summary>
        /// Fade audio source volume
        /// </summary>
        public static System.Collections.IEnumerator FadeVolume(AudioSource source, float targetVolume, float duration)
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
        /// Fade in audio source
        /// </summary>
        public static System.Collections.IEnumerator FadeIn(AudioSource source, float duration, float targetVolume = 1f)
        {
            source.volume = 0f;
            source.Play();
            return FadeVolume(source, targetVolume, duration);
        }

        /// <summary>
        /// Fade out audio source
        /// </summary>
        public static System.Collections.IEnumerator FadeOut(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            yield return FadeVolume(source, 0f, duration);
            source.Stop();
            source.volume = startVolume;
        }

        /// <summary>
        /// Crossfade between two audio sources
        /// </summary>
        public static System.Collections.IEnumerator Crossfade(AudioSource from, AudioSource to, float duration)
        {
            if (from != null)
            {
                yield return FadeOut(from, duration);
            }

            if (to != null)
            {
                yield return FadeIn(to, duration);
            }
        }
    }
}
