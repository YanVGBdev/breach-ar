using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Core;
using BreachAR.Audio;
using BreachAR.Backend;
using VContainer;

namespace BreachAR.UI
{
    /// <summary>
    /// Settings UI screen with full functionality
    /// Referência: UI-015, 07_ui.md §7.3
    /// </summary>
    public class SettingsUI : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private Slider masterVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        [SerializeField] private Slider sfxVolumeSlider;
        [SerializeField] private TextMeshProUGUI masterVolumeText;
        [SerializeField] private TextMeshProUGUI musicVolumeText;
        [SerializeField] private TextMeshProUGUI sfxVolumeText;

        [Header("Graphics Settings")]
        [SerializeField] private TMP_Dropdown graphicsQualityDropdown;
        [SerializeField] private Toggle reducedParticlesToggle;
        [SerializeField] private Toggle advancedOcclusionToggle;

        [Header("Control Settings")]
        [SerializeField] private Slider dragSensitivitySlider;
        [SerializeField] private Toggle showTrajectoryToggle;

        [Header("Accessibility")]
        [SerializeField] private Toggle highContrastHUDToggle;
        [SerializeField] private Slider fontScaleSlider;
        [SerializeField] private Toggle reducedShakeToggle;

        [Header("Account")]
        [SerializeField] private Button linkAccountButton;
        [SerializeField] private Button restorePurchasesButton;
        [SerializeField] private TextMeshProUGUI accountStatusText;

        [Header("Privacy")]
        [SerializeField] private Button privacyPolicyButton;
        [SerializeField] private Button deleteAccountButton;

        [Header("Buttons")]
        [SerializeField] private Button backButton;
        [SerializeField] private Button applyButton;

        private GameSettings currentSettings;

        [Inject] private SaveService saveService;
        [Inject] private AudioManager audioManager;

        private void Start()
        {
            LoadSettings();
            SetupButtons();
            SetupSliders();
            UpdateVolumeTexts();
        }

        private void SetupButtons()
        {
            backButton?.onClick.AddListener(OnBackClicked);
            applyButton?.onClick.AddListener(OnApplyClicked);
            linkAccountButton?.onClick.AddListener(OnLinkAccountClicked);
            restorePurchasesButton?.onClick.AddListener(OnRestorePurchasesClicked);
            privacyPolicyButton?.onClick.AddListener(OnPrivacyPolicyClicked);
            deleteAccountButton?.onClick.AddListener(OnDeleteAccountClicked);
        }

        private void SetupSliders()
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
                masterVolumeSlider.value = currentSettings.MasterVolume;
            }

            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
                musicVolumeSlider.value = currentSettings.MusicVolume;
            }

            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
                sfxVolumeSlider.value = currentSettings.SFXVolume;
            }

            if (dragSensitivitySlider != null)
            {
                dragSensitivitySlider.onValueChanged.AddListener(OnDragSensitivityChanged);
                dragSensitivitySlider.value = currentSettings.Sensitivity;
            }

            if (fontScaleSlider != null)
            {
                fontScaleSlider.onValueChanged.AddListener(OnFontScaleChanged);
                fontScaleSlider.value = currentSettings.FontScale;
            }

            if (graphicsQualityDropdown != null)
            {
                graphicsQualityDropdown.ClearOptions();
                graphicsQualityDropdown.AddOptions(new System.Collections.Generic.List<string>
                {
                    "Low", "Medium", "High"
                });
                graphicsQualityDropdown.value = currentSettings.QualityLevel;
                graphicsQualityDropdown.onValueChanged.AddListener(OnGraphicsQualityChanged);
            }

            if (reducedParticlesToggle != null)
            {
                reducedParticlesToggle.isOn = false;
            }

            if (advancedOcclusionToggle != null)
            {
                advancedOcclusionToggle.isOn = false;
            }

            if (showTrajectoryToggle != null)
            {
                showTrajectoryToggle.isOn = true;
            }

            if (highContrastHUDToggle != null)
            {
                highContrastHUDToggle.isOn = currentSettings.HighContrast;
                highContrastHUDToggle.onValueChanged.AddListener(OnHighContrastChanged);
            }

            if (reducedShakeToggle != null)
            {
                reducedShakeToggle.isOn = currentSettings.ReduceShake;
                reducedShakeToggle.onValueChanged.AddListener(OnReducedShakeChanged);
            }
        }

        private void LoadSettings()
        {
            if (saveService != null && saveService.CurrentSaveData != null)
            {
                currentSettings = saveService.CurrentSaveData.Settings;
            }
            else
            {
                currentSettings = new GameSettings();
            }
        }

        private void UpdateVolumeTexts()
        {
            if (masterVolumeText != null)
                masterVolumeText.text = $"{Mathf.RoundToInt(currentSettings.MasterVolume * 100)}%";
            if (musicVolumeText != null)
                musicVolumeText.text = $"{Mathf.RoundToInt(currentSettings.MusicVolume * 100)}%";
            if (sfxVolumeText != null)
                sfxVolumeText.text = $"{Mathf.RoundToInt(currentSettings.SFXVolume * 100)}%";
        }

        #region Callbacks

        private void OnMasterVolumeChanged(float value)
        {
            currentSettings.MasterVolume = value;
            UpdateVolumeTexts();
            audioManager?.SetMasterVolume(value);
        }

        private void OnMusicVolumeChanged(float value)
        {
            currentSettings.MusicVolume = value;
            UpdateVolumeTexts();
            audioManager?.SetMusicVolume(value);
        }

        private void OnSFXVolumeChanged(float value)
        {
            currentSettings.SFXVolume = value;
            UpdateVolumeTexts();
            audioManager?.SetSFXVolume(value);
        }

        private void OnDragSensitivityChanged(float value)
        {
            currentSettings.Sensitivity = value;
        }

        private void OnFontScaleChanged(float value)
        {
            currentSettings.FontScale = value;
        }

        private void OnGraphicsQualityChanged(int value)
        {
            currentSettings.QualityLevel = value;
            QualitySettings.SetQualityLevel(value);
        }

        private void OnHighContrastChanged(bool value)
        {
            currentSettings.HighContrast = value;
        }

        private void OnReducedShakeChanged(bool value)
        {
            currentSettings.ReduceShake = value;
        }

        private void OnBackClicked()
        {
            gameObject.SetActive(false);
        }

        private void OnApplyClicked()
        {
            saveService?.UpdateSettings(currentSettings);
            saveService?.Save();
            
            Debug.Log("[Settings] Settings applied and saved");
            gameObject.SetActive(false);
        }

        private void OnLinkAccountClicked()
        {
            Debug.Log("[Settings] Link account clicked");
            // Would open Google Play Games linking flow
        }

        private void OnRestorePurchasesClicked()
        {
            Debug.Log("[Settings] Restore purchases clicked");
            // Would trigger IAP restoration
        }

        private void OnPrivacyPolicyClicked()
        {
            Debug.Log("[Settings] Privacy policy clicked");
            Application.OpenURL("https://breachar.com/privacy");
        }

        private void OnDeleteAccountClicked()
        {
            Debug.Log("[Settings] Delete account clicked");
            // Would show confirmation dialog
        }

        #endregion
    }
}
