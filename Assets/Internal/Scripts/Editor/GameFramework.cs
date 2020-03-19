using UnityEditor;
using UnityEngine;
using System.IO;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace Internal.Scripts.Editor
{
    public class GameFramework : MonoBehaviour
    {
        static string[] _testScenePaths =
        {
            "Assets/FlipWebApps/GameFramework/_Demo/GameStructure/GameStructure-Title.unity",
            "Assets/FlipWebApps/GameFramework/_Demo/GameStructure/GameStructure-Menu.unity",
            "Assets/FlipWebApps/GameFramework/_Demo/GameStructure/GameStructure-Character.unity",
            "Assets/FlipWebApps/GameFramework/_Demo/GameStructure/GameStructure-Game.unity"
        };


        [MenuItem("FlipWebApps/Game Framework/Archive Uploaded Asset")]
        static void ArchiveUploadedAsset()
        {
            var readmePath = System.IO.Path.Combine(Application.dataPath, @"..\Assets\FlipWebApps\GameFramework\Release Notes.txt");
            var exportedAssetPath = System.IO.Path.Combine(Application.dataPath, @"..\Temp\uploadtool_FlipWebApps_GameFramework.unitypackage");
            const string archivePathTemplate = @"I:\OneDrive\Documents\Mark\Unity\Games\Game Framework\Releases\Free\GameFramework - Free v{0}({1}).unitypackage";

            FwaLifeCycleShared.ArchiveAndVerifyAsset(readmePath, exportedAssetPath, archivePathTemplate);
        }


        [MenuItem("FlipWebApps/Game Framework/Build Windows 64-bit")]
        static void Win64Build()
        {
            var outputFolder = FwaLifeCycleShared.GetCommandLineArgumentValue("BuildOutputDirectory") ?? "Builds/Win64";

            FwaLifeCycleShared.EvaluateBuildReport(BuildPipeline.BuildPlayer(_testScenePaths, Path.Combine(outputFolder, "Demo.exe"), BuildTarget.StandaloneWindows64, BuildOptions.None));
        }


        [MenuItem("FlipWebApps/Game Framework/Build Android")]
        static void AndroidBuild()
        {
            var outputFolder = FwaLifeCycleShared.GetCommandLineArgumentValue("BuildOutputDirectory") ?? "Builds/Android";

            var oldCompanyName = PlayerSettings.companyName;
            var oldProductName = PlayerSettings.productName;
            var oldIdentifier = PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);

            PlayerSettings.companyName = "Flip Web Apps";
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.flipwebapps.gameframework");

            PlayerSettings.productName = "Getting Started";
            FwaLifeCycleShared.EvaluateBuildReport(BuildPipeline.BuildPlayer(_testScenePaths, Path.Combine(outputFolder, "Demo.apk"), BuildTarget.Android, BuildOptions.None));


            PlayerSettings.companyName = oldCompanyName;
            PlayerSettings.productName = oldProductName;
            PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, oldIdentifier);
        }


        [MenuItem("FlipWebApps/Game Framework/Build WebGL")]
        static void WebGLBuild()
        {
            var outputFolder = FwaLifeCycleShared.GetCommandLineArgumentValue("BuildOutputDirectory") ?? "Builds/WebGL";

            var oldProductName = PlayerSettings.productName;

            PlayerSettings.productName = "Game Framework - Getting Started";
            FwaLifeCycleShared.EvaluateBuildReport(BuildPipeline.BuildPlayer(_testScenePaths, Path.Combine(outputFolder, "demo"), BuildTarget.WebGL, BuildOptions.None));

            PlayerSettings.productName = oldProductName;
        }


        [MenuItem("FlipWebApps/Game Framework/Build All")]
        static void PerformBuilds()
        {
            Win64Build();
            AndroidBuild();
            WebGLBuild();
        }


        [MenuItem("FlipWebApps/Game Framework/Add Scenes")]
        static void GameFrameworkAddScenes()
        {
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[4]
            {
                new EditorBuildSettingsScene("Assets/FlipWebApps/GameFramework/_Demo/GameStructure/GameStructure-Title.unity", true),
                new EditorBuildSettingsScene("Assets/FlipWebApps/GameFramework/_Demo/GameStructure/GameStructure-Menu.unity", true),
                new EditorBuildSettingsScene("Assets/FlipWebApps/GameFramework/_Demo/GameStructure/GameStructure-Character.unity", true),
                new EditorBuildSettingsScene("Assets/FlipWebApps/GameFramework/_Demo/GameStructure/GameStructure-Game.unity", true)
            };

            Debug.Log("All scenes added.");
        }
    }
}