#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace BreachAR.Editor
{
    /// <summary>
    /// Editor script to configure build settings
    /// Referência: Setup Guide
    /// </summary>
    public class BuildSettingsConfigurator : EditorWindow
    {
        [MenuItem("BreachAR/Configure Build Settings")]
        public static void ShowWindow()
        {
            GetWindow<BuildSettingsConfigurator>("Build Settings");
        }

        private void OnGUI()
        {
            GUILayout.Label("Build Settings Configurator", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("Configure Android Settings", GUILayout.Height(35)))
                ConfigureAndroid();

            if (GUILayout.Button("Configure iOS Settings", GUILayout.Height(35)))
                ConfigureiOS();

            GUILayout.Space(10);

            if (GUILayout.Button("Add Scenes to Build", GUILayout.Height(35)))
                AddScenesToBuild();
        }

        private void ConfigureAndroid()
        {
            // Player Settings
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingBackend.IL2CPP);
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel24;
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel33;

            // Graphics
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.Android, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.Android, new[] {
                UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3,
                UnityEngine.Rendering.GraphicsDeviceType.Vulkan
            });

            // Optimization
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.Android, ManagedStrippingLevel.High);
            PlayerSettings.SetIncrementalIl2cppBuild(BuildTargetGroup.Android, true);

            // Package
            PlayerSettings.applicationIdentifier = "com.breachar.game";
            PlayerSettings.bundleVersion = "1.0.0";

            Debug.Log("[Build] Android settings configured");
            EditorUtility.DisplayDialog("Android", "Android build settings configured!", "OK");
        }

        private void ConfigureiOS()
        {
            // Player Settings
            PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingBackend.IL2CPP);
            PlayerSettings.SetArchitecture(BuildTargetGroup.iOS, 1); // ARM64

            // iOS specific
            PlayerSettings.iOS.targetOSVersionString = "12.0";
            PlayerSettings.iOS.appRequiresManualSigning = false;

            // Graphics
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.iOS, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.iOS, new[] {
                UnityEngine.Rendering.GraphicsDeviceType.Metal
            });

            // Optimization
            PlayerSettings.SetManagedStrippingLevel(BuildTargetGroup.iOS, ManagedStrippingLevel.High);

            // Package
            PlayerSettings.applicationIdentifier = "com.breachar.game";
            PlayerSettings.bundleVersion = "1.0.0";

            Debug.Log("[Build] iOS settings configured");
            EditorUtility.DisplayDialog("iOS", "iOS build settings configured!", "OK");
        }

        private void AddScenesToBuild()
        {
            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[]
            {
                new EditorBuildSettingsScene("Assets/_Project/Scenes/Boot.unity", true),
                new EditorBuildSettingsScene("Assets/_Project/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/_Project/Scenes/Gameplay.unity", true)
            };

            EditorBuildSettings.scenes = scenes;

            Debug.Log("[Build] Scenes added to build settings");
            EditorUtility.DisplayDialog("Scenes", "Scenes added to build settings!", "OK");
        }
    }
}
#endif
