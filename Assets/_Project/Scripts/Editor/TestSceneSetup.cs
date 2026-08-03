#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using BreachAR.Gameplay;
using BreachAR.AI;
using BreachAR.Physics;
using BreachAR.Utils;

namespace BreachAR.Editor
{
    /// <summary>
    /// Editor script to setup test scene
    /// Referência: Setup Guide
    /// </summary>
    public class TestSceneSetup : EditorWindow
    {
        [MenuItem("BreachAR/Setup Test Scene")]
        public static void ShowWindow()
        {
            GetWindow<TestSceneSetup>("Test Scene Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Test Scene Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Create Gameplay Test Scene", GUILayout.Height(40)))
            {
                CreateGameplayTestScene();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Add AR Session"))
                AddARSession();

            if (GUILayout.Button("Add GameManager"))
                AddGameManager();

            if (GUILayout.Button("Add PoolManager"))
                AddPoolManager();

            if (GUILayout.Button("Add Gameplay Systems"))
                AddGameplaySystems();
        }

        private void CreateGameplayTestScene()
        {
            // Clear existing scene
            foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            {
                DestroyImmediate(root);
            }

            // Create basic scene structure
            AddARSession();
            AddGameManager();
            AddPoolManager();
            AddGameplaySystems();
            AddLighting();
            AddCamera();

            Debug.Log("[TestScene] Gameplay test scene created!");
            EditorUtility.DisplayDialog("Done", "Test scene has been created!", "OK");
        }

        private void AddARSession()
        {
            // AR Session
            var arSession = new GameObject("AR Session");
            arSession.AddComponent<UnityEngine.XR.ARFoundation.ARSession>();
            arSession.AddComponent<UnityEngine.XR.Management.XRGeneralSettings>();

            // AR Session Origin
            var origin = new GameObject("AR Session Origin");
            origin.AddComponent<UnityEngine.XR.ARFoundation.ARSessionOrigin>();

            // AR Camera
            var cameraObj = new GameObject("AR Camera");
            cameraObj.transform.SetParent(origin.transform);
            cameraObj.AddComponent<Camera>();
            cameraObj.AddComponent<UnityEngine.XR.ARFoundation.ARCameraManager>();
            cameraObj.AddComponent<UnityEngine.XR.ARFoundation.ARCameraBackground>();

            Debug.Log("[TestScene] AR Session added");
        }

        private void AddGameManager()
        {
            var gameManager = new GameObject("GameManager");
            gameManager.AddComponent<GameManager>();

            Debug.Log("[TestScene] GameManager added");
        }

        private void AddPoolManager()
        {
            var poolManager = new GameObject("PoolManager");
            poolManager.AddComponent<PoolManager>();

            Debug.Log("[TestScene] PoolManager added");
        }

        private void AddGameplaySystems()
        {
            // Session State Machine
            var sessionObj = new GameObject("SessionStateMachine");
            sessionObj.AddComponent<SessionStateMachine>();

            // Combo System
            var comboObj = new GameObject("ComboSystem");
            comboObj.AddComponent<ComboSystem>();

            // Score System
            var scoreObj = new GameObject("ScoreSystem");
            scoreObj.AddComponent<ScoreSystem>();

            // Difficulty Director
            var difficultyObj = new GameObject("DifficultyDirector");
            difficultyObj.AddComponent<DifficultyDirector>();

            // Wave Generator
            var waveObj = new GameObject("WaveGenerator");
            waveObj.AddComponent<WaveGenerator>();

            // Rift Spawn Director
            var riftObj = new GameObject("RiftSpawnDirector");
            riftObj.AddComponent<RiftSpawnDirector>();

            // Launch System
            var launchObj = new GameObject("LaunchSystem");
            launchObj.AddComponent<LaunchSystem>();

            // Physics Manager
            var physicsObj = new GameObject("PhysicsManager");
            physicsObj.AddComponent<PhysicsManager>();

            Debug.Log("[TestScene] Gameplay systems added");
        }

        private void AddLighting()
        {
            // Directional Light
            var lightObj = new GameObject("Directional Light");
            var light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1f;
            light.color = Color.white;
            lightObj.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Ambient Light
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.7f, 0.8f, 0.9f);
            RenderSettings.ambientEquatorColor = new Color(0.6f, 0.6f, 0.6f);
            RenderSettings.ambientGroundColor = new Color(0.4f, 0.4f, 0.4f);

            Debug.Log("[TestScene] Lighting added");
        }

        private void AddCamera()
        {
            var cameraObj = new GameObject("Main Camera");
            var camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.2f, 0.2f, 0.3f);
            cameraObj.tag = "MainCamera";

            Debug.Log("[TestScene] Camera added");
        }
    }
}
#endif
