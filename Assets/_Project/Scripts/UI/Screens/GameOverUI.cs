using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Core;
using BreachAR.Gameplay;
using VContainer;

namespace BreachAR.UI
{
    /// <summary>
    /// Game over screen UI with full functionality
    /// Referência: UI-018, UI-019, UI-020
    /// </summary>
    public class GameOverUI : MonoBehaviour
    {
        [Header("Result")]
        [SerializeField] private TextMeshProUGUI resultTitle;
        [SerializeField] private TextMeshProUGUI resultSubtitle;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI wavesClearedText;
        [SerializeField] private TextMeshProUGUI maxComboText;
        [SerializeField] private TextMeshProUGUI fragmentsKilledText;
        [SerializeField] private TextMeshProUGUI riftsClosedText;

        [Header("Rewards")]
        [SerializeField] private Transform rewardsContainer;
        [SerializeField] private GameObject rewardItemPrefab;
        [SerializeField] private TextMeshProUGUI softCurrencyRewardText;
        [SerializeField] private TextMeshProUGUI experienceRewardText;

        [Header("Record")]
        [SerializeField] private GameObject newRecordPanel;
        [SerializeField] private TextMeshProUGUI personalBestText;

        [Header("Buttons")]
        [SerializeField] private Button retryButton;
        [SerializeField] private Button reviveButton;
        [SerializeField] private Button shareButton;
        [SerializeField] private Button leaderboardButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Animations")]
        [SerializeField] private Animator gameOverAnimator;
        [SerializeField] private float statsRevealDelay = 0.5f;

        private ScoreBreakdown scoreBreakdown;
        private bool isVictory;

        [Inject] private GameManager gameManager;
        [Inject] private ScoreSystem scoreSystem;
        [Inject] private EconomyService economyService;

        private void Start()
        {
            SetupButtons();
        }

        private void SetupButtons()
        {
            retryButton?.onClick.AddListener(OnRetryClicked);
            reviveButton?.onClick.AddListener(OnReviveClicked);
            shareButton?.onClick.AddListener(OnShareClicked);
            leaderboardButton?.onClick.AddListener(OnLeaderboardClicked);
            mainMenuButton?.onClick.AddListener(OnMainMenuClicked);
        }

        /// <summary>
        /// Show game over screen
        /// </summary>
        public void Show(ScoreBreakdown breakdown, bool victory, int experienceReward = 0)
        {
            gameObject.SetActive(true);
            scoreBreakdown = breakdown;
            isVictory = victory;

            // Set title
            if (resultTitle != null)
            {
                resultTitle.text = victory ? "VICTORY!" : "GAME OVER";
            }

            if (resultSubtitle != null)
            {
                resultSubtitle.text = victory ? "All waves cleared!" : "The Core has been destroyed";
            }

            // Update stats with animation
            StartCoroutine(RevealStats(breakdown, experienceReward));

            // Check for new record
            CheckNewRecord(breakdown.TotalScore);

            // Show/hide revive button
            if (reviveButton != null)
            {
                reviveButton.gameObject.SetActive(!victory);
            }

            // Trigger animation
            if (gameOverAnimator != null)
            {
                gameOverAnimator.SetTrigger(victory ? "Victory" : "Defeat");
            }

            // Emit game over event
            GameEvents.OnGameOver?.Invoke(new GameOverData
            {
                Victory = victory,
                FinalScore = breakdown.TotalScore,
                WavesCleared = breakdown.WavesCleared,
                MaxCombo = breakdown.MaxCombo,
                FragmentsKilled = breakdown.FragmentsKilled,
                RiftsClosed = breakdown.RiftsClosed
            });
        }

        private System.Collections.IEnumerator RevealStats(ScoreBreakdown breakdown, int experienceReward)
        {
            yield return new WaitForSeconds(statsRevealDelay);

            // Animate score counting
            if (scoreText != null)
            {
                yield return StartCoroutine(AnimateNumber(scoreText, 0, breakdown.TotalScore, 1f));
            }

            yield return new WaitForSeconds(0.2f);

            // Show other stats
            if (wavesClearedText != null)
                wavesClearedText.text = $"Waves: {breakdown.WavesCleared}";

            if (maxComboText != null)
                maxComboText.text = $"Max Combo: x{breakdown.MaxCombo:F1}";

            if (fragmentsKilledText != null)
                fragmentsKilledText.text = $"Fragments: {breakdown.FragmentsKilled}";

            if (riftsClosedText != null)
                riftsClosedText.text = $"Rifts: {breakdown.RiftsClosed}";

            // Show rewards
            if (softCurrencyRewardText != null)
            {
                int softReward = breakdown.TotalScore / 100;
                softCurrencyRewardText.text = $"+{softReward} Energy Fragments";
            }

            if (experienceRewardText != null)
            {
                experienceRewardText.text = $"+{experienceReward} XP";
            }
        }

        private System.Collections.IEnumerator AnimateNumber(TextMeshProUGUI text, int from, int to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / duration;
                int current = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
                text.text = current.ToString("N0");
                yield return null;
            }
            text.text = to.ToString("N0");
        }

        private void CheckNewRecord(int score)
        {
            int personalBest = PlayerPrefs.GetInt("PersonalBest", 0);
            
            if (score > personalBest)
            {
                PlayerPrefs.SetInt("PersonalBest", score);
                PlayerPrefs.Save();

                if (newRecordPanel != null)
                {
                    newRecordPanel.SetActive(true);
                }
            }
            else if (personalBestText != null)
            {
                personalBestText.text = $"Best: {personalBest.ToString("N0")}";
            }
        }

        private void OnRetryClicked()
        {
            Debug.Log("[UI] Retry clicked");
            
            gameManager?.StartGame(gameManager.CurrentGameMode);
            Hide();
        }

        private void OnReviveClicked()
        {
            Debug.Log("[UI] Revive clicked");
            
            // Check if player has enough hard currency
            if (economyService != null)
            {
                if (economyService.SpendHardCurrency(50, "Revive"))
                {
                    // Revive flow - would restore Core health and resume
                    Hide();
                }
                else
                {
                    Debug.Log("[UI] Insufficient crystals for revive");
                    // Would show error notification
                }
            }
        }

        private void OnShareClicked()
        {
            Debug.Log("[UI] Share clicked");
            
            // Generate share content
            string shareText = $"I scored {scoreBreakdown.TotalScore:N0} in BreachAR! " +
                             $"Waves: {scoreBreakdown.WavesCleared} | " +
                             $"Max Combo: x{scoreBreakdown.MaxCombo:F1}";
            
            // Copy to clipboard as fallback
            GUIUtility.systemCopyBuffer = shareText;
            Debug.Log("[UI] Score copied to clipboard");
        }

        private void OnLeaderboardClicked()
        {
            Debug.Log("[UI] Leaderboard clicked");
            // Would navigate to leaderboard screen
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

        /// <summary>
        /// Hide game over screen
        /// </summary>
        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
