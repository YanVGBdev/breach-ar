using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// Component extension methods
    /// </summary>
    public static class ComponentExtensions
    {
        /// <summary>
        /// Get or add component
        /// </summary>
        public static T GetOrAddComponent<T>(this Component component) where T : Component
        {
            T foundComponent = component.GetComponent<T>();
            if (foundComponent == null)
            {
                foundComponent = component.gameObject.AddComponent<T>();
            }
            return foundComponent;
        }

        /// <summary>
        /// Check if has component
        /// </summary>
        public static bool HasComponent<T>(this Component component) where T : Component
        {
            return component.GetComponent<T>() != null;
        }

        /// <summary>
        /// Get component in children
        /// </summary>
        public static T GetComponentInChildren<T>(this Component component, bool includeInactive) where T : Component
        {
            return component.GetComponentInChildren<T>(includeInactive);
        }

        /// <summary>
        /// Get component in parent
        /// </summary>
        public static T GetComponentInParent<T>(this Component component, bool includeInactive) where T : Component
        {
            return component.GetComponentInParent<T>(includeInactive);
        }
    }
}
