using UnityEngine;

namespace BreachAR.ScriptableObjects
{
    /// <summary>
    /// Configuration data for orb types
    /// Referência: specs/OrbLaunch.md
    /// </summary>
    [CreateAssetMenu(fileName = "OrbDefinition", menuName = "BreachAR/Orbs/Orb Definition")]
    public class OrbDefinitionSO : ScriptableObject
    {
        [Header("Identity")]
        public string OrbId;
        public string DisplayName;
        [TextArea(2, 4)]
        public string Description;
        public Sprite Icon;

        [Header("Physics")]
        public float Mass = 1f;
        public float GravityScale = 1f;
        public int MaxRicochets = 3;
        public float DamageFalloffPerBounce = 0.1f;

        [Header("Damage")]
        public float BaseDamage = 25f;
        public float[] DamagePerLevel = { 25f, 35f, 50f, 70f, 100f };

        [Header("Area Damage")]
        public bool HasAreaDamage = false;
        public float AreaDamageMultiplier = 0.5f;
        public float[] AreaRadiusPerLevel = { 1f, 1.2f, 1.5f, 1.8f, 2f };

        [Header("Visual")]
        public Color OrbColor = Color.cyan;
        public GameObject Prefab;
        public GameObject TrailEffectPrefab;

        [Header("Audio")]
        public AudioClip LaunchSound;
        public AudioClip RicochetSound;
        public AudioClip ImpactSound;

        /// <summary>
        /// Get damage at upgrade level
        /// </summary>
        public float GetDamageAtLevel(int level)
        {
            if (DamagePerLevel == null || DamagePerLevel.Length == 0)
                return BaseDamage;

            int index = Mathf.Clamp(level, 0, DamagePerLevel.Length - 1);
            return DamagePerLevel[index];
        }

        /// <summary>
        /// Get area radius at upgrade level
        /// </summary>
        public float GetAreaRadiusAtLevel(int level)
        {
            if (AreaRadiusPerLevel == null || AreaRadiusPerLevel.Length == 0)
                return 1f;

            int index = Mathf.Clamp(level, 0, AreaRadiusPerLevel.Length - 1);
            return AreaRadiusPerLevel[index];
        }
    }
}
