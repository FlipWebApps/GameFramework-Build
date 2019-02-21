using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Internal.Scripts.Editor
{
    public class FwaLifeCycleShared : MonoBehaviour {

        // Use this for initialization
        void Start () {
	
        }

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
    }
}
