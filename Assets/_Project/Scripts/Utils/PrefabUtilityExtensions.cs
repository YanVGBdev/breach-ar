using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// PrefabUtility extension methods
    /// </summary>
    public static class PrefabUtilityExtensions
    {
        /// <summary>
        /// Check if is prefab
        /// </summary>
        public static bool IsPrefab(this GameObject gameObject)
        {
            #if UNITY_EDITOR
            return UnityEditor.PrefabUtility.IsPartOfPrefabAsset(gameObject);
            #else
            return gameObject.scene.name == null;
            #endif
        }

        /// <summary>
        /// Check if is prefab instance
        /// </summary>
        public static bool IsPrefabInstance(this GameObject gameObject)
        {
            #if UNITY_EDITOR
            return UnityEditor.PrefabUtility.IsPartOfPrefabInstance(gameObject);
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Get prefab asset
        /// </summary>
        public static GameObject GetPrefabAsset(this GameObject gameObject)
        {
            #if UNITY_EDITOR
            return UnityEditor.PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
            #else
            return null;
            #endif
        }

        /// <summary>
        /// Get prefab path
        /// </summary>
        public static string GetPrefabPath(this GameObject gameObject)
        {
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.GetAssetPath(gameObject);
            #else
            return "";
            #endif
        }

        /// <summary>
        /// Instantiate prefab
        /// </summary>
        public static GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            return Object.Instantiate(prefab, position, rotation);
        }

        /// <summary>
        /// Instantiate prefab as child
        /// </summary>
        public static GameObject InstantiatePrefabAsChild(GameObject prefab, Transform parent)
        {
            return Object.Instantiate(prefab, parent);
        }
    }
}
