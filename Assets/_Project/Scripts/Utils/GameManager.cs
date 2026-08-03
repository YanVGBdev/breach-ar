using UnityEngine;
using BreachAR.Core;
using BreachAR.Gameplay;
using BreachAR.Backend;
using VContainer;

namespace BreachAR.Utils
{
    /// <summary>
    /// Central game manager that coordinates all systems
    /// Referência: 99_agent_rules.md - DI via VContainer
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Header("Game State")]
        [SerializeField] private GameState currentState;
        [SerializeField] private GameMode currentGameMode;

        [Inject] private SessionStateMachine sessionStateMachine;
        [Inject] private ComboSystem comboSystem;
        [Inject] private ScoreSystem scoreSystem;
        [Inject] private CoreController coreController;
        [Inject] private DifficultyDirector difficultyDirector;
        [Inject] private EconomyService economyService;

        public GameState CurrentState => currentState;
        public GameMode CurrentGameMode => currentGameMode;

        private void Start()
        {
            ChangeGameState(GameState.MainMenu);
        }

        /// <summary>
        /// Start a new game session
        /// </summary>
        public void StartGame(GameMode mode)
        {
            currentGameMode = mode;
            ChangeGameState(GameState.Scanning);
        }

        /// <summary>
        /// Complete AR scanning and start gameplay
        /// </summary>
        public void CompleteScanning()
        {
            ChangeGameState(GameState.Playing);
            
            // Initialize session
            int totalWaves = GetTotalWavesForMode(currentGameMode);
            sessionStateMachine.Initialize(totalWaves);
            comboSystem.Activate();
            scoreSystem.ResetScore();
        }

        /// <summary>
        /// Pause the game
        /// </summary>
        public void PauseGame()
        {
            if (currentState == GameState.Playing)
            {
                ChangeGameState(GameState.Paused);
                sessionStateMachine.Pause();
                Time.timeScale = 0f;
            }
        }

        /// <summary>
        /// Resume the game
        /// </summary>
        public void ResumeGame()
        {
            if (currentState == GameState.Paused)
            {
                ChangeGameState(GameState.Playing);
                sessionStateMachine.Resume();
                Time.timeScale = 1f;
            }
        }

        /// <summary>
        /// End the current session
        /// </summary>
        public void EndSession(bool victory)
        {
            ChangeGameState(GameState.GameOver);
            
            comboSystem.Deactivate();
            Time.timeScale = 0f;

            // Calculate rewards
            SessionRewards rewards = economyService.CalculateSessionRewards(
                scoreSystem.CurrentScore,
                sessionStateMachine.CurrentWaveIndex,
                false // perfectWave - would need to track this
            );

            economyService.ApplySessionRewards(rewards);

            // Emit game over event
            GameEvents.OnGameOver?.Invoke(new GameOverData
            {
                Victory = victory,
                FinalScore = scoreSystem.CurrentScore,
                WavesCleared = sessionStateMachine.CurrentWaveIndex,
                MaxCombo = comboSystem.MaxCombo,
                FragmentsKilled = 0, // TODO: Track this
                RiftsClosed = 0 // TODO: Track this
            });
        }

        /// <summary>
        /// Return to main menu
        /// </summary>
        public void ReturnToMainMenu()
        {
            ChangeGameState(GameState.MainMenu);
            Time.timeScale = 1f;
            
            // Reset systems
            comboSystem.Deactivate();
            scoreSystem.ResetScore();
        }

        private void ChangeGameState(GameState newState)
        {
            Debug.Log($"[GameManager] State change: {currentState} → {newState}");
            currentState = newState;
        }

        private int GetTotalWavesForMode(GameMode mode)
        {
            switch (mode)
            {
                case GameMode.Campaign:
                    return 20;
                case GameMode.Endless:
                    return int.MaxValue;
                case GameMode.DailyChallenge:
                    return 15;
                case GameMode.Zen:
                    return int.MaxValue;
                default:
                    return 10;
            }
        }
    }
}
