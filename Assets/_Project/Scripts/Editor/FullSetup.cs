#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using BreachAR.Gameplay;
using BreachAR.AI;
using BreachAR.Physics;
using BreachAR.AR;
using BreachAR.Utils;

namespace BreachAR.Editor
{
    /// <summary>
    /// Complete automated setup script
    /// Execute via: BreachAR > Run Complete Setup
    /// </summary>
    public class FullSetup : EditorWindow
    {
        [MenuItem("BreachAR/Run Complete Setup", false, 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<FullSetup>("BreachAR Complete Setup");
            window.minSize = new Vector2(400, 500);
        }

        private Vector2 scrollPosition;
        private bool step1Complete = false;
        private bool step2Complete = false;
        private bool step3Complete = false;
        private bool step4Complete = false;
        private bool step5Complete = false;

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            GUILayout.Label("BreachAR Complete Setup", EditorStyles.boldLabel);
            GUILayout.Label("Execute todos os passos em ordem", EditorStyles.miniLabel);
            GUILayout.Space(20);

            // Step 1
            GUI.backgroundColor = step1Complete ? Color.green : Color.white;
            if (GUILayout.Button("1. Criar Estrutura de Pastas", GUILayout.Height(35)))
            {
                CreateFolderStructure();
                step1Complete = true;
            }

            // Step 2
            GUI.backgroundColor = step2Complete ? Color.green : Color.white;
            if (GUILayout.Button("2. Configurar Layers e Tags", GUILayout.Height(35)))
            {
                SetupLayersAndTags();
                step2Complete = true;
            }

            // Step 3
            GUI.backgroundColor = step3Complete ? Color.green : Color.white;
            if (GUILayout.Button("3. Gerar Prefabs", GUILayout.Height(35)))
            {
                GenerateAllPrefabs();
                step3Complete = true;
            }

            // Step 4
            GUI.backgroundColor = step4Complete ? Color.green : Color.white;
            if (GUILayout.Button("4. Criar ScriptableObjects", GUILayout.Height(35)))
            {
                CreateScriptableObjects();
                step4Complete = true;
            }

            // Step 5
            GUI.backgroundColor = step5Complete ? Color.green : Color.white;
            if (GUILayout.Button("5. Criar Cena de Teste", GUILayout.Height(35)))
            {
                CreateTestScene();
                step5Complete = true;
            }

            GUI.backgroundColor = Color.white;
            GUILayout.Space(20);

            // Auto setup all
            GUI.backgroundColor = Color.cyan;
            if (GUILayout.Button("EXECUTAR TUDO AUTOMATICAMENTE", GUILayout.Height(50)))
            {
                RunCompleteSetup();
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(20);

            // Status
            EditorGUILayout.HelpBox(
                $"Status:\n" +
                $"✓ Pastas: {(step1Complete ? "Completo" : "Pendente")}\n" +
                $"✓ Layers/Tags: {(step2Complete ? "Completo" : "Pendente")}\n" +
                $"✓ Prefabs: {(step3Complete ? "Completo" : "Pendente")}\n" +
                $"✓ ScriptableObjects: {(step4Complete ? "Completo" : "Pendente")}\n" +
                $"✓ Cena: {(step5Complete ? "Completo" : "Pendente")}",
                MessageType.Info
            );

            EditorGUILayout.EndScrollView();
        }

        private void RunCompleteSetup()
        {
            EditorUtility.DisplayProgressBar("BreachAR Setup", "Criando estrutura de pastas...", 0.1f);
            CreateFolderStructure();

            EditorUtility.DisplayProgressBar("BreachAR Setup", "Configurando Layers e Tags...", 0.3f);
            SetupLayersAndTags();

            EditorUtility.DisplayProgressBar("BreachAR Setup", "Gerando Prefabs...", 0.5f);
            GenerateAllPrefabs();

            EditorUtility.DisplayProgressBar("BreachAR Setup", "Criando ScriptableObjects...", 0.7f);
            CreateScriptableObjects();

            EditorUtility.DisplayProgressBar("BreachAR Setup", "Criando Cena de Teste...", 0.9f);
            CreateTestScene();

            EditorUtility.ClearProgressBar();

            step1Complete = step2Complete = step3Complete = step4Complete = step5Complete = true;

            EditorUtility.DisplayDialog(
                "Setup Completo!",
                "BreachAR foi configurado com sucesso!\n\n" +
                "Próximos passos:\n" +
                "1. Abra a cena 'TestScene'\n" +
                "2. Configure os prefabs no PoolManager\n" +
                "3. Teste em dispositivo real",
                "OK"
            );
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
                "Scripts/Editor",
                "Scripts/Tests/EditMode",
                "Scripts/Tests/PlayMode",
                "ScriptableObjects/Configs",
                "ScriptableObjects/Waves",
                "ScriptableObjects/Orbs",
                "ScriptableObjects/Fragments",
                "Prefabs/Orbs",
                "Prefabs/Fragments",
                "Prefabs/Rifts",
                "Prefabs/PowerUps",
                "Prefabs/VFX",
                "Prefabs/UI",
                "Prefabs/Core",
                "Art/Materials",
                "Art/Shaders",
                "Art/Textures",
                "Art/Animations",
                "Audio/Music",
                "Audio/SFX",
                "Audio/Ambience",
                "Scenes",
                "Docs"
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

        private void SetupLayersAndTags()
        {
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

            // Setup Layers
            SerializedProperty layers = tagManager.FindProperty("layers");
            SetLayerName(layers, 8, "RealWorldSurface");
            SetLayerName(layers, 9, "Orb");
            SetLayerName(layers, 10, "Fragment");
            SetLayerName(layers, 11, "Rift");
            SetLayerName(layers, 12, "Core");
            SetLayerName(layers, 13, "PowerUp");
            SetLayerName(layers, 14, "ARPlane");

            // Setup Tags
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
            Debug.Log("[Setup] Layers and Tags configured");
        }

        private void SetLayerName(SerializedProperty layers, int index, string name)
        {
            SerializedProperty layer = layers.GetArrayElementAtIndex(index);
            if (string.IsNullOrEmpty(layer.stringValue))
            {
                layer.stringValue = name;
            }
        }

        private void GenerateAllPrefabs()
        {
            GenerateOrbPrefab();
            GenerateFragmentPrefab();
            GenerateRiftPrefab();
            GeneratePowerUpPrefab();
            GenerateCorePrefab();

            AssetDatabase.Refresh();
            Debug.Log("[Setup] All prefabs generated");
        }

        private void GenerateOrbPrefab()
        {
            GameObject orb = CreateBasicPrefab("Orb", Color.cyan);
            orb.tag = "Orb";
            orb.layer = 9;

            var rb = orb.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            var collider = orb.AddComponent<SphereCollider>();
            collider.radius = 0.2f;

            orb.AddComponent<OrbController>();

            var trail = orb.AddComponent<TrailRenderer>();
            trail.startWidth = 0.1f;
            trail.endWidth = 0f;
            trail.time = 0.5f;

            SavePrefab(orb, "Assets/_Project/Prefabs/Orbs/OrbPrefab.prefab");
        }

        private void GenerateFragmentPrefab()
        {
            GameObject fragment = CreateBasicPrefab("Fragment", Color.red);
            fragment.tag = "Fragment";
            fragment.layer = 10;

            var rb = fragment.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            var collider = fragment.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.8f, 0.8f, 0.8f);

            fragment.AddComponent<FragmentController>();

            SavePrefab(fragment, "Assets/_Project/Prefabs/Fragments/FragmentPrefab.prefab");
        }

        private void GenerateRiftPrefab()
        {
            GameObject rift = CreateBasicPrefab("Rift", Color.magenta);
            rift.tag = "Rift";
            rift.layer = 11;

            var collider = rift.AddComponent<BoxCollider>();
            collider.size = new Vector3(2f, 3f, 0.1f);
            collider.isTrigger = true;

            rift.AddComponent<RiftController>();

            var particles = rift.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 2f;
            main.startSpeed = 0.5f;
            main.startSize = 0.1f;
            main.startColor = Color.magenta;
            main.maxParticles = 100;

            SavePrefab(rift, "Assets/_Project/Prefabs/Rifts/RiftPrefab.prefab");
        }

        private void GeneratePowerUpPrefab()
        {
            GameObject powerUp = CreateBasicPrefab("PowerUp", Color.yellow);
            powerUp.tag = "PowerUp";
            powerUp.layer = 13;

            var rb = powerUp.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 0.1f;

            var collider = powerUp.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.3f;

            powerUp.AddComponent<PowerUpController>();

            SavePrefab(powerUp, "Assets/_Project/Prefabs/PowerUps/PowerUpPrefab.prefab");
        }

        private void GenerateCorePrefab()
        {
            GameObject core = CreateBasicPrefab("Core", Color.green);
            core.tag = "Core";
            core.layer = 12;

            var collider = core.AddComponent<BoxCollider>();
            collider.size = new Vector3(1f, 1f, 1f);

            core.AddComponent<CoreController>();

            var light = core.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 5f;
            light.intensity = 1f;
            light.color = Color.green;

            SavePrefab(core, "Assets/_Project/Prefabs/Core/CorePrefab.prefab");
        }

        private GameObject CreateBasicPrefab(string name, Color color)
        {
            GameObject obj = new GameObject(name);

            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(obj.transform);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = Vector3.one * 0.5f;

            var renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                var material = new Material(Shader.Find("Standard"));
                material.color = color;
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 0.3f);
                renderer.material = material;
            }

            var defaultCollider = cube.GetComponent<Collider>();
            if (defaultCollider != null)
            {
                DestroyImmediate(defaultCollider);
            }

            return obj;
        }

        private void SavePrefab(GameObject obj, string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            PrefabUtility.SaveAsPrefabAsset(obj, path);
            DestroyImmediate(obj);
        }

        private void CreateScriptableObjects()
        {
            string configPath = "Assets/_Project/ScriptableObjects/Configs";

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

        private void CreateTestScene()
        {
            // Create new scene
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Add AR Session
            var arSession = new GameObject("AR Session");
            arSession.AddComponent<UnityEngine.XR.ARFoundation.ARSession>();

            // Add AR Session Origin
            var origin = new GameObject("AR Session Origin");
            origin.AddComponent<UnityEngine.XR.ARFoundation.ARSessionOrigin>();

            // Add AR Camera
            var cameraObj = new GameObject("AR Camera");
            cameraObj.transform.SetParent(origin.transform);
            cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<UnityEngine.XR.ARFoundation.ARCameraManager>();
            cameraObj.AddComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();

            // Add GameManager
            var gameManager = new GameObject("GameManager");
            gameManager.AddComponent<GameManager>();

            // Add PoolManager
            var poolManager = new GameObject("PoolManager");
            poolManager.AddComponent<PoolManager>();

            // Add Gameplay Systems
            var sessionObj = new GameObject("SessionStateMachine");
            sessionObj.AddComponent<SessionStateMachine>();

            var comboObj = new GameObject("ComboSystem");
            comboObj.AddComponent<ComboSystem>();

            var scoreObj = new GameObject("ScoreSystem");
            scoreObj.AddComponent<ScoreSystem>();

            var difficultyObj = new GameObject("DifficultyDirector");
            difficultyObj.AddComponent<DifficultyDirector>();

            var waveObj = new GameObject("WaveGenerator");
            waveObj.AddComponent<WaveGenerator>();

            var riftObj = new GameObject("RiftSpawnDirector");
            riftObj.AddComponent<RiftSpawnDirector>();

            var launchObj = new GameObject("LaunchSystem");
            launchObj.AddComponent<LaunchSystem>();

            var physicsObj = new GameObject("PhysicsManager");
            physicsObj.AddComponent<PhysicsManager>();

            // Add Lighting
            var lightObj = new GameObject("Directional Light");
            var light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Save scene
            string scenePath = "Assets/_Project/Scenes/TestScene.unity";
            string sceneDir = Path.GetDirectoryName(scenePath);
            if (!Directory.Exists(sceneDir))
            {
                Directory.CreateDirectory(sceneDir);
            }

            EditorSceneManager.SaveScene(scene, scenePath);
            Debug.Log("[Setup] Test scene created and saved");
        }
    }
}
#endif
