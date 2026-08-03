using UnityEngine;

namespace BreachAR.Utils
{
    /// <summary>
    /// AssetDatabase extension methods
    /// </summary>
    public static class AssetDatabaseExtensions
    {
        /// <summary>
        /// Load asset at path
        /// </summary>
        public static T LoadAsset<T>(string path) where T : Object
        {
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            #else
            return Resources.Load<T>(path);
            #endif
        }

        /// <summary>
        /// Find assets by type
        /// </summary>
        public static T[] FindAssetsOfType<T>() where T : Object
        {
            #if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            T[] assets = new T[guids.Length];
            for (int i = 0; i < guids.Length; i++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[i]);
                assets[i] = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
            }
            return assets;
            #else
            return new T[0];
            #endif
        }

        /// <summary>
        /// Save asset
        /// </summary>
        public static void SaveAsset(Object asset)
        {
            #if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(asset);
            UnityEditor.AssetDatabase.SaveAssets();
            #endif
        }

        /// <summary>
        /// Refresh asset database
        /// </summary>
        public static void Refresh()
        {
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
            #endif
        }

        /// <summary>
        /// Delete asset
        /// </summary>
        public static bool DeleteAsset(string path)
        {
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.DeleteAsset(path);
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Move asset
        /// </summary>
        public static bool MoveAsset(string oldPath, string newPath)
        {
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.MoveAsset(oldPath, newPath) == "";
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Copy asset
        /// </summary>
        public static bool CopyAsset(string oldPath, string newPath)
        {
            #if UNITY_EDITOR
            return UnityEditor.AssetDatabase.CopyAsset(oldPath, newPath);
            #else
            return false;
            #endif
        }

        /// <summary>
        /// Create folder
        /// </summary>
        public static void CreateFolder(string parentPath, string folderName)
        {
            #if UNITY_EDITOR
            string path = parentPath + "/" + folderName;
            if (!UnityEditor.AssetDatabase.IsValidFolder(path))
            {
                UnityEditor.AssetDatabase.CreateFolder(parentPath, folderName);
            }
            #endif
        }
    }
}
