//
// NOTE: Game Framework Full is the master copy
//
using UnityEditor;
using UnityEngine;

#if UNITY_2018_1_OR_NEWER
using UnityEditor.Build.Reporting;
#endif

namespace Internal.Scripts.Editor
{
    public class FwaLifeCycleShared : MonoBehaviour
    {

        [MenuItem("Assets/Copy Path")]
        static void CopyPath()
        {
            var selected = Selection.activeObject;
            EditorGUIUtility.systemCopyBuffer = AssetDatabase.GetAssetPath(selected);
        }

        [MenuItem("FlipWebApps/Clean All Scenes and Defines")]
        static void CleanScenesAndDefines()
        {
            EditorBuildSettings.scenes = new EditorBuildSettingsScene[0];

            foreach (BuildTargetGroup target in System.Enum.GetValues(typeof(BuildTargetGroup)))
            {
                if (target == BuildTargetGroup.Unknown) continue;
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, null);
            }
            Debug.Log("All scenes and defines cleared.");
        }

        #region Build Functionality
        /// <summary>
        /// Evaluate Build Report and if batch mode exit on error - Unity 2017 version where BuildPlayer returns a string
        /// </summary>
        /// <param name="report"></param>
        public static void EvaluateBuildReport(string report)
        {
            if (IsBatchMode() && !string.IsNullOrEmpty(report))
                EditorApplication.Exit(1);
        }


        /// <summary>
        /// Evaluate build report and if batch mode exit on error
        /// </summary>
        /// <param name="report"></param>
#if UNITY_2018_1_OR_NEWER
        public static void EvaluateBuildReport(BuildReport report)
        {
            if (IsBatchMode() && report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                EditorApplication.Exit(1);
        }
#endif


        /// <summary>
        /// Return whether running in batch mode (e.g. from a build)
        /// </summary>
        /// <returns></returns>
        public static bool IsBatchMode()
        {
#if UNITY_2018_2_OR_NEWER
            return Application.isBatchMode;
#else
            return UnityEditorInternal.InternalEditorUtility.inBatchMode;
            //bool isBatchMode =  System.Environment.CommandLine.Contains("-batchmode");
#endif
        }
        #endregion Build Functionality


        /// <summary>
        /// Archive an uploaded asset and perform some validation checks.
        /// </summary>
        /// <param name="readmePath"></param>
        /// <param name="exportedAssetPath"></param>
        /// <param name="archivePathTemplate"></param>
        public static void ArchiveAndVerifyAsset(string readmePath, string exportedAssetPath, string archivePathTemplate)
        {
            string releaseVersion;
            using (var streamReader = new System.IO.StreamReader(System.IO.Path.Combine(Application.dataPath, "Version.txt")))
                releaseVersion = streamReader.ReadLine();
            Assert.IsNotNull(releaseVersion, "Version.txt not read");

            var archivePath = string.Format(archivePathTemplate, releaseVersion, Application.unityVersion);
            if (System.IO.File.Exists(archivePath))
                if (!EditorUtility.DisplayDialog("Replace existing backup?", "An existing backup already exists. Are you sure you want to replace?", "Yes", "No"))
                    return;

            System.IO.File.Copy(exportedAssetPath, archivePath, true);
            Debug.Log("Uploaded asset archived to " + archivePath);


            // Verify versions are updated in readme
            string line;
            bool firstLineRead = false;
            bool changesFound = false;
            using (var streamReader = new System.IO.StreamReader(readmePath))
            {
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (firstLineRead)
                    {
                        // find line with changes
                        if (line.StartsWith("v" + releaseVersion))
                            changesFound = true;
                    }
                    else
                    {
                        // check header contains correct version
                        if (!line.Contains("v" + releaseVersion))
                            Debug.LogWarning(string.Format("The Readme first line contains '{0}'. It should have version '{1}'", line, releaseVersion));
                        firstLineRead = true;
                    }
                }
                if (!changesFound)
                    Debug.LogWarning(string.Format("The Readme doesn't contain changes for version '{0}'!", releaseVersion));
            }
        }
    }
}
