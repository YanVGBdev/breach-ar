using UnityEngine;
using BreachAR.Core;
using System;
using VContainer;

namespace BreachAR.Gameplay
{
    /// <summary>
    /// Manages session state transitions and wave progression
    /// Referência: GP-010, specs/EnemySpawner.md
    /// </summary>
    public class SessionStateMachine : MonoBehaviour
    {
        [Header("State")]
        [SerializeField] private SessionState currentState = SessionState.Idle;
        [SerializeField] private int currentWaveIndex;
        [SerializeField] private int totalWaves;
        [SerializeField] private float waveStartTime;

        [Inject] private DifficultyDirector difficultyDirector;
        [Inject] private WaveGenerator waveGenerator;
        [Inject] private ComboSystem comboSystem;
        [Inject] private ScoreSystem scoreSystem;

        public SessionState CurrentState => currentState;
        public int CurrentWaveIndex => currentWaveIndex;
        public int TotalWaves => totalWaves;
        public bool IsSessionActive => currentState == SessionState.WaveIntro || 
                                       currentState == SessionState.WaveActive || 
                                       currentState == SessionState.BossActive;

        /// <summary>
        /// Initialize session with total waves
        /// </summary>
        public void Initialize(int waves)
        {
            totalWaves = waves;
            currentWaveIndex = 0;
            ChangeState(SessionState.Initializing);
            
            // Initialize systems
            waveGenerator.Initialize();
            difficultyDirector.ResetDifficulty();
            comboSystem.Activate();
            scoreSystem.ResetScore();
        }

        /// <summary>
        /// Start the first wave
        /// </summary>
        public void StartSession()
        {
            if (currentState == SessionState.Initializing)
            {
                ChangeState(SessionState.WaveIntro);
                StartNextWave();
            }
        }

        /// <summary>
        /// Start the next wave
        /// </summary>
        private void StartNextWave()
        {
            currentWaveIndex++;
            waveStartTime = Time.time;

            bool isBossWave = currentWaveIndex % 10 == 0 && currentWaveIndex <= totalWaves;

            // Generate wave composition
            var waveDef = waveGenerator.GenerateWave(currentWaveIndex, isBossWave);

            // Notify systems
            GameEvents.OnWaveStarted?.Invoke(new WaveStartedData
            {
                WaveIndex = currentWaveIndex,
                TotalWaves = totalWaves,
                IsBossWave = isBossWave
            });

            ChangeState(isBossWave ? SessionState.BossActive : SessionState.WaveActive);

            Debug.Log($"[Session] Wave {currentWaveIndex} started" + (isBossWave ? " (BOSS)" : ""));
        }

        /// <summary>
        /// Call when all enemies in wave are defeated
        /// </summary>
        public void CompleteWave()
        {
            if (currentState != SessionState.WaveActive && currentState != SessionState.BossActive)
                return;

            float timeTaken = Time.time - waveStartTime;
            bool perfectWave = comboSystem.ComboCount > 0; // Simplified check

            // Calculate rewards
            GameEvents.OnWaveCompleted?.Invoke(new WaveCompletedData
            {
                WaveIndex = currentWaveIndex,
                TimeTaken = timeTaken,
                CoreHpRemaining = 100f, // Would come from CoreController
                PerfectWave = perfectWave
            });

            // Check if session complete
            if (currentWaveIndex >= totalWaves)
            {
                ChangeState(SessionState.SessionComplete);
                Debug.Log("[Session] All waves completed!");
            }
            else
            {
                // Brief pause between waves
                ChangeState(SessionState.WaveTransition);
                StartCoroutine(WaveTransitionCoroutine());
            }
        }

        /// <summary>
        /// Handle core destroyed
        /// </summary>
        public void HandleCoreDestroyed()
        {
            ChangeState(SessionState.Failed);
            
            GameEvents.OnGameOver?.Invoke(new GameOverData
            {
                Victory = false,
                FinalScore = scoreSystem.CurrentScore,
                WavesCleared = currentWaveIndex - 1,
                MaxCombo = scoreSystem.MaxComboAchieved,
                FragmentsKilled = scoreSystem.TotalFragmentsKilled,
                RiftsClosed = scoreSystem.TotalRiftsClosed
            });

            Debug.Log("[Session] Core destroyed - Game Over");
        }

        /// <summary>
        /// Pause the session
        /// </summary>
        public void Pause()
        {
            if (IsSessionActive)
            {
                ChangeState(SessionState.Paused);
            }
        }

        /// <summary>
        /// Resume from pause
        /// </summary>
        public void Resume()
        {
            if (currentState == SessionState.Paused)
            {
                ChangeState(currentWaveIndex % 10 == 0 ? SessionState.BossActive : SessionState.WaveActive);
            }
        }

        /// <summary>
        /// Abort the current session
        /// </summary>
        public void AbortSession()
        {
            comboSystem.Deactivate();
            ChangeState(SessionState.Idle);
            Debug.Log("[Session] Session aborted");
        }

        /// <summary>
        /// Wave transition delay
        /// </summary>
        private System.Collections.IEnumerator WaveTransitionCoroutine()
        {
            yield return new WaitForSeconds(2f); // 2 second transition
            StartNextWave();
        }

        /// <summary>
        /// Change session state
        /// </summary>
        private void ChangeState(SessionState newState)
        {
            Debug.Log($"[Session] State: {currentState} → {newState}");
            currentState = newState;
        }
    }

    /// <summary>
    /// Session states
    /// </summary>
    public enum SessionState
    {
        Idle,
        Initializing,
        WaveIntro,
        WaveActive,
        WaveTransition,
        BossActive,
        SessionComplete,
        Failed,
        Paused
    }
}
