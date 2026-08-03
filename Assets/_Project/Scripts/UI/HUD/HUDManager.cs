using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Core;
using BreachAR.Gameplay;
using VContainer;

namespace BreachAR.UI
{
    /// <summary>
    /// Manages all HUD elements during gameplay
    /// Referência: UI-001, UI-002, UI-003, UI-004, UI-005, specs/HUD.md
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        [Header("Core Health")]
        [SerializeField] private Slider coreHealthBar;
        [SerializeField] private Image coreHealthFill;
        [SerializeField] private TextMeshProUGUI coreHealthText;
        [SerializeField] private float healthBarLerpSpeed = 5f;

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI scoreDeltaText;
        [SerializeField] private float scoreDeltaDisplayTime = 1f;

        [Header("Combo")]
        [SerializeField] private TextMeshProUGUI comboText;
        [SerializeField] private Image comboBackground;
        [SerializeField] private Color comboLowColor = Color.white;
        [SerializeField] private Color comboHighColor = Color.yellow;
        [SerializeField] private float comboPulseScale = 1.2f;

        [Header("Wave")]
        [SerializeField] private TextMeshProUGUI waveText;

        [Header("Power-ups")]
        [SerializeField] private Transform powerUpContainer;
        [SerializeField] private GameObject powerUpIconPrefab;

        [Header("Threat Indicators")]
        [SerializeField] private GameObject threatIndicatorPrefab;
        [SerializeField] private float indicatorOffset = 50f;

        private float scoreDeltaTimer;
        private float currentHealthDisplay;
        private Camera mainCamera;

        [Inject] private CoreController coreController;
        [Inject] private ComboSystem comboSystem;
        [Inject] private ScoreSystem scoreSystem;
        [Inject] private SessionStateMachine sessionStateMachine;

        private void Start()
        {
            mainCamera = Camera.main;
            
            // Subscribe to events
            GameEvents.OnCoreDamaged += HandleCoreDamaged;
            GameEvents.OnComboChanged += HandleComboChanged;
            GameEvents.OnScoreChanged += HandleScoreChanged;
            GameEvents.OnWaveStarted += HandleWaveStarted;
            
            // Initialize displays
            if (coreController != null)
            {
                currentHealthDisplay = coreController.CurrentHealth;
            }
        }

        private void OnDestroy()
        {
            GameEvents.OnCoreDamaged -= HandleCoreDamaged;
            GameEvents.OnComboChanged -= HandleComboChanged;
            GameEvents.OnScoreChanged -= HandleScoreChanged;
            GameEvents.OnWaveStarted -= HandleWaveStarted;
        }

        private void Update()
        {
            UpdateCoreHealth();
            UpdateCombo();
            UpdateScoreDelta();
            UpdateScore();
        }

        /// <summary>
        /// Update core health display with smooth lerp
        /// Referência: UI-002
        /// </summary>
        private void UpdateCoreHealth()
        {
            if (coreController == null || coreHealthBar == null) return;

            // Smooth lerp to current health
            currentHealthDisplay = Mathf.Lerp(currentHealthDisplay, coreController.CurrentHealth, 
                                              Time.deltaTime * healthBarLerpSpeed);
            
            float healthPercent = currentHealthDisplay / coreController.MaxHealth;
            coreHealthBar.value = healthPercent;

            // Update color based on health
            if (coreHealthFill != null)
            {
                coreHealthFill.color = Color.Lerp(Color.red, Color.green, healthPercent);
            }

            // Update text
            if (coreHealthText != null)
            {
                coreHealthText.text = $"{currentHealthDisplay:F0}/{coreController.MaxHealth:F0}";
            }
        }

        /// <summary>
        /// Update combo display
        /// Referência: UI-005
        /// </summary>
        private void UpdateCombo()
        {
            if (comboSystem == null || comboText == null) return;

            float multiplier = comboSystem.CurrentMultiplier;
            comboText.text = $"x{multiplier:F1}";

            // Update color based on multiplier
            if (comboBackground != null)
            {
                float t = (multiplier - 1f) / 4f; // 1.0 = 0, 5.0 = 1
                t = Mathf.Clamp01(t);
                comboBackground.color = Color.Lerp(comboLowColor, comboHighColor, t);
            }
        }

        /// <summary>
        /// Update score display
        /// Referência: UI-004
        /// </summary>
        private void UpdateScore()
        {
            if (scoreSystem == null || scoreText == null) return;

            scoreText.text = scoreSystem.CurrentScore.ToString("N0");
        }

        /// <summary>
        /// Update score delta display
        /// </summary>
        private void UpdateScoreDelta()
        {
            if (scoreDeltaText == null) return;

            if (scoreDeltaTimer > 0)
            {
                scoreDeltaTimer -= Time.deltaTime;
                if (scoreDeltaTimer <= 0)
                {
                    scoreDeltaText.gameObject.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Show score change
        /// </summary>
        public void ShowScoreDelta(int delta, string reason)
        {
            if (scoreDeltaText == null) return;

            scoreDeltaText.text = $"+{delta}";
            scoreDeltaText.gameObject.SetActive(true);
            scoreDeltaTimer = scoreDeltaDisplayTime;
        }

        /// <summary>
        /// Update wave display
        /// Referência: UI-003
        /// </summary>
        public void UpdateWaveDisplay(int currentWave, int totalWaves)
        {
            if (waveText == null) return;

            if (totalWaves == int.MaxValue)
            {
                waveText.text = $"Wave {currentWave}";
            }
            else
            {
                waveText.text = $"Wave {currentWave}/{totalWaves}";
            }
        }

        /// <summary>
        /// Create threat indicator for off-screen enemy
        /// Referência: GP-035
        /// </summary>
        public void CreateThreatIndicator(Vector3 worldPosition, ThreatType type)
        {
            if (threatIndicatorPrefab == null || mainCamera == null) return;

            Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPosition);
            bool isOnScreen = screenPos.x > 0 && screenPos.x < Screen.width &&
                              screenPos.y > 0 && screenPos.y < Screen.height &&
                              screenPos.z > 0;

            if (isOnScreen) return;

            GameObject indicator = Instantiate(threatIndicatorPrefab, transform);
            
            Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Vector2 direction = ((Vector2)screenPos - screenCenter).normalized;
            
            indicator.transform.position = screenCenter + direction * indicatorOffset;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            indicator.transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

            Destroy(indicator, 2f);
        }

        /// <summary>
        /// Show/hide HUD
        /// </summary>
        public void SetHUDVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        #region Event Handlers

        private void HandleCoreDamaged(CoreDamagedData data)
        {
            // Health lerp will handle the visual update
        }

        private void HandleComboChanged(ComboChangedData data)
        {
            if (data.WasReset)
            {
                // Could trigger visual feedback for combo reset
            }
        }

        private void HandleScoreChanged(ScoreChangedData data)
        {
            ShowScoreDelta(data.ScoreDelta, data.Reason);
        }

        private void HandleWaveStarted(WaveStartedData data)
        {
            UpdateWaveDisplay(data.WaveIndex, data.TotalWaves);
        }

        #endregion
    }

    /// <summary>
    /// Threat types for indicators
    /// </summary>
    public enum ThreatType
    {
        Fragment,
        Rift,
        Boss
    }
}
