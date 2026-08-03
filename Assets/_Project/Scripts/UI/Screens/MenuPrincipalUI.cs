using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Core;
using BreachAR.AR;
using BreachAR.Utils;
using VContainer;

namespace BreachAR.UI
{
    /// <summary>
    /// Main menu UI screen with navigation
    /// Referência: UI-009, 07_ui.md §7.4
    /// </summary>
    public class MenuPrincipalUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button playButton;
        [SerializeField] private Button campaignButton;
        [SerializeField] private Button endlessButton;
        [SerializeField] private Button dailyChallengeButton;
        [SerializeField] private Button zenButton;
        [SerializeField] private Button storeButton;
        [SerializeField] private Button profileButton;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private Button settingsButton;

        [Header("Sub-Menus")]
        [SerializeField] private GameObject storePanel;
        [SerializeField] private GameObject profilePanel;
        [SerializeField] private GameObject leaderboardPanel;
        [SerializeField] private GameObject settingsPanel;
        [SerializeField] private GameObject privacyConsentPanel;

        [Header("Info Panels")]
        [SerializeField] private GameObject deviceWarningPanel;
        [SerializeField] private TextMeshProUGUI deviceWarningText;

        [Header("Animations")]
        [SerializeField] private Animator menuAnimator;

        [Inject] private GameManager gameManager;
        [Inject] private ARSessionService arSessionService;
        [Inject] private SceneLoader sceneLoader;

        private void Start()
        {
            SetupButtons();
            CheckDeviceCompatibility();
            HideAllPanels();
        }

        private void SetupButtons()
        {
            playButton?.onClick.AddListener(OnPlayClicked);
            campaignButton?.onClick.AddListener(OnCampaignClicked);
            endlessButton?.onClick.AddListener(OnEndlessClicked);
            dailyChallengeButton?.onClick.AddListener(OnDailyChallengeClicked);
            zenButton?.onClick.AddListener(OnZenClicked);
            storeButton?.onClick.AddListener(OnStoreClicked);
            profileButton?.onClick.AddListener(OnProfileClicked);
            leaderboardButton?.onClick.AddListener(OnLeaderboardClicked);
            settingsButton?.onClick.AddListener(OnSettingsClicked);
        }

        private void CheckDeviceCompatibility()
        {
            if (arSessionService == null) return;

            var capability = arSessionService.CheckDeviceCapability();
            if (!capability.SupportsAR)
            {
                ShowDeviceWarning("Your device does not support AR. Some features may be limited.");
            }
        }

        private void ShowDeviceWarning(string message)
        {
            if (deviceWarningPanel != null)
            {
                deviceWarningPanel.SetActive(true);
                if (deviceWarningText != null)
                {
                    deviceWarningText.text = message;
                }
            }
        }

        private void HideAllPanels()
        {
            storePanel?.SetActive(false);
            profilePanel?.SetActive(false);
            leaderboardPanel?.SetActive(false);
            settingsPanel?.SetActive(false);
            privacyConsentPanel?.SetActive(false);
        }

        private void OnPlayClicked()
        {
            StartGame(GameMode.Campaign);
        }

        private void OnCampaignClicked()
        {
            StartGame(GameMode.Campaign);
        }

        private void OnEndlessClicked()
        {
            StartGame(GameMode.Endless);
        }

        private void OnDailyChallengeClicked()
        {
            StartGame(GameMode.DailyChallenge);
        }

        private void OnZenClicked()
        {
            StartGame(GameMode.Zen);
        }

        private void OnStoreClicked()
        {
            HideAllPanels();
            storePanel?.SetActive(true);
            GameEvents.OnMenuOpened?.Invoke(new MenuOpenedData { MenuName = "Store" });
        }

        private void OnProfileClicked()
        {
            HideAllPanels();
            profilePanel?.SetActive(true);
            GameEvents.OnMenuOpened?.Invoke(new MenuOpenedData { MenuName = "Profile" });
        }

        private void OnLeaderboardClicked()
        {
            HideAllPanels();
            leaderboardPanel?.SetActive(true);
            GameEvents.OnMenuOpened?.Invoke(new MenuOpenedData { MenuName = "Leaderboard" });
        }

        private void OnSettingsClicked()
        {
            HideAllPanels();
            settingsPanel?.SetActive(true);
            GameEvents.OnMenuOpened?.Invoke(new MenuOpenedData { MenuName = "Settings" });
        }

        private void StartGame(GameMode mode)
        {
            gameManager?.StartGame(mode);
            Hide();
        }

        /// <summary>
        /// Show menu with animation
        /// </summary>
        public void Show()
        {
            gameObject.SetActive(true);
            HideAllPanels();
            menuAnimator?.SetTrigger("Show");
        }

        /// <summary>
        /// Hide menu with animation
        /// </summary>
        public void Hide()
        {
            menuAnimator?.SetTrigger("Hide");
            
            if (menuAnimator == null)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
