#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace BreachAR.Editor
{
    /// <summary>
    /// Editor wizard for automated project setup
    /// Referência: AR-001, Setup Guide
    /// </summary>
    public class ProjectSetupWizard : EditorWindow
    {
        private bool setupLayers = true;
        private bool setupPhysics = true;
        private bool setupTags = true;
        private bool createFolders = true;
        private bool createScriptableObjects = true;

        [MenuItem("BreachAR/Project Setup Wizard")]
        public static void ShowWindow()
        {
            GetWindow<ProjectSetupWizard>("BreachAR Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("BreachAR Project Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            setupLayers = EditorGUILayout.Toggle("Setup Layers", setupLayers);
            setupPhysics = EditorGUILayout.Toggle("Setup Physics Matrix", setupPhysics);
            setupTags = EditorGUILayout.Toggle("Setup Tags", setupTags);
            createFolders = EditorGUILayout.Toggle("Create Folder Structure", createFolders);
            createScriptableObjects = EditorGUILayout.Toggle("Create ScriptableObjects", createScriptableObjects);

            GUILayout.Space(20);

            if (GUILayout.Button("Run Setup", GUILayout.Height(40)))
            {
                RunSetup();
            }
        }

        private void RunSetup()
        {
            if (setupLayers) SetupLayers();
            if (setupPhysics) SetupPhysicsMatrix();
            if (setupTags) SetupTags();
            if (createFolders) CreateFolderStructure();
            if (createScriptableObjects) CreateScriptableObjects();

            Debug.Log("[Setup] Project setup complete!");
            EditorUtility.DisplayDialog("Setup Complete", "BreachAR project has been configured!", "OK");
        }

        private void SetupLayers()
        {
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            SerializedProperty layers = tagManager.FindProperty("layers");

            // Set layer names (layers 8-15 are user-definable)
            SetLayerName(layers, 8, "RealWorldSurface");
            SetLayerName(layers, 9, "Orb");
            SetLayerName(layers, 10, "Fragment");
            SetLayerName(layers, 11, "Rift");
            SetLayerName(layers, 12, "Core");
            SetLayerName(layers, 13, "PowerUp");
            SetLayerName(layers, 14, "ARPlane");

            tagManager.ApplyModifiedProperties();
            Debug.Log("[Setup] Layers configured");
        }

        private void SetLayerName(SerializedProperty layers, int index, string name)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(index);
            if (string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = name;
            }
        }

        private void SetupPhysicsMatrix()
        {
            // Configure physics collisions via script
            // Note: Physics matrix cannot be set via Editor script directly
            // This must be done manually or via PlayerSettings
            Debug.Log("[Setup] Physics matrix - Please configure manually in Project Settings > Physics");
        }

        private void SetupTags()
        {
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            SerializedProperty tags = tagManager.FindProperty("tags");

            string[] newTags = { "Orb", "Fragment", "Rift", "Core", "PowerUp", "VFX" };

            foreach (string tag in newTags)
            {
                bool found = false;
                for (int i = 0; i < tags.arraySize; i++)
                {
                    if (tags.GetArrayElementAtIndex(i).stringValue == tag)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    tags.InsertArrayElementAtIndex(tags.arraySize);
                    tags.GetArrayElementAtIndex(tags.arraySize - 1).stringValue = tag;
                }
            }

            tagManager.ApplyModifiedProperties();
            Debug.Log("[Setup] Tags configured");
        }

        private void CreateFolderStructure()
        {
            string basePath = "Assets/_Project";

            string[] folders = {
                "Scripts/Core",
                "Scripts/Gameplay/Orbs",
                "Scripts/Gameplay/Fragments",
                "Scripts/Gameplay/Rifts",
                "Scripts/Gameplay/Combo",
                "Scripts/Gameplay/Powerups",
                "Scripts/Gameplay/Bosses",
                "Scripts/AI",
                "Scripts/Physics",
                "Scripts/Audio",
                "Scripts/UI/HUD",
                "Scripts/UI/Screens",
                "Scripts/Backend/Save",
                "Scripts/Backend/Networking",
                "Scripts/AR",
                "Scripts/Analytics",
                "Scripts/Utils",
                "Scripts/Tests/EditMode",
                "Scripts/Tests/PlayMode",
                "ScriptableObjects/Configs",
                "ScriptableObjects/Waves",
                "ScriptableObjects/Orbs",
                "ScriptableObjects/Fragments",
                "Prefabs/Orbs",
                "Prefabs/Fragments",
                "Prefabs/Rifts",
                "Prefabs/VFX",
                "Prefabs/UI",
                "Art/Materials",
                "Art/Shaders",
                "Art/Animations",
                "Audio/Music",
                "Audio/SFX",
                "Audio/Ambience",
                "Scenes"
            };

            foreach (string folder in folders)
            {
                string fullPath = Path.Combine(basePath, folder);
                if (!Directory.Exists(fullPath))
                {
                    Directory.CreateDirectory(fullPath);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("[Setup] Folder structure created");
        }

        private void CreateScriptableObjects()
        {
            string configPath = "Assets/_Project/ScriptableObjects/Configs";

            // Create default configs if they don't exist
            CreateIfNotExists<DifficultyConfig>(configPath, "DifficultyConfig");
            CreateIfNotExists<WaveGenerationConfig>(configPath, "WaveGenerationConfig");

            AssetDatabase.Refresh();
            Debug.Log("[Setup] ScriptableObjects created");
        }

        private void CreateIfNotExists<T>(string path, string name) where T : ScriptableObject
        {
            string fullPath = Path.Combine(path, $"{name}.asset");
            if (!File.Exists(fullPath))
            {
                T config = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(config, fullPath);
            }
        }
    }
}
#endif
