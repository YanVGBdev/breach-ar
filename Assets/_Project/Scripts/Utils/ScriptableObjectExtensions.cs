using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// ScriptableObject extension methods
    /// </summary>
    public static class ScriptableObjectExtensions
    {
        /// <summary>
        /// Create instance
        /// </summary>
        public static T CreateInstance<T>() where T : ScriptableObject
        {
            return ScriptableObject.CreateInstance<T>();
        }

        /// <summary>
        /// Create instance with name
        /// </summary>
        public static T CreateInstance<T>(string name) where T : ScriptableObject
        {
            T instance = ScriptableObject.CreateInstance<T>();
            instance.name = name;
            return instance;
        }

        /// <summary>
        /// Deep clone
        /// </summary>
        public static T DeepClone<T>(this T scriptableObject) where T : ScriptableObject
        {
            T instance = ScriptableObject.CreateInstance<T>();
            string json = JsonUtility.ToJson(scriptableObject);
            JsonUtility.FromJsonOverwrite(json, instance);
            instance.name = scriptableObject.name + " (Clone)";
            return instance;
        }

        /// <summary>
        /// Save to asset
        /// </summary>
        public static void SaveAsAsset(this ScriptableObject scriptableObject, string path)
        {
            #if UNITY_EDITOR
            string assetPath = path + ".asset";
            UnityEditor.AssetDatabase.CreateAsset(scriptableObject, assetPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        /// <summary>
        /// Load from asset
        /// </summary>
        public static T LoadFromAsset<T>(string path) where T : ScriptableObject
        {
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            #else
            return Resources.Load<T>(path);
            #endif
        }
    }
}
