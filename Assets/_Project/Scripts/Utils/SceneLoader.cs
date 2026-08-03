using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using VContainer;

namespace BreachAR.Utils
{
    /// <summary>
    /// Scene loader utility with async loading
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] private CanvasGroup loadingScreen;
        [SerializeField] private UnityEngine.UI.Slider progressBar;
        [SerializeField] private UnityEngine.UI.Text progressText;

        private bool isLoading;



        /// <summary>
        /// Load scene synchronously
        /// </summary>
        public void LoadScene(string sceneName)
        {
            if (isLoading) return;
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Load scene asynchronously
        /// </summary>
        public void LoadSceneAsync(string sceneName)
        {
            if (isLoading) return;
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }

        /// <summary>
        /// Load scene with loading screen
        /// </summary>
        public void LoadSceneWithLoading(string sceneName)
        {
            if (isLoading) return;
            StartCoroutine(LoadSceneWithLoadingCoroutine(sceneName));
        }

        private IEnumerator LoadSceneCoroutine(string sceneName)
        {
            isLoading = true;

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                // Wait until the scene is almost done
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }

            isLoading = false;
        }

        private IEnumerator LoadSceneWithLoadingCoroutine(string sceneName)
        {
            isLoading = true;

            // Show loading screen
            if (loadingScreen != null)
            {
                loadingScreen.alpha = 1;
                loadingScreen.interactable = true;
                loadingScreen.blocksRaycasts = true;
            }

            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);

                // Update UI
                if (progressBar != null)
                    progressBar.value = progress;
                if (progressText != null)
                    progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";

                // Wait until the scene is almost done
                if (asyncLoad.progress >= 0.9f)
                {
                    // Small delay for visual feedback
                    yield return new WaitForSeconds(0.5f);
                    asyncLoad.allowSceneActivation = true;
                }

                yield return null;
            }

            // Hide loading screen
            if (loadingScreen != null)
            {
                loadingScreen.alpha = 0;
                loadingScreen.interactable = false;
                loadingScreen.blocksRaycasts = false;
            }

            isLoading = false;
        }

        /// <summary>
        /// Reload current scene
        /// </summary>
        public void ReloadCurrentScene()
        {
            LoadSceneAsync(SceneManager.GetActiveScene().name);
        }

        /// <summary>
        /// Load main menu
        /// </summary>
        public void LoadMainMenu()
        {
            LoadSceneAsync(GameConstants.SCENE_MAIN_MENU);
        }

        /// <summary>
        /// Load gameplay scene
        /// </summary>
        public void LoadGameplay()
        {
            LoadSceneAsync(GameConstants.SCENE_GAMEPLAY);
        }

        /// <summary>
        /// Quit application
        /// </summary>
        public void Quit()
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #else
            Application.Quit();
            #endif
        }
    }
}
