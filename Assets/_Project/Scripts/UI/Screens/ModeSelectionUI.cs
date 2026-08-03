using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Core;
using VContainer;

namespace BreachAR.UI
{
    /// <summary>
    /// Mode selection UI screen
    /// Referência: UI-011, 07_ui.md §7.4
    /// </summary>
    public class ModeSelectionUI : MonoBehaviour
    {
        [Header("Mode Buttons")]
        [SerializeField] private Button campaignButton;
        [SerializeField] private Button endlessButton;
        [SerializeField] private Button dailyChallengeButton;
        [SerializeField] private Button zenButton;

        [Header("Mode Info")]
        [SerializeField] private TextMeshProUGUI modeTitleText;
        [SerializeField] private TextMeshProUGUI modeDescriptionText;
        [SerializeField] private Image modePreviewImage;

        [Header("Campaign Info")]
        [SerializeField] private TextMeshProUGUI campaignProgressText;
        [SerializeField] private Slider campaignProgressBar;

        [Header("Daily Challenge Info")]
        [SerializeField] private TextMeshProUGUI dailyTimerText;
        [SerializeField] private TextMeshProUGUI dailyBestScoreText;

        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button backButton;

        private GameMode selectedMode;

        [Inject] private GameManager gameManager;

        private void Start()
        {
            SetupButtons();
            UpdateModeInfo(GameMode.Campaign);
        }

        private void SetupButtons()
        {
            campaignButton?.onClick.AddListener(() => SelectMode(GameMode.Campaign));
            endlessButton?.onClick.AddListener(() => SelectMode(GameMode.Endless));
            dailyChallengeButton?.onClick.AddListener(() => SelectMode(GameMode.DailyChallenge));
            zenButton?.onClick.AddListener(() => SelectMode(GameMode.Zen));
            playButton?.onClick.AddListener(OnPlayClicked);
            backButton?.onClick.AddListener(OnBackClicked);
        }

        private void SelectMode(GameMode mode)
        {
            selectedMode = mode;
            UpdateModeInfo(mode);
        }

        private void UpdateModeInfo(GameMode mode)
        {
            string title = "";
            string description = "";

            switch (mode)
            {
                case GameMode.Campaign:
                    title = "Campaign";
                    description = "Progress through waves of increasing difficulty. Face bosses every 10 waves!";
                    break;
                case GameMode.Endless:
                    title = "Endless";
                    description = "How long can you survive? Difficulty increases indefinitely.";
                    break;
                case GameMode.DailyChallenge:
                    title = "Daily Challenge";
                    description = "Same challenge for all players. Compete on the leaderboard!";
                    break;
                case GameMode.Zen:
                    title = "Zen Mode";
                    description = "Practice without pressure. No Game Over, no scoring.";
                    break;
            }

            if (modeTitleText != null) modeTitleText.text = title;
            if (modeDescriptionText != null) modeDescriptionText.text = description;

            // Update button states
            campaignButton?.interactable = mode != GameMode.Campaign;
            endlessButton?.interactable = mode != GameMode.Endless;
            dailyChallengeButton?.interactable = mode != GameMode.DailyChallenge;
            zenButton?.interactable = mode != GameMode.Zen;
        }

        private void OnPlayClicked()
        {
            Debug.Log($"[UI] Starting {selectedMode} mode");
            gameManager?.StartGame(selectedMode);
            gameObject.SetActive(false);
        }

        private void OnBackClicked()
        {
            gameObject.SetActive(false);
        }
    }
}
