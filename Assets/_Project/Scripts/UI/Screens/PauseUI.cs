using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Core;
using BreachAR.Gameplay;
using BreachAR.AR;
using VContainer;

namespace BreachAR.UI
{
    /// <summary>
    /// Pause menu UI with full functionality
    /// Referência: UI-007, UI-017, specs/HUD.md
    /// </summary>
    public class PauseUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button rescanButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Stats Display")]
        [SerializeField] private TextMeshProUGUI currentScoreText;
        [SerializeField] private TextMeshProUGUI currentWaveText;
        [SerializeField] private TextMeshProUGUI elapsedTimeText;

        [Header("Settings Panel")]
        [SerializeField] private GameObject settingsPanel;

        [Header("Animations")]
        [SerializeField] private Animator pauseAnimator;

        private float sessionElapsedSeconds;
        private float pauseStartTime;
        private bool isPaused;

        [Inject] private GameManager gameManager;
        [Inject] private ARSessionService arSessionService;
        [Inject] private ScoreSystem scoreSystem;
        [Inject] private SessionStateMachine sessionStateMachine;

        private void Start()
        {
            SetupButtons();
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(false);
            }
        }

        private void SetupButtons()
        {
            resumeButton?.onClick.AddListener(OnResumeClicked);
            rescanButton?.onClick.AddListener(OnRescanClicked);
            restartButton?.onClick.AddListener(OnRestartClicked);
            settingsButton?.onClick.AddListener(OnSettingsClicked);
            mainMenuButton?.onClick.AddListener(OnMainMenuClicked);
        }

        /// <summary>
        /// Show pause menu
        /// </summary>
        public void Show(int currentScore, int currentWave, float sessionElapsed)
        {
            gameObject.SetActive(true);
            isPaused = true;
            sessionElapsedSeconds = sessionElapsed;
            pauseStartTime = Time.unscaledTime;

            // Update stats
            if (currentScoreText != null)
                currentScoreText.text = $"Score: {currentScore.ToString("N0")}";

            if (currentWaveText != null)
                currentWaveText.text = $"Wave: {currentWave}";

            if (elapsedTimeText != null)
                elapsedTimeText.text = $"Time: {FormatTime(sessionTime)}";

            // Trigger animation
            if (pauseAnimator != null)
            {
                pauseAnimator.SetTrigger("Show");
            }

            // Emit pause event
            GameEvents.OnPauseToggled?.Invoke(new PauseToggledData
            {
                IsPaused = true
            });
        }

        /// <summary>
        /// Hide pause menu
        /// </summary>
        public void Hide()
        {
            isPaused = false;
            
            if (pauseAnimator != null)
            {
                pauseAnimator.SetTrigger("Hide");
            }
            else
            {
                gameObject.SetActive(false);
            }

            // Emit pause event
            GameEvents.OnPauseToggled?.Invoke(new PauseToggledData
            {
                IsPaused = false
            });
        }

        private void OnResumeClicked()
        {
            Debug.Log("[UI] Resume clicked");
            
            if (gameManager != null)
            {
                gameManager.ResumeGame();
            }
            
            Hide();
        }

        private void OnRescanClicked()
        {
            Debug.Log("[UI] Rescan clicked");
            
            arSessionService?.Rescan();
        }

        private void OnRestartClicked()
        {
            Debug.Log("[UI] Restart clicked");
            
            if (gameManager != null)
            {
                gameManager.StartGame(gameManager.CurrentGameMode);
            }
            
            Hide();
        }

        private void OnSettingsClicked()
        {
            Debug.Log("[UI] Settings clicked");
            
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
        }

        private void OnMainMenuClicked()
        {
            Debug.Log("[UI] Main menu clicked");
            
            if (gameManager != null)
            {
                gameManager.ReturnToMainMenu();
            }
            
            Hide();
        }

        private string FormatTime(float seconds)
        {
            int minutes = Mathf.FloorToInt(seconds / 60f);
            int secs = Mathf.FloorToInt(seconds % 60f);
            return $"{minutes:00}:{secs:00}";
        }

        private void Update()
        {
            // Update elapsed time while paused
            if (isPaused && elapsedTimeText != null)
            {
                float currentElapsed = sessionElapsedSeconds + (Time.unscaledTime - pauseStartTime);
                elapsedTimeText.text = $"Time: {FormatTime(currentElapsed)}";
            }
        }
    }
}
