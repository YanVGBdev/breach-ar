using UnityEngine;

namespace BreachAR.ScriptableObjects
{
    /// <summary>
    /// ScriptableObject defining a biome visual theme
    /// </summary>
    [CreateAssetMenu(fileName = "NewBiomeTheme", menuName = "BreachAR/Art/BiomeTheme")]
    public class BiomeThemeSO : ScriptableObject
    {
        [Header("Basic Info")]
        public string BiomeId;
        public string DisplayName;
        [TextArea(2, 5)]
        public string Description;

        [Header("Colors")]
        public Color primaryColor = new Color(0.6f, 0.2f, 0.8f); // Purple
        public Color secondaryColor = new Color(0.2f, 0.8f, 0.8f); // Cyan
        public Color accentColor = new Color(1f, 0.5f, 0f); // Orange

        [Header("Rift Settings")]
        public GameObject riftPrefab;
        public Color riftColor = new Color(0.6f, 0.2f, 0.8f);
        public float riftPulseSpeed = 1f;

        [Header("Fragment Settings")]
        public FragmentDefinitionSO[] availableFragments;
        public Color fragmentColor = new Color(0.6f, 0.2f, 0.8f);

        [Header("Environment")]
        public Material skyboxMaterial;
        public Color ambientLightColor = new Color(0.2f, 0.2f, 0.3f);
        public float ambientIntensity = 1f;

        [Header("Audio")]
        public AudioClip backgroundMusic;
        public AudioClip ambientSound;
        public float musicVolume = 0.7f;

        [Header("VFX")]
        public GameObject ambientVFXPrefab;
        public Color vfxTintColor = Color.white;

        [Header("UI")]
        public Sprite biomeIcon;
        public Color uiTintColor = new Color(0.6f, 0.2f, 0.8f);

        /// <summary>
        /// Apply biome theme to environment
        /// </summary>
        public void ApplyTheme()
        {
            // Apply ambient light
            RenderSettings.ambientLight = ambientLightColor * ambientIntensity;

            // Apply skybox
            if (skyboxMaterial != null)
            {
                RenderSettings.skybox = skyboxMaterial;
            }

            Debug.Log($"[Biome] Applied theme: {DisplayName}");
        }

        /// <summary>
        /// Get a random fragment type from this biome
        /// </summary>
        public FragmentDefinitionSO GetRandomFragment()
        {
            if (availableFragments == null || availableFragments.Length == 0)
                return null;

            return availableFragments[Random.Range(0, availableFragments.Length)];
        }

        /// <summary>
        /// Get fragment color for this biome
        /// </summary>
        public Color GetFragmentColor(FragmentType type)
        {
            switch (type)
            {
                case FragmentType.Elite:
                    return accentColor;
                case FragmentType.Explosive:
                    return Color.red;
                default:
                    return fragmentColor;
            }
        }
    }
}
