using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// GameObject extension methods
    /// </summary>
    public static class GameObjectExtensions
    {
        /// <summary>
        /// Get or add component
        /// </summary>
        public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (component == null)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        /// <summary>
        /// Check if has component
        /// </summary>
        public static bool HasComponent<T>(this GameObject gameObject) where T : Component
        {
            return gameObject.GetComponent<T>() != null;
        }

        /// <summary>
        /// Set layer recursively
        /// </summary>
        public static void SetLayerRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetLayerRecursively(layer);
            }
        }

        /// <summary>
        /// Set active state recursively
        /// </summary>
        public static void SetActiveRecursively(this GameObject gameObject, bool active)
        {
            gameObject.SetActive(active);
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.SetActiveRecursively(active);
            }
        }

        /// <summary>
        /// Destroy all children
        /// </summary>
        public static void DestroyAllChildren(this GameObject gameObject)
        {
            foreach (Transform child in gameObject.transform)
            {
                Object.Destroy(child.gameObject);
            }
        }

        /// <summary>
        /// Destroy all children immediately
        /// </summary>
        public static void DestroyAllChildrenImmediate(this GameObject gameObject)
        {
            foreach (Transform child in gameObject.transform)
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }

        /// <summary>
        /// Get path in hierarchy
        /// </summary>
        public static string GetPath(this GameObject gameObject)
        {
            string path = gameObject.name;
            Transform parent = gameObject.transform.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }
    }
}
