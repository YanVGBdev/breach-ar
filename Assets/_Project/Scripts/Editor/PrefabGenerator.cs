#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using BreachAR.Gameplay;
using BreachAR.Utils;

namespace BreachAR.Editor
{
    /// <summary>
    /// Editor script to generate placeholder prefabs
    /// Referência: GP-038, Setup Guide
    /// </summary>
    public class PrefabGenerator : EditorWindow
    {
        private string prefabFolder = "Assets/_Project/Prefabs";

        [MenuItem("BreachAR/Generate Placeholder Prefabs")]
        public static void ShowWindow()
        {
            GetWindow<PrefabGenerator>("Prefab Generator");
        }

        private void OnGUI()
        {
            GUILayout.Label("Generate Placeholder Prefabs", EditorStyles.boldLabel);
            GUILayout.Space(10);

            prefabFolder = EditorGUILayout.TextField("Prefab Folder", prefabFolder);

            GUILayout.Space(20);

            if (GUILayout.Button("Generate All Prefabs", GUILayout.Height(40)))
            {
                GenerateAllPrefabs();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Orb Prefab"))
                GenerateOrbPrefab();

            if (GUILayout.Button("Generate Fragment Prefab"))
                GenerateFragmentPrefab();

            if (GUILayout.Button("Generate Rift Prefab"))
                GenerateRiftPrefab();

            if (GUILayout.Button("Generate PowerUp Prefab"))
                GeneratePowerUpPrefab();
        }

        private void GenerateAllPrefabs()
        {
            GenerateOrbPrefab();
            GenerateFragmentPrefab();
            GenerateRiftPrefab();
            GeneratePowerUpPrefab();
            GenerateCorePrefab();

            Debug.Log("[PrefabGen] All prefabs generated!");
            EditorUtility.DisplayDialog("Done", "All placeholder prefabs have been generated!", "OK");
        }

        private void GenerateOrbPrefab()
        {
            GameObject orb = CreateBasicPrefab("Orb", Color.cyan);
            orb.tag = GameConstants.TAG_ORB;
            orb.layer = GameConstants.LAYER_ORB;

            // Add components
            var rb = orb.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.useGravity = true;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            var collider = orb.AddComponent<SphereCollider>();
            collider.radius = 0.2f;

            var orbController = orb.AddComponent<OrbController>();

            // Add trail renderer
            var trail = orb.AddComponent<TrailRenderer>();
            trail.startWidth = 0.1f;
            trail.endWidth = 0f;
            trail.time = 0.5f;

            SavePrefab(orb, "Orbs/OrbPrefab");
        }

        private void GenerateFragmentPrefab()
        {
            GameObject fragment = CreateBasicPrefab("Fragment", Color.red);
            fragment.tag = GameConstants.TAG_FRAGMENT;
            fragment.layer = GameConstants.LAYER_FRAGMENT;

            // Add components
            var rb = fragment.AddComponent<Rigidbody>();
            rb.isKinematic = true;

            var collider = fragment.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.8f, 0.8f, 0.8f);

            var fragmentController = fragment.AddComponent<FragmentController>();

            SavePrefab(fragment, "Fragments/FragmentPrefab");
        }

        private void GenerateRiftPrefab()
        {
            GameObject rift = CreateBasicPrefab("Rift", Color.magenta);
            rift.tag = GameConstants.TAG_RIFT;
            rift.layer = GameConstants.LAYER_RIFT;

            // Add components
            var collider = rift.AddComponent<BoxCollider>();
            collider.size = new Vector3(2f, 3f, 0.1f);
            collider.isTrigger = true;

            var riftController = rift.AddComponent<RiftController>();

            // Add particle system for effect
            var particles = rift.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 2f;
            main.startSpeed = 0.5f;
            main.startSize = 0.1f;
            main.startColor = Color.magenta;
            main.maxParticles = 100;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            SavePrefab(rift, "Rifts/RiftPrefab");
        }

        private void GeneratePowerUpPrefab()
        {
            GameObject powerUp = CreateBasicPrefab("PowerUp", Color.yellow);
            powerUp.tag = GameConstants.TAG_POWERUP;
            powerUp.layer = GameConstants.LAYER_POWERUP;

            // Add components
            var rb = powerUp.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.mass = 0.1f;

            var collider = powerUp.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = 0.3f;

            var powerUpController = powerUp.AddComponent<PowerUpController>();

            SavePrefab(powerUp, "PowerUps/PowerUpPrefab");
        }

        private void GenerateCorePrefab()
        {
            GameObject core = CreateBasicPrefab("Core", Color.green);
            core.tag = GameConstants.TAG_CORE;
            core.layer = GameConstants.LAYER_CORE;

            // Add components
            var collider = core.AddComponent<BoxCollider>();
            collider.size = new Vector3(1f, 1f, 1f);

            var coreController = core.AddComponent<CoreController>();

            // Add light for visual feedback
            var light = core.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 5f;
            light.intensity = 1f;
            light.color = Color.green;

            SavePrefab(core, "Core/CorePrefab");
        }

        private GameObject CreateBasicPrefab(string name, Color color)
        {
            GameObject obj = new GameObject(name);

            // Create visual representation
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.SetParent(obj.transform);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = Vector3.one * 0.5f;

            // Set material color
            var renderer = cube.GetComponent<Renderer>();
            if (renderer != null)
            {
                var material = new Material(Shader.Find("Standard"));
                material.color = color;
                material.EnableKeyword("_EMISSION");
                material.SetColor("_EmissionColor", color * 0.3f);
                renderer.material = material;
            }

            // Remove default collider from cube (we'll add our own)
            var defaultCollider = cube.GetComponent<Collider>();
            if (defaultCollider != null)
            {
                DestroyImmediate(defaultCollider);
            }

            return obj;
        }

        private void SavePrefab(GameObject obj, string path)
        {
            string fullPath = $"{prefabFolder}/{path}.prefab";
            string directory = System.IO.Path.GetDirectoryName(fullPath);

            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            PrefabUtility.SaveAsPrefabAsset(obj, fullPath);
            DestroyImmediate(obj);

            Debug.Log($"[PrefabGen] Created: {fullPath}");
        }
    }
}
#endif
