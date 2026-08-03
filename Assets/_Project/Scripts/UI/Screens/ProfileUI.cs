using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Core;

namespace BreachAR.UI
{
    /// <summary>
    /// Profile UI screen
    /// </summary>
    public class ProfileUI : MonoBehaviour
    {
        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI playerNameText;
        [SerializeField] private TextMeshProUGUI playerLevelText;
        [SerializeField] private Image playerAvatarImage;
        [SerializeField] private Slider experienceBar;
        [SerializeField] private TextMeshProUGUI experienceText;

        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI totalGamesText;
        [SerializeField] private TextMeshProUGUI bestScoreText;
        [SerializeField] private TextMeshProUGUI totalFragmentsText;
        [SerializeField] private TextMeshProUGUI playTimeText;

        [Header("Orbs")]
        [SerializeField] private Transform orbContainer;
        [SerializeField] private GameObject orbItemPrefab;

        [Header("Buttons")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button editNameButton;

        private void Start()
        {
            SetupButtons();
            LoadProfile();
        }

        private void SetupButtons()
        {
            backButton?.onClick.AddListener(OnBackClicked);
            editNameButton?.onClick.AddListener(OnEditNameClicked);
        }

        private void LoadProfile()
        {
            // TODO: Load from SaveService
            // For now, use placeholder data
            UpdateProfile("Player", 1, 0, 100);
            UpdateStats(42, 15000, 1250, "5h 30m");
        }

        /// <summary>
        /// Update profile display
        /// </summary>
        public void UpdateProfile(string name, int level, float currentExp, float expRequired)
        {
            if (playerNameText != null)
                playerNameText.text = name;

            if (playerLevelText != null)
                playerLevelText.text = $"Level {level}";

            if (experienceBar != null)
                experienceBar.value = currentExp / expRequired;

            if (experienceText != null)
                experienceText.text = $"{currentExp:F0} / {expRequired:F0} XP";
        }

        /// <summary>
        /// Update stats display
        /// </summary>
        public void UpdateStats(int totalGames, int bestScore, int totalFragments, string playTime)
        {
            if (totalGamesText != null)
                totalGamesText.text = totalGames.ToString();

            if (bestScoreText != null)
                bestScoreText.text = bestScore.ToString("N0");

            if (totalFragmentsText != null)
                totalFragmentsText.text = totalFragments.ToString("N0");

            if (playTimeText != null)
                playTimeText.text = playTime;
        }

        private void OnBackClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnEditNameClicked()
        {
            Debug.Log("[Profile] Edit name clicked");
            // TODO: Show name input dialog
        }
    }
}
