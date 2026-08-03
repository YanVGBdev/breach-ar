using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace BreachAR.UI
{
    /// <summary>
    /// Leaderboard UI screen
    /// </summary>
    public class LeaderboardUI : MonoBehaviour
    {
        [Header("Tabs")]
        [SerializeField] private Button globalTab;
        [SerializeField] private Button friendsTab;
        [SerializeField] private Button dailyTab;
        [SerializeField] private GameObject globalPanel;
        [SerializeField] private GameObject friendsPanel;
        [SerializeField] private GameObject dailyPanel;

        [Header("Mode Filter")]
        [SerializeField] private TMP_Dropdown modeDropdown;

        [Header("Leaderboard List")]
        [SerializeField] private Transform leaderboardContainer;
        [SerializeField] private GameObject leaderboardEntryPrefab;
        [SerializeField] private GameObject playerEntryPrefab;

        [Header("Player Info")]
        [SerializeField] private TextMeshProUGUI playerRankText;
        [SerializeField] private TextMeshProUGUI playerScoreText;

        [Header("Buttons")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button refreshButton;

        private List<LeaderboardEntryUI> entries;

        private void Start()
        {
            SetupTabs();
            SetupButtons();
            SetupModeDropdown();
            LoadLeaderboard();
        }

        private void SetupTabs()
        {
            globalTab?.onClick.AddListener(() => ShowPanel(globalPanel));
            friendsTab?.onClick.AddListener(() => ShowPanel(friendsPanel));
            dailyTab?.onClick.AddListener(() => ShowPanel(dailyPanel));
        }

        private void SetupButtons()
        {
            backButton?.onClick.AddListener(OnBackClicked);
            refreshButton?.onClick.AddListener(OnRefreshClicked);
        }

        private void SetupModeDropdown()
        {
            if (modeDropdown == null) return;

            modeDropdown.ClearOptions();
            modeDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "Campaign", "Endless", "Daily Challenge"
            });
            modeDropdown.onValueChanged.AddListener(OnModeChanged);
        }

        private void ShowPanel(GameObject panel)
        {
            globalPanel?.SetActive(false);
            friendsPanel?.SetActive(false);
            dailyPanel?.SetActive(false);
            panel?.SetActive(true);
            LoadLeaderboard();
        }

        private void LoadLeaderboard()
        {
            // TODO: Load from backend
            // For now, create placeholder entries
            entries = new List<LeaderboardEntryUI>();

            ClearEntries();

            // Add sample entries
            for (int i = 1; i <= 10; i++)
            {
                CreateEntry(i, $"Player_{i:000}", 100000 - (i * 5000));
            }
        }

        private void ClearEntries()
        {
            if (leaderboardContainer == null) return;

            foreach (Transform child in leaderboardContainer)
            {
                Destroy(child.gameObject);
            }
        }

        private void CreateEntry(int rank, string playerName, int score)
        {
            if (leaderboardEntryPrefab == null || leaderboardContainer == null) return;

            GameObject entryObj = Instantiate(leaderboardEntryPrefab, leaderboardContainer);
            LeaderboardEntryUI entryUI = entryObj.GetComponent<LeaderboardEntryUI>();

            if (entryUI != null)
            {
                entryUI.Initialize(rank, playerName, score);
                entries.Add(entryUI);
            }
        }

        private void OnModeChanged(int modeIndex)
        {
            Debug.Log($"[Leaderboard] Mode changed to: {modeIndex}");
            LoadLeaderboard();
        }

        private void OnBackClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnRefreshClicked()
        {
            LoadLeaderboard();
        }

        /// <summary>
        /// Highlight player's entry
        /// </summary>
        public void HighlightPlayerEntry(int playerRank)
        {
            foreach (var entry in entries)
            {
                if (entry.Rank == playerRank)
                {
                    entry.SetHighlighted(true);
                    break;
                }
            }

            // Update player info display
            if (playerRankText != null)
                playerRankText.text = $"#{playerRank}";
        }
    }

    /// <summary>
    /// Leaderboard entry UI component
    /// </summary>
    public class LeaderboardEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI rankText;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Color normalColor = Color.clear;
        [SerializeField] private Color highlightColor = new Color(1f, 0.8f, 0.2f, 0.3f);

        public int Rank { get; private set; }

        public void Initialize(int rank, string playerName, int score)
        {
            Rank = rank;
            if (rankText != null) rankText.text = $"#{rank}";
            if (nameText != null) nameText.text = playerName;
            if (scoreText != null) scoreText.text = score.ToString("N0");
        }

        public void SetHighlighted(bool highlighted)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = highlighted ? highlightColor : normalColor;
            }
        }
    }
}
