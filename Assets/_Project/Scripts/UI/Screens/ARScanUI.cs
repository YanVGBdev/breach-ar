using UnityEngine;
using UnityEngine.UI;
using TMPro;
using BreachAR.Core;
using BreachAR.AR;
using System.Collections;
using VContainer;

namespace BreachAR.UI
{
    /// <summary>
    /// AR scanning UI with visual feedback and progress
    /// Referência: AR-003, specs/ARSurfaceService.md
    /// </summary>
    public class ARScanUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private CanvasGroup scanPanel;
        [SerializeField] private Slider progressBar;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI instructionText;
        [SerializeField] private Image scanRing;
        [SerializeField] private Animator scanAnimator;

        [Header("Surface Indicators")]
        [SerializeField] private GameObject floorIndicator;
        [SerializeField] private GameObject wallIndicator;
        [SerializeField] private GameObject ceilingIndicator;
        [SerializeField] private TextMeshProUGUI surfaceCountText;

        [Header("Settings")]
        [SerializeField] private float fadeInDuration = 0.5f;
        [SerializeField] private float fadeOutDuration = 0.3f;
        [SerializeField] private string[] scanInstructions;
        [SerializeField] private float instructionRotationInterval = 3f;

        [Inject] private ARSessionService arSessionService;

        private bool isScanning;
        private int currentInstructionIndex;
        private float instructionTimer;
        private int previousSurfaceCount;

        private void Start()
        {
            // Initialize indicators
            SetIndicatorActive(floorIndicator, false);
            SetIndicatorActive(wallIndicator, false);
            SetIndicatorActive(ceilingIndicator, false);

            // Subscribe to AR events
            GameEvents.OnSurfaceDetected += HandleSurfaceDetected;
            GameEvents.OnScanComplete += HandleScanComplete;
        }

        private void OnDestroy()
        {
            GameEvents.OnSurfaceDetected -= HandleSurfaceDetected;
            GameEvents.OnScanComplete -= HandleScanComplete;
        }

        private void Update()
        {
            if (!isScanning) return;

            // Update progress from AR service
            UpdateProgress();

            // Rotate instructions
            UpdateInstructions();

            // Update surface indicators
            UpdateSurfaceIndicators();
        }

        /// <summary>
        /// Start scanning UI
        /// </summary>
        public void StartScan()
        {
            isScanning = true;
            previousSurfaceCount = 0;
            currentInstructionIndex = 0;
            instructionTimer = 0f;

            gameObject.SetActive(true);
            StartCoroutine(FadeIn());
            ShowInstruction(0);

            // Start AR session
            arSessionService?.StartSession();
        }

        /// <summary>
        /// Stop scanning UI
        /// </summary>
        public void StopScan()
        {
            isScanning = false;
            StartCoroutine(FadeOut());
        }

        /// <summary>
        /// Update progress bar
        /// </summary>
        private void UpdateProgress()
        {
            if (arSessionService == null) return;

            float progress = arSessionService.ScanProgress;

            if (progressBar != null)
            {
                progressBar.value = progress;
            }

            if (progressText != null)
            {
                progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
            }

            // Update scan ring rotation
            if (scanRing != null)
            {
                scanRing.transform.Rotate(0, 0, -90f * Time.deltaTime);
            }
        }

        /// <summary>
        /// Update scan instructions
        /// </summary>
        private void UpdateInstructions()
        {
            if (scanInstructions == null || scanInstructions.Length == 0) return;

            instructionTimer += Time.deltaTime;

            if (instructionTimer >= instructionRotationInterval)
            {
                instructionTimer = 0f;
                currentInstructionIndex = (currentInstructionIndex + 1) % scanInstructions.Length;
                ShowInstruction(currentInstructionIndex);
            }
        }

        /// <summary>
        /// Show instruction text
        /// </summary>
        private void ShowInstruction(int index)
        {
            if (instructionText != null && scanInstructions != null && index < scanInstructions.Length)
            {
                instructionText.text = scanInstructions[index];
            }
        }

        /// <summary>
        /// Update surface type indicators
        /// </summary>
        private void UpdateSurfaceIndicators()
        {
            if (arSessionService == null) return;

            var surfaces = arSessionService.DetectedSurfaces;
            int currentCount = surfaces.Count;

            if (currentCount != previousSurfaceCount)
            {
                previousSurfaceCount = currentCount;

                // Count by type
                int floorCount = 0, wallCount = 0, ceilingCount = 0;

                foreach (var surface in surfaces)
                {
                    switch (surface.Type)
                    {
                        case SurfaceType.Floor:
                            floorCount++;
                            break;
                        case SurfaceType.Wall:
                            wallCount++;
                            break;
                        case SurfaceType.Ceiling:
                            ceilingCount++;
                            break;
                    }
                }

                // Update indicators
                SetIndicatorActive(floorIndicator, floorCount > 0);
                SetIndicatorActive(wallIndicator, wallCount > 0);
                SetIndicatorActive(ceilingIndicator, ceilingCount > 0);

                // Update count text
                if (surfaceCountText != null)
                {
                    surfaceCountText.text = $"{currentCount} surfaces detected";
                }
            }
        }

        /// <summary>
        /// Handle surface detected event
        /// </summary>
        private void HandleSurfaceDetected(SurfaceDetectedData data)
        {
            // Trigger visual feedback
            if (scanAnimator != null)
            {
                scanAnimator.SetTrigger("SurfaceDetected");
            }
        }

        /// <summary>
        /// Handle scan complete event
        /// </summary>
        private void HandleScanComplete(ScanCompleteData data)
        {
            // Show completion
            if (instructionText != null)
            {
                instructionText.text = "Scan Complete!";
            }

            // Auto-advance after delay
            StartCoroutine(CompleteSequence());
        }

        /// <summary>
        /// Completion sequence
        /// </summary>
        private IEnumerator CompleteSequence()
        {
            yield return new WaitForSeconds(1f);
            StopScan();
        }

        /// <summary>
        /// Set indicator active state
        /// </summary>
        private void SetIndicatorActive(GameObject indicator, bool active)
        {
            if (indicator != null)
            {
                indicator.SetActive(active);
            }
        }

        /// <summary>
        /// Fade in animation
        /// </summary>
        private IEnumerator FadeIn()
        {
            if (scanPanel == null) yield break;

            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                scanPanel.alpha = elapsed / fadeInDuration;
                yield return null;
            }
            scanPanel.alpha = 1f;
        }

        /// <summary>
        /// Fade out animation
        /// </summary>
        private IEnumerator FadeOut()
        {
            if (scanPanel == null)
            {
                gameObject.SetActive(false);
                yield break;
            }

            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                scanPanel.alpha = 1f - (elapsed / fadeOutDuration);
                yield return null;
            }
            scanPanel.alpha = 0f;
            gameObject.SetActive(false);
        }
    }
}
