using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace BreachAR.UI
{
    /// <summary>
    /// Onboarding UI for new players
    /// </summary>
    public class OnboardingUI : MonoBehaviour
    {
        [Header("Steps")]
        [SerializeField] private GameObject[] onboardingSteps;
        [SerializeField] private int currentStep;

        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private Image illustrationImage;
        [SerializeField] private Animator stepAnimator;

        [Header("Buttons")]
        [SerializeField] private Button nextButton;
        [SerializeField] private Button skipButton;
        [SerializeField] private Button previousButton;

        [Header("Progress")]
        [SerializeField] private Transform progressContainer;
        [SerializeField] private GameObject progressDotPrefab;

        private void Start()
        {
            SetupButtons();
            CreateProgressDots();
            ShowStep(0);
        }

        private void SetupButtons()
        {
            nextButton?.onClick.AddListener(OnNextClicked);
            skipButton?.onClick.AddListener(OnSkipClicked);
            previousButton?.onClick.AddListener(OnPreviousClicked);
        }

        private void CreateProgressDots()
        {
            if (progressContainer == null || progressDotPrefab == null) return;

            for (int i = 0; i < onboardingSteps.Length; i++)
            {
                GameObject dot = Instantiate(progressDotPrefab, progressContainer);
                dot.name = $"Dot_{i}";
            }
        }

        private void ShowStep(int stepIndex)
        {
            if (stepIndex < 0 || stepIndex >= onboardingSteps.Length) return;

            // Hide all steps
            foreach (var step in onboardingSteps)
            {
                step.SetActive(false);
            }

            // Show current step
            onboardingSteps[stepIndex].SetActive(true);
            currentStep = stepIndex;

            // Update progress dots
            UpdateProgressDots();

            // Update button states
            if (previousButton != null)
                previousButton.gameObject.SetActive(stepIndex > 0);

            if (nextButton != null)
            {
                TextMeshProUGUI nextText = nextButton.GetComponentInChildren<TextMeshProUGUI>();
                if (nextText != null)
                {
                    nextText.text = stepIndex == onboardingSteps.Length - 1 ? "Start" : "Next";
                }
            }

            // Trigger animation
            if (stepAnimator != null)
            {
                stepAnimator.SetTrigger("NextStep");
            }
        }

        private void UpdateProgressDots()
        {
            if (progressContainer == null) return;

            for (int i = 0; i < progressContainer.childCount; i++)
            {
                Transform dot = progressContainer.GetChild(i);
                Image dotImage = dot.GetComponent<Image>();
                if (dotImage != null)
                {
                    dotImage.color = i <= currentStep ? Color.white : Color.gray;
                }
            }
        }

        private void OnNextClicked()
        {
            if (currentStep < onboardingSteps.Length - 1)
            {
                ShowStep(currentStep + 1);
            }
            else
            {
                CompleteOnboarding();
            }
        }

        private void OnPreviousClicked()
        {
            if (currentStep > 0)
            {
                ShowStep(currentStep - 1);
            }
        }

        private void OnSkipClicked()
        {
            CompleteOnboarding();
        }

        private void CompleteOnboarding()
        {
            Debug.Log("[Onboarding] Completed");
            PlayerPrefs.SetInt("OnboardingCompleted", 1);
            PlayerPrefs.Save();
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Check if onboarding has been completed
        /// </summary>
        public static bool IsCompleted()
        {
            return PlayerPrefs.GetInt("OnboardingCompleted", 0) == 1;
        }

        /// <summary>
        /// Reset onboarding (for testing)
        /// </summary>
        public static void Reset()
        {
            PlayerPrefs.SetInt("OnboardingCompleted", 0);
            PlayerPrefs.Save();
        }
    }
}
