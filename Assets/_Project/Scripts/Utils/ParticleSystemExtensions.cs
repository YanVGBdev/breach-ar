using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// ParticleSystem extension methods
    /// </summary>
    public static class ParticleSystemExtensions
    {
        /// <summary>
        /// Play if not playing
        /// </summary>
        public static void PlayIfNotPlaying(this ParticleSystem particleSystem)
        {
            if (particleSystem != null && !particleSystem.isPlaying)
            {
                particleSystem.Play();
            }
        }

        /// <summary>
        /// Stop if playing
        /// </summary>
        public static void StopIfPlaying(this ParticleSystem particleSystem)
        {
            if (particleSystem != null && particleSystem.isPlaying)
            {
                particleSystem.Stop();
            }
        }

        /// <summary>
        /// Clear particles
        /// </summary>
        public static void ClearParticles(this ParticleSystem particleSystem)
        {
            if (particleSystem != null)
            {
                particleSystem.Clear();
            }
        }

        /// <summary>
        /// Set emission rate
        /// </summary>
        public static void SetEmissionRate(this ParticleSystem particleSystem, float rate)
        {
            if (particleSystem != null)
            {
                var emission = particleSystem.emission;
                emission.rateOverTime = rate;
            }
        }

        /// <summary>
        /// Set max particles
        /// </summary>
        public static void SetMaxParticles(this ParticleSystem particleSystem, int max)
        {
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.maxParticles = max;
            }
        }

        /// <summary>
        /// Set start color
        /// </summary>
        public static void SetStartColor(this ParticleSystem particleSystem, Color color)
        {
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startColor = color;
            }
        }

        /// <summary>
        /// Set start size
        /// </summary>
        public static void SetStartSize(this ParticleSystem particleSystem, float size)
        {
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startSize = size;
            }
        }

        /// <summary>
        /// Set start lifetime
        /// </summary>
        public static void SetStartLifetime(this ParticleSystem particleSystem, float lifetime)
        {
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.startLifetime = lifetime;
            }
        }

        /// <summary>
        /// Set simulation speed
        /// </summary>
        public static void SetSimulationSpeed(this ParticleSystem particleSystem, float speed)
        {
            if (particleSystem != null)
            {
                var main = particleSystem.main;
                main.simulationSpeed = speed;
            }
        }
    }
}
