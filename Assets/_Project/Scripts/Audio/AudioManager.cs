using UnityEngine;
using System.Collections.Generic;

namespace BreachAR.Audio
{
    /// <summary>
    /// Manages all game audio - music, SFX, and spatial audio
    /// Injected via VContainer DI
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Volume Settings")]
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

        [Header("SFX Pools")]
        [SerializeField] private int sfxPoolSize = 10;

        private Queue<AudioSource> sfxPool;
        private Dictionary<string, AudioClip> audioClipCache;

        private void Awake()
        {
            InitializeSFXPool();
            LoadSettings();
        }

        private void InitializeSFXPool()
        {
            sfxPool = new Queue<AudioSource>();

            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject sfxObject = new GameObject($"SFX_{i}");
                sfxObject.transform.parent = transform;
                AudioSource source = sfxObject.AddComponent<AudioSource>();
                source.playOnAwake = false;
                sfxPool.Enqueue(source);
            }
        }

        /// <summary>
        /// Play a sound effect
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeScale = 1f)
        {
            if (clip == null) return;

            AudioSource source = GetAvailableSFXSource();
            if (source == null) return;

            source.clip = clip;
            source.volume = sfxVolume * masterVolume * volumeScale;
            source.Play();
        }

        /// <summary>
        /// Play a sound effect at a position
        /// </summary>
        public void PlaySFX(AudioClip clip, Vector3 position, float volumeScale = 1f)
        {
            if (clip == null) return;

            AudioSource source = GetAvailableSFXSource();
            if (source == null) return;

            source.transform.position = position;
            source.clip = clip;
            source.volume = sfxVolume * masterVolume * volumeScale;
            source.spatialBlend = 1f;
            source.Play();
        }

        /// <summary>
        /// Play a random sound from a collection
        /// </summary>
        public void PlaySFXRandom(AudioClip[] clips, float volumeScale = 1f)
        {
            if (clips == null || clips.Length == 0) return;

            AudioClip randomClip = clips[Random.Range(0, clips.Length)];
            PlaySFX(randomClip, volumeScale);
        }

        /// <summary>
        /// Play music track
        /// </summary>
        public void PlayMusic(AudioClip clip, float fadeDuration = 0.5f)
        {
            if (clip == null) return;

            musicSource.clip = clip;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.loop = true;
            musicSource.Play();
        }

        /// <summary>
        /// Stop music
        /// </summary>
        public void StopMusic(float fadeDuration = 0.5f)
        {
            musicSource.Stop();
        }

        /// <summary>
        /// Pause music
        /// </summary>
        public void PauseMusic()
        {
            musicSource.Pause();
        }

        /// <summary>
        /// Resume music
        /// </summary>
        public void ResumeMusic()
        {
            musicSource.UnPause();
        }

        /// <summary>
        /// Set master volume
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
            SaveSettings();
        }

        /// <summary>
        /// Set music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
            SaveSettings();
        }

        /// <summary>
        /// Set SFX volume
        /// </summary>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            UpdateVolumes();
            SaveSettings();
        }

        /// <summary>
        /// Get current volume settings
        /// </summary>
        public AudioVolumeSettings GetVolumeSettings()
        {
            return new AudioVolumeSettings
            {
                MasterVolume = masterVolume,
                MusicVolume = musicVolume,
                SFXVolume = sfxVolume
            };
        }

        private void UpdateVolumes()
        {
            if (musicSource != null)
            {
                musicSource.volume = musicVolume * masterVolume;
            }
        }

        private AudioSource GetAvailableSFXSource()
        {
            foreach (AudioSource source in sfxPool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }

            return sfxPool.Count > 0 ? sfxPool.Peek() : null;
        }

        private void LoadSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            UpdateVolumes();
        }

        private void SaveSettings()
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.Save();
        }
    }

    /// <summary>
    /// Audio volume settings
    /// </summary>
    [System.Serializable]
    public struct AudioVolumeSettings
    {
        public float MasterVolume;
        public float MusicVolume;
        public float SFXVolume;
    }
}
