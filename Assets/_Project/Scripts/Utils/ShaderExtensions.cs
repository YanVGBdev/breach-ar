using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Shader extension methods
    /// </summary>
    public static class ShaderExtensions
    {
        /// <summary>
        /// Get property ID
        /// </summary>
        public static int PropertyToID(string propertyName)
        {
            return Shader.PropertyToID(propertyName);
        }

        /// <summary>
        /// Check if shader has property
        /// </summary>
        public static bool HasProperty(this Shader shader, string propertyName)
        {
            return shader.FindPropertyIndex(propertyName) != -1;
        }

        /// <summary>
        /// Get render queue
        /// </summary>
        public static int GetRenderQueue(this Shader shader)
        {
            return shader.renderQueue;
        }

        /// <summary>
        /// Check if shader is supported
        /// </summary>
        public static bool IsSupported(this Shader shader)
        {
            return shader != null && shader.isSupported;
        }
    }
}
