using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Material extension methods
    /// </summary>
    public static class MaterialExtensions
    {
        /// <summary>
        /// Set color safely
        /// </summary>
        public static void SetColorSafe(this Material material, string propertyName, Color color)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                material.SetColor(propertyName, color);
            }
        }

        /// <summary>
        /// Set float safely
        /// </summary>
        public static void SetFloatSafe(this Material material, string propertyName, float value)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                material.SetFloat(propertyName, value);
            }
        }

        /// <summary>
        /// Set int safely
        /// </summary>
        public static void SetIntSafe(this Material material, string propertyName, int value)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                material.SetInt(propertyName, value);
            }
        }

        /// <summary>
        /// Set texture safely
        /// </summary>
        public static void SetTextureSafe(this Material material, string propertyName, Texture texture)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                material.SetTexture(propertyName, texture);
            }
        }

        /// <summary>
        /// Set vector safely
        /// </summary>
        public static void SetVectorSafe(this Material material, string propertyName, Vector4 value)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                material.SetVector(propertyName, value);
            }
        }

        /// <summary>
        /// Get color safely
        /// </summary>
        public static Color GetColorSafe(this Material material, string propertyName, Color defaultValue)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                return material.GetColor(propertyName);
            }
            return defaultValue;
        }

        /// <summary>
        /// Get float safely
        /// </summary>
        public static float GetFloatSafe(this Material material, string propertyName, float defaultValue)
        {
            if (material != null && material.HasProperty(propertyName))
            {
                return material.GetFloat(propertyName);
            }
            return defaultValue;
        }

        /// <summary>
        /// Enable keyword
        /// </summary>
        public static void EnableKeywordSafe(this Material material, string keyword)
        {
            if (material != null)
            {
                material.EnableKeyword(keyword);
            }
        }

        /// <summary>
        /// Disable keyword
        /// </summary>
        public static void DisableKeywordSafe(this Material material, string keyword)
        {
            if (material != null)
            {
                material.DisableKeyword(keyword);
            }
        }
    }
}
