using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Reusable timer utility
    /// </summary>
    [System.Serializable]
    public class Timer
    {
        public float Duration;
        public bool IsLooping;
        public bool StartOnEnable;

        private float currentTime;
        private bool isRunning;
        private bool isPaused;

        public float CurrentTime => currentTime;
        public float NormalizedTime => Duration > 0 ? currentTime / Duration : 0f;
        public bool IsFinished => currentTime >= Duration;
        public bool IsRunning => isRunning;
        public bool IsPaused => isPaused;

        public event System.Action OnTimerFinished;
        public event System.Action<float> OnTimerUpdated;

        public Timer(float duration, bool looping = false)
        {
            Duration = duration;
            IsLooping = looping;
            currentTime = 0f;
            isRunning = false;
            isPaused = false;
        }

        /// <summary>
        /// Start the timer
        /// </summary>
        public void Start()
        {
            currentTime = 0f;
            isRunning = true;
            isPaused = false;
        }

        /// <summary>
        /// Stop the timer
        /// </summary>
        public void Stop()
        {
            isRunning = false;
            isPaused = false;
        }

        /// <summary>
        /// Pause the timer
        /// </summary>
        public void Pause()
        {
            isPaused = true;
        }

        /// <summary>
        /// Resume the timer
        /// </summary>
        public void Resume()
        {
            isPaused = false;
        }

        /// <summary>
        /// Reset the timer
        /// </summary>
        public void Reset()
        {
            currentTime = 0f;
            isRunning = false;
            isPaused = false;
        }

        /// <summary>
        /// Update the timer
        /// </summary>
        public void Tick(float deltaTime)
        {
            if (!isRunning || isPaused) return;

            currentTime += deltaTime;
            OnTimerUpdated?.Invoke(currentTime);

            if (currentTime >= Duration)
            {
                if (IsLooping)
                {
                    currentTime = 0f;
                }
                else
                {
                    isRunning = false;
                }

                OnTimerFinished?.Invoke();
            }
        }

        /// <summary>
        /// Update the timer using unscaled time
        /// </summary>
        public void TickUnscaled()
        {
            Tick(Time.unscaledDeltaTime);
        }

        /// <summary>
        /// Update the timer using scaled time
        /// </summary>
        public void TickScaled()
        {
            Tick(Time.deltaTime);
        }

        /// <summary>
        /// Get remaining time
        /// </summary>
        public float GetRemainingTime()
        {
            return Mathf.Max(0, Duration - currentTime);
        }

        /// <summary>
        /// Check if timer is running and not finished
        /// </summary>
        public bool IsActive()
        {
            return isRunning && !isPaused && !IsFinished;
        }
    }

    /// <summary>
    /// Timer that counts down from a duration
    /// </summary>
    [System.Serializable]
    public class CountdownTimer : Timer
    {
        public CountdownTimer(float duration) : base(duration)
        {
        }

        /// <summary>
        /// Get normalized remaining time (1 = full, 0 = empty)
        /// </summary>
        public float GetNormalizedRemaining()
        {
            return Duration > 0 ? GetRemainingTime() / Duration : 0f;
        }
    }

    /// <summary>
    /// Stopwatch that counts up
    /// </summary>
    [System.Serializable]
    public class Stopwatch
    {
        private float elapsedTime;
        private bool isRunning;

        public float ElapsedTime => elapsedTime;
        public bool IsRunning => isRunning;

        public Stopwatch()
        {
            elapsedTime = 0f;
            isRunning = false;
        }

        public void Start()
        {
            isRunning = true;
        }

        public void Stop()
        {
            isRunning = false;
        }

        public void Reset()
        {
            elapsedTime = 0f;
            isRunning = false;
        }

        public void Tick(float deltaTime)
        {
            if (isRunning)
            {
                elapsedTime += deltaTime;
            }
        }

        public string GetFormattedTime()
        {
            int minutes = Mathf.FloorToInt(elapsedTime / 60f);
            int seconds = Mathf.FloorToInt(elapsedTime % 60f);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);
            return $"{minutes:00}:{seconds:00}.{milliseconds:00}";
        }
    }
}
